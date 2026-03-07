using Exchange.Application.UseCases.GetExchangeRate;
using Exchange.Domain.Entities;
using Exchange.Domain.Interfaces;
using Moq;
using Xunit;

namespace Exchange.Unit.Tests.Application.UseCases
{
    public class GetExchangeRateUseCaseTests
    {
        private readonly Mock<IExchangeRateProvider> _exchangeRateProviderMock;
        private readonly GetExchangeRateUseCase _useCase;

        public GetExchangeRateUseCaseTests()
        {
            _exchangeRateProviderMock = new Mock<IExchangeRateProvider>();
            _useCase = new GetExchangeRateUseCase(_exchangeRateProviderMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenCurrencyIsInvalid()
        {
            var result = await _useCase.ExecuteAsync(" ", new DateOnly(2025, 8, 13));
            Assert.True(result.IsFailure);
            Assert.Equal("VALIDATION_ERROR", result.Error?.Code);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNormalizeCurrency_AndReturnProviderResponseDate()
        {
            var dateQuotation = new DateOnly(2025, 8, 13);
            var rate = new ExchangeRate("EUR", 6.10m, 6.20m, "2025-08-13T13:00:00");

            _exchangeRateProviderMock
                .Setup(x => x.GetExchangeRateAsync("EUR", dateQuotation))
                .ReturnsAsync(rate);

            var result = await _useCase.ExecuteAsync(" eur ", dateQuotation);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("EUR", result.Value!.Currency);
            Assert.Equal(6.10m, result.Value.BuyRate);
            Assert.Equal(6.20m, result.Value.SellRate);
            Assert.Equal(new DateOnly(2025, 8, 13), result.Value.DateQuotation);
            Assert.Equal("BACEN", result.Value.Provider);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUseFallbackDate_WhenProviderDateIsInvalid()
        {
            var dateQuotation = new DateOnly(2025, 8, 13);
            var rate = new ExchangeRate("USD", 5.00m, 5.10m, "data-invalida");

            _exchangeRateProviderMock
                .Setup(x => x.GetExchangeRateAsync("USD", dateQuotation))
                .ReturnsAsync(rate);

            var result = await _useCase.ExecuteAsync("USD", dateQuotation);

            Assert.True(result.IsSuccess);
            Assert.Equal(dateQuotation, result.Value!.DateQuotation);
        }
    }
}
