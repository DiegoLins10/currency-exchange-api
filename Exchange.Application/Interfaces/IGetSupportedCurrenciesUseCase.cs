using Exchange.Application.Common;
using Exchange.Application.Dtos.Responses;

namespace Exchange.Application.Interfaces
{
    public interface IGetSupportedCurrenciesUseCase
    {
        Task<Result<IReadOnlyCollection<SupportedCurrencyResponse>>> ExecuteAsync();
    }
}
