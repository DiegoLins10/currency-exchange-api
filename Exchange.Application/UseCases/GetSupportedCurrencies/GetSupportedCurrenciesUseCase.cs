using Exchange.Application.Dtos.Responses;
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

        public async Task<IReadOnlyCollection<SupportedCurrencyResponse>> ExecuteAsync()
        {
            var currencies = await _exchangeRateProvider.GetSupportedCurrenciesAsync();

            return currencies
                .Select(c => new SupportedCurrencyResponse(c.Symbol, c.Name))
                .ToList();
        }
    }
}
