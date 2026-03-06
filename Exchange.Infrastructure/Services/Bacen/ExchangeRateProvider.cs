using Exchange.Domain.Entities;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure.Services.Bacen.Responses;
using System.Net.Http.Json;

namespace Exchange.Infrastructure.Services.Bacen
{
    public class ExchangeRateProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ExchangeRate> GetExchangeRateAsync(string currency, DateOnly date)
        {
            var url = $"https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/odata/" +
                      $"CotacaoMoedaDia(moeda=@moeda,dataCotacao=@dataCotacao)" +
                      $"?@moeda='{currency}'&@dataCotacao='{date:MM-dd-yyyy}'&$top=1&$format=json";

            var response = await _httpClient.GetFromJsonAsync<CurrencyBacenResponse>(url);

            CurrencyItemBacen item = response?.Value?.FirstOrDefault()
                ?? throw new InvalidOperationException("No exchange rate found.");

            return new ExchangeRate(
                currency,
                item.CotacaoCompra,
                item.CotacaoVenda,
                item.DataHoraCotacao
            );
        }

        public async Task<IReadOnlyCollection<string>> GetSupportedCurrenciesAsync()
        {
            const string url = "https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/odata/Moedas?$top=100&$format=json";

            var response = await _httpClient.GetFromJsonAsync<CurrencyBacenResponse>(url);
            var symbols = response?.Value?
                .Where(x => !string.IsNullOrWhiteSpace(x.Simbolo))
                .Select(x => x.Simbolo.Trim().ToUpper())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (symbols is null || symbols.Count == 0)
            {
                throw new InvalidOperationException("No supported currencies found.");
            }

            return symbols;
        }
    }
}
