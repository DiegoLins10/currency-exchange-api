using Exchange.Domain.Entities;

namespace Exchange.Domain.Interfaces
{
    public interface IConversionRepository
    {
        Task SaveAsync(ConversionRecord record);
        Task<(IEnumerable<ConversionRecord> Items, int TotalCount)> GetHistoryAsync(
            string? fromCurrency,
            string? toCurrency,
            DateOnly? startDate,
            DateOnly? endDate,
            int page,
            int pageSize);
        Task<ConversionRecord?> GetByIdAsync(Guid id);
    }
}
