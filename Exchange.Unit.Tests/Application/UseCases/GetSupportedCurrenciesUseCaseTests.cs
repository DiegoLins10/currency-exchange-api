using Exchange.Application.Interfaces;
using Exchange.Application.UseCases.GetSupportedCurrencies;
using Exchange.Domain.Entities;
using Exchange.Domain.Interfaces;
using Moq;
using Xunit;

namespace Exchange.Unit.Tests.Application.UseCases
{
    public class GetSupportedCurrenciesUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldReturnSupportedCurrenciesFromProvider()
        {
            var providerMock = new Mock<IExchangeRateProvider>();
            var expected = new List<SupportedCurrency>
            {
                new("AUD", "Dólar Australiano"),
                new("EUR", "Euro"),
                new("USD", "Dólar dos EUA")
            };

            providerMock
                .Setup(p => p.GetSupportedCurrenciesAsync())
                .ReturnsAsync(expected);

            IGetSupportedCurrenciesUseCase useCase = new GetSupportedCurrenciesUseCase(providerMock.Object);

            var result = await useCase.ExecuteAsync();

            Assert.Equal(3, result.Count);
            Assert.Contains(result, x => x.Symbol == "USD" && x.Name == "Dólar dos EUA");
            providerMock.Verify(p => p.GetSupportedCurrenciesAsync(), Times.Once);
        }
    }
}
