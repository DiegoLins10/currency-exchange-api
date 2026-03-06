namespace Exchange.Application.Dtos.Responses
{
    public record ExchangeRateResponse(
        string Currency,
        decimal BuyRate,
        decimal SellRate,
        DateOnly DateQuotation,
        string Provider);
}
