using Exchange.Application.Dtos.Requests;
using Exchange.Application.UseCases.ConvertCurrency;
using Exchange.Domain.Entities;
using Exchange.Domain.Enums;
using Exchange.Domain.Interfaces;
using Moq;
using Xunit;

namespace Exchange.Unit.Tests.Application.UseCases
{
    public class ConvertCurrencyUseCaseTests
    {
        private readonly Mock<IExchangeRateProvider> _rateProviderMock;
        private readonly Mock<IConversionRepository> _repositoryMock;
        private readonly ConvertCurrencyUseCase _useCase;

        public ConvertCurrencyUseCaseTests()
        {
            _rateProviderMock = new Mock<IExchangeRateProvider>();
            _repositoryMock = new Mock<IConversionRepository>();
            _useCase = new ConvertCurrencyUseCase(_rateProviderMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenAmountBRLIsZero()
        {
            var request = new ConvertCurrencyRequest(
                ToCurrency: "EUR",
                AmountBRL: 0,
                DateQuotation: DateOnly.FromDateTime(DateTime.Today),
                ExchangeType: ExchangeQuotationEnum.Buy
            );

            var result = await _useCase.ExecuteAsync(request);

            Assert.True(result.IsFailure);
            Assert.Equal("VALIDATION_ERROR", result.Error?.Code);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldConvertCurrency_WhenExchangeTypeIsBuy()
        {
            var request = new ConvertCurrencyRequest(
                ToCurrency: "EUR",
                AmountBRL: 1000m,
                DateQuotation: DateOnly.FromDateTime(DateTime.Today),
                ExchangeType: ExchangeQuotationEnum.Buy
            );

            var exchangeRate = new ExchangeRate("EUR", 6.0m, 6.2m, "2025-08-13T13:00:00");

            _rateProviderMock
                .Setup(x => x.GetExchangeRateAsync(request.ToCurrency, request.DateQuotation))
                .ReturnsAsync(exchangeRate);

            var result = await _useCase.ExecuteAsync(request);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(1000m, result.Value!.OriginalAmount);
            Assert.Equal("BRL", result.Value.FromCurrency);
            Assert.Equal("EUR", result.Value.ToCurrency);
            Assert.Equal(Math.Round(1000m / exchangeRate.BuyRate, 2), result.Value.ConvertedAmount);
            Assert.Equal(Math.Round(exchangeRate.BuyRate, 2), result.Value.ExchangeRate);
            Assert.Equal(ExchangeQuotationEnum.Buy, result.Value.ExchangeType);
            Assert.Equal("BACEN", result.Value.Provider);
            Assert.Equal(new DateOnly(2025, 8, 13), result.Value.DateQuotation);

            _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<ConversionRecord>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldConvertCurrency_WhenExchangeTypeIsSell()
        {
            var request = new ConvertCurrencyRequest(
                ToCurrency: "EUR",
                AmountBRL: 1000m,
                DateQuotation: DateOnly.FromDateTime(DateTime.Today),
                ExchangeType: ExchangeQuotationEnum.Sell
            );

            var exchangeRate = new ExchangeRate("EUR", 6.0m, 6.2m, "2025-08-13T13:00:00");

            _rateProviderMock
                .Setup(x => x.GetExchangeRateAsync(request.ToCurrency, request.DateQuotation))
                .ReturnsAsync(exchangeRate);

            var result = await _useCase.ExecuteAsync(request);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(Math.Round(1000m / exchangeRate.SellRate, 2), result.Value!.ConvertedAmount);
            Assert.Equal(Math.Round(exchangeRate.SellRate, 2), result.Value.ExchangeRate);
            Assert.Equal(ExchangeQuotationEnum.Sell, result.Value.ExchangeType);

            _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<ConversionRecord>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUseRequestDate_WhenProviderDateIsInvalid()
        {
            var requestDate = new DateOnly(2025, 8, 13);
            var request = new ConvertCurrencyRequest("USD", 100m, requestDate, ExchangeQuotationEnum.Buy);
            var exchangeRate = new ExchangeRate("USD", 5m, 5.1m, "invalid-date");

            _rateProviderMock
                .Setup(x => x.GetExchangeRateAsync(request.ToCurrency, request.DateQuotation))
                .ReturnsAsync(exchangeRate);

            var result = await _useCase.ExecuteAsync(request);

            Assert.True(result.IsSuccess);
            Assert.Equal(requestDate, result.Value!.DateQuotation);
        }
    }
}
