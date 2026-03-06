using Exchange.Application.Dtos.Requests;
using Exchange.Application.Dtos.Responses;
using Exchange.Application.Interfaces;
using Exchange.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Exchange.Application.UseCases.GetConversionHistory
{
    public class GetConversionHistoryUseCase : IGetConversionHistoryUseCase
    {
        private readonly IConversionRepository _repository;
        private readonly IMemoryCache _memoryCache;

        public GetConversionHistoryUseCase(IConversionRepository repository, IMemoryCache memoryCache)
        {
            _repository = repository;
            _memoryCache = memoryCache;
        }

        public async Task<PaginatedConversionHistoryResponse> ExecuteAsync(GetConversionHistoryRequest request)
        {
            if (request.Page <= 0)
            {
                throw new ArgumentException("Page deve ser maior que zero.");
            }

            if (request.PageSize <= 0 || request.PageSize > 100)
            {
                throw new ArgumentException("PageSize deve estar entre 1 e 100.");
            }

            var normalizedFrom = NormalizeCurrency(request.FromCurrency);
            var normalizedTo = NormalizeCurrency(request.ToCurrency);

            var cacheKey = BuildCacheKey(normalizedFrom, normalizedTo, request.StartDate, request.EndDate, request.Page, request.PageSize);

            if (_memoryCache.TryGetValue(cacheKey, out PaginatedConversionHistoryResponse? cachedHistory) && cachedHistory is not null)
            {
                return cachedHistory;
            }

            var (items, totalCount) = await _repository.GetHistoryAsync(
                normalizedFrom,
                normalizedTo,
                request.StartDate,
                request.EndDate,
                request.Page,
                request.PageSize);

            var response = new PaginatedConversionHistoryResponse(
                items.Select(MapToResponse),
                totalCount,
                request.Page,
                request.PageSize);

            _memoryCache.Set(cacheKey, response, TimeSpan.FromSeconds(60));

            return response;
        }

        public async Task<ConversionHistoryItemResponse?> GetByIdAsync(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item is null ? null : MapToResponse(item);
        }

        private static string NormalizeCurrency(string? currency)
        {
            return string.IsNullOrWhiteSpace(currency) ? string.Empty : currency.Trim().ToUpper();
        }

        private static string BuildCacheKey(string fromCurrency, string toCurrency, DateOnly? startDate, DateOnly? endDate, int page, int pageSize)
        {
            return $"conversion_history:{fromCurrency}:{toCurrency}:{startDate}:{endDate}:{page}:{pageSize}";
        }

        private static ConversionHistoryItemResponse MapToResponse(Domain.Entities.ConversionRecord item)
        {
            return new ConversionHistoryItemResponse(
                item.Id,
                item.FromCurrency,
                item.ToCurrency,
                item.OriginalAmount,
                item.ConvertedAmount,
                item.ExchangeRate,
                item.ExchangeType,
                item.QuotationDate,
                item.Provider,
                item.ConversionDate);
        }
    }
}
