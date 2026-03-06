using Exchange.Application.Dtos.Responses;

namespace Exchange.Application.Interfaces
{
    public interface IGetSupportedCurrenciesUseCase
    {
        Task<IReadOnlyCollection<SupportedCurrencyResponse>> ExecuteAsync();
    }
}
