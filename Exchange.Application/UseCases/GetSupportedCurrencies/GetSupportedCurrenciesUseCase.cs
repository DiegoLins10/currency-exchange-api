using Exchange.Application.Interfaces;

namespace Exchange.Application.UseCases.GetSupportedCurrencies
{
    public class GetSupportedCurrenciesUseCase : IGetSupportedCurrenciesUseCase
    {
        private static readonly string[] SupportedCurrencies =
        {
            "USD", "EUR", "GBP", "ARS", "CAD", "AUD", "JPY", "CHF"
        };

        public IReadOnlyCollection<string> Execute()
        {
            return SupportedCurrencies;
        }
    }
}
