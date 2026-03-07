using Exchange.Application.Common;
using Exchange.Application.Dtos.Requests;
using Exchange.Application.Dtos.Responses;
using Exchange.Application.Interfaces;

namespace Exchange.Application.UseCases.AuthenticateClient
{
    public class AuthenticateClientUseCase : IAuthenticateClientUseCase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticateClientUseCase(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result<AuthResponse>> ExecuteAsync(AuthRequest request)
        {
            try
            {
                var response = await _authenticationService.Authenticate(request);
                return Result<AuthResponse>.Success(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<AuthResponse>.Failure(new ResultError("UNAUTHORIZED", ex.Message));
            }
        }
    }
}
