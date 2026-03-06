using Exchange.Application.Interfaces;
using Exchange.Domain.Interfaces;

namespace Exchange.Application.UseCases.GetSupportedCurrencies
{
    public class GetSupportedCurrenciesUseCase : IGetSupportedCurrenciesUseCase
    {
        private readonly IExchangeRateProvider _exchangeRateProvider;

        public GetSupportedCurrenciesUseCase(IExchangeRateProvider exchangeRateProvider)
        {
            _exchangeRateProvider = exchangeRateProvider;
        }

        public Task<IReadOnlyCollection<string>> ExecuteAsync()
        {
            return _exchangeRateProvider.GetSupportedCurrenciesAsync();
        }
    }
}
