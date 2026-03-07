using Exchange.Application.Common;
using Exchange.Application.Dtos.Requests;
using Exchange.Application.Dtos.Responses;

namespace Exchange.Application.Interfaces
{
    public interface IConvertCurrencyUseCase
    {
        Task<Result<ConvertCurrencyResponse>> ExecuteAsync(ConvertCurrencyRequest request);
    }
}
