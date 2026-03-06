using Exchange.Application.Dtos.Requests;
using Exchange.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.API.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticateClientUseCase _authenticateClientUseCase;

        public AuthController(IAuthenticateClientUseCase authUseCase)
        {
            _authenticateClientUseCase = authUseCase;
        }

        [HttpPost("token")]
        public async Task<IActionResult> Authenticate([FromHeader(Name = "client_id")] string clientId, [FromHeader(Name = "secret")] string clientSecret)
        {
            var request = new AuthRequest
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            var response = await _authenticateClientUseCase.ExecuteAsync(request);
            return Ok(response);
        }
    }
}
