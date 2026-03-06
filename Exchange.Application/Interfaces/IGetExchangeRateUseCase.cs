using Exchange.Application.Dtos.Responses;

namespace Exchange.Application.Interfaces
{
    public interface IGetExchangeRateUseCase
    {
        Task<ExchangeRateResponse> ExecuteAsync(string currency, DateOnly dateQuotation);
    }
}
