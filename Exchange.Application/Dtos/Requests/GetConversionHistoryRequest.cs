namespace Exchange.Application.Dtos.Requests
{
    public record GetConversionHistoryRequest(
        string? FromCurrency,
        string? ToCurrency,
        DateOnly? StartDate,
        DateOnly? EndDate,
        int Page = 1,
        int PageSize = 20);
}
