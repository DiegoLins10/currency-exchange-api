using Exchange.Application.UseCases.GetSupportedCurrencies;
using Xunit;

namespace Exchange.Unit.Tests.Application.UseCases
{
    public class GetSupportedCurrenciesUseCaseTests
    {
        [Fact]
        public void Execute_ShouldReturnSupportedCurrencies()
        {
            var useCase = new GetSupportedCurrenciesUseCase();

            var result = useCase.Execute();

            Assert.NotNull(result);
            Assert.Contains("USD", result);
            Assert.Contains("EUR", result);
            Assert.Equal(8, result.Count);
        }
    }
}
