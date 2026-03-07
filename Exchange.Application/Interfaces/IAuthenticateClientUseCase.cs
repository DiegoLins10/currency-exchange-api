using Exchange.Application.Common;
using Exchange.Application.Dtos.Requests;
using Exchange.Application.Dtos.Responses;

namespace Exchange.Application.Interfaces
{
    public interface IAuthenticateClientUseCase
    {
        Task<Result<AuthResponse>> ExecuteAsync(AuthRequest request);
    }
}
