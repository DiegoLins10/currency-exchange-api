using Exchange.Application.Common;
using Exchange.Application.Dtos.Responses;

namespace Exchange.Application.Interfaces
{
    public interface IGetExchangeRateUseCase
    {
        Task<Result<ExchangeRateResponse>> ExecuteAsync(string currency, DateOnly dateQuotation);
    }
}
