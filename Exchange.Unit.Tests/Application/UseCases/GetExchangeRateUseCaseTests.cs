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
        public async Task ExecuteAsync_ShouldThrowArgumentException_WhenCurrencyIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(" ", new DateOnly(2025, 8, 13)));
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

            Assert.Equal("EUR", result.Currency);
            Assert.Equal(6.10m, result.BuyRate);
            Assert.Equal(6.20m, result.SellRate);
            Assert.Equal(new DateOnly(2025, 8, 13), result.DateQuotation);
            Assert.Equal("BACEN", result.Provider);
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

            Assert.Equal(dateQuotation, result.DateQuotation);
        }
    }
}
