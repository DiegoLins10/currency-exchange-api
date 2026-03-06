namespace Exchange.Application.Interfaces
{
    public interface IGetSupportedCurrenciesUseCase
    {
        IReadOnlyCollection<string> Execute();
    }
}
