using Exchange.Application.Dtos.Requests;
using Exchange.Application.Dtos.Responses;

namespace Exchange.Application.Interfaces
{
    public interface IGetConversionHistoryUseCase
    {
        Task<PaginatedConversionHistoryResponse> ExecuteAsync(GetConversionHistoryRequest request);
        Task<ConversionHistoryItemResponse?> GetByIdAsync(Guid id);
    }
}
