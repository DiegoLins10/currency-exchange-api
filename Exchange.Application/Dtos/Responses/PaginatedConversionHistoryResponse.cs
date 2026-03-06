namespace Exchange.Application.Dtos.Responses
{
    public record PaginatedConversionHistoryResponse(
        IEnumerable<ConversionHistoryItemResponse> Items,
        int TotalCount,
        int Page,
        int PageSize);
}
