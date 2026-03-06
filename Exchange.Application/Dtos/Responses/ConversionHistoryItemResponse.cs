using Exchange.Domain.Enums;

namespace Exchange.Application.Dtos.Responses
{
    public record ConversionHistoryItemResponse(
        Guid Id,
        string FromCurrency,
        string ToCurrency,
        decimal OriginalAmount,
        decimal ConvertedAmount,
        decimal ExchangeRate,
        ExchangeQuotationEnum ExchangeType,
        DateOnly DateQuotation,
        string Provider,
        DateTime ConversionDate);
}
