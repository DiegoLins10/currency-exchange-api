namespace Exchange.Application.Interfaces
{
    public interface IGetSupportedCurrenciesUseCase
    {
        Task<IReadOnlyCollection<string>> ExecuteAsync();
    }
}
