using Exchange.Application.Dtos.Responses;
using Exchange.Application.Interfaces;
using Exchange.Domain.Interfaces;

namespace Exchange.Application.UseCases.GetExchangeRate
{
    public class GetExchangeRateUseCase : IGetExchangeRateUseCase
    {
        private const string ProviderName = "BACEN";
        private readonly IExchangeRateProvider _exchangeRateProvider;

        public GetExchangeRateUseCase(IExchangeRateProvider exchangeRateProvider)
        {
            _exchangeRateProvider = exchangeRateProvider;
        }

        public async Task<ExchangeRateResponse> ExecuteAsync(string currency, DateOnly dateQuotation)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("currency é obrigatório.");
            }

            var normalizedCurrency = currency.Trim().ToUpper();
            var rate = await _exchangeRateProvider.GetExchangeRateAsync(normalizedCurrency, dateQuotation);
            var date = DateTime.TryParse(rate.QuotationDate, out var parsed)
                ? DateOnly.FromDateTime(parsed)
                : dateQuotation;

            return new ExchangeRateResponse(rate.Currency, rate.BuyRate, rate.SellRate, date, ProviderName);
        }
    }
}
