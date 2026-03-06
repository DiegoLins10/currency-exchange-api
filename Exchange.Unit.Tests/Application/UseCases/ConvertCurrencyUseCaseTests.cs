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
        public async Task ExecuteAsync_ShouldThrowArgumentException_WhenAmountBRLIsZero()
        {
            var request = new ConvertCurrencyRequest(
                ToCurrency: "EUR",
                AmountBRL: 0,
                DateQuotation: DateOnly.FromDateTime(DateTime.Today),
                ExchangeType: ExchangeQuotationEnum.Buy
            );

            await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(request));
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

            var response = await _useCase.ExecuteAsync(request);

            Assert.Equal(1000m, response.OriginalAmount);
            Assert.Equal("BRL", response.FromCurrency);
            Assert.Equal("EUR", response.ToCurrency);
            Assert.Equal(Math.Round(1000m / exchangeRate.BuyRate, 2), response.ConvertedAmount);
            Assert.Equal(Math.Round(exchangeRate.BuyRate, 2), response.ExchangeRate);
            Assert.Equal(ExchangeQuotationEnum.Buy, response.ExchangeType);
            Assert.Equal("BACEN", response.Provider);
            Assert.Equal(new DateOnly(2025, 8, 13), response.DateQuotation);

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

            var response = await _useCase.ExecuteAsync(request);

            Assert.Equal(Math.Round(1000m / exchangeRate.SellRate, 2), response.ConvertedAmount);
            Assert.Equal(Math.Round(exchangeRate.SellRate, 2), response.ExchangeRate);
            Assert.Equal(ExchangeQuotationEnum.Sell, response.ExchangeType);

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

            var response = await _useCase.ExecuteAsync(request);

            Assert.Equal(requestDate, response.DateQuotation);
        }
    }
}
