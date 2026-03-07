using Exchange.Application.Common;
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

        public async Task<Result<IReadOnlyCollection<SupportedCurrencyResponse>>> ExecuteAsync()
        {
            var currencies = await _exchangeRateProvider.GetSupportedCurrenciesAsync();

            var response = currencies
                .Select(c => new SupportedCurrencyResponse(c.Symbol, c.Name))
                .ToList();

            return Result<IReadOnlyCollection<SupportedCurrencyResponse>>.Success(response);
        }
    }
}
