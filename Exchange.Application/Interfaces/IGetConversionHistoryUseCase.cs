using Exchange.Application.Common;
using Exchange.Application.Dtos.Requests;
using Exchange.Application.Dtos.Responses;

namespace Exchange.Application.Interfaces
{
    public interface IGetConversionHistoryUseCase
    {
        Task<Result<PaginatedConversionHistoryResponse>> ExecuteAsync(GetConversionHistoryRequest request);
        Task<Result<ConversionHistoryItemResponse>> GetByIdAsync(Guid id);
    }
}
