using Exchange.Application.Dtos.Requests;
using Exchange.Application.Dtos.Responses;
using Exchange.Application.Interfaces;
using Exchange.Domain.Entities;
using Exchange.Domain.Enums;
using Exchange.Domain.Interfaces;

namespace Exchange.Application.UseCases.ConvertCurrency
{
    public class ConvertCurrencyUseCase : IConvertCurrencyUseCase
    {
        private readonly IExchangeRateProvider _rateProvider;
        private readonly IConversionRepository _repository;

        private const string ProviderName = "BACEN";

        public ConvertCurrencyUseCase(IExchangeRateProvider rateProvider, IConversionRepository repository)
        {
            _rateProvider = rateProvider;
            _repository = repository;
        }

        public async Task<ConvertCurrencyResponse> ExecuteAsync(ConvertCurrencyRequest request)
        {
            if (request.AmountBRL <= 0)
                throw new ArgumentException("O valor deve ser maior que zero.");

            ExchangeRate exchangeRate = await _rateProvider.GetExchangeRateAsync(request.ToCurrency, request.DateQuotation);

            decimal exchangeParity = request.ExchangeType == ExchangeQuotationEnum.Buy ? exchangeRate.BuyRate : exchangeRate.SellRate;

            decimal convertedAmount = request.AmountBRL / exchangeParity;

            DateOnly quotationDate = TryResolveQuotationDate(exchangeRate.QuotationDate, request.DateQuotation);

            var record = new ConversionRecord(
                "BRL",
                request.ToCurrency,
                request.AmountBRL,
                convertedAmount,
                exchangeParity,
                request.ExchangeType,
                quotationDate,
                ProviderName
            );

            await _repository.SaveAsync(record);

            return new ConvertCurrencyResponse(
                request.AmountBRL,
                "BRL",
                Math.Round(convertedAmount, 2),
                request.ToCurrency,
                Math.Round(exchangeParity, 2),
                request.ExchangeType,
                quotationDate,
                ProviderName
            );
        }

        private static DateOnly TryResolveQuotationDate(string providerQuotationDate, DateOnly fallbackDate)
        {
            if (DateTime.TryParse(providerQuotationDate, out var parsed))
            {
                return DateOnly.FromDateTime(parsed);
            }

            return fallbackDate;
        }
    }
}
