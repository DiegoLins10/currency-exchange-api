using Exchange.Domain.Entities;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Infrastructure.Repositories
{
    public class ConversionRepository : IConversionRepository
    {
        private readonly ExchangeDbContext _context;

        public ConversionRepository(ExchangeDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(ConversionRecord record)
        {
            await _context.ConversionRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<ConversionRecord> Items, int TotalCount)> GetHistoryAsync(
            string? fromCurrency,
            string? toCurrency,
            DateOnly? startDate,
            DateOnly? endDate,
            int page,
            int pageSize)
        {
            var query = _context.ConversionRecords.AsQueryable();

            if (!string.IsNullOrWhiteSpace(fromCurrency))
            {
                query = query.Where(c => c.FromCurrency == fromCurrency.ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(toCurrency))
            {
                query = query.Where(c => c.ToCurrency == toCurrency.ToUpper());
            }

            if (startDate.HasValue)
            {
                query = query.Where(c => c.QuotationDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.QuotationDate <= endDate.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.ConversionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ConversionRecord?> GetByIdAsync(Guid id)
        {
            return await _context.ConversionRecords.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
