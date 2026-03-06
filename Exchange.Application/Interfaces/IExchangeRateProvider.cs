using Exchange.Domain.Entities;

namespace Exchange.Domain.Interfaces
{
    public interface IExchangeRateProvider
    {
        Task<ExchangeRate> GetExchangeRateAsync(string toCurrency, DateOnly DateQuotation);
        Task<IReadOnlyCollection<SupportedCurrency>> GetSupportedCurrenciesAsync();
    }
}
