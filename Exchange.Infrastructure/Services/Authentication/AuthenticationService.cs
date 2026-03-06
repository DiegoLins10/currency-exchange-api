using Exchange.Application.Dtos.Requests;
using Exchange.Application.Dtos.Responses;
using Exchange.Application.Interfaces;
using Exchange.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Exchange.Infrastructure.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IClientRepository _clientRepository;

        public AuthenticationService(IConfiguration configuration, IClientRepository clientRepository)
        {
            _configuration = configuration;
            _clientRepository = clientRepository;
        }

        public async Task<AuthResponse> Authenticate(AuthRequest request)
        {
            var client = await _clientRepository.GetClientByIdAsync(request.ClientId, request.ClientSecret);

            if (client == null)
            {
                throw new UnauthorizedAccessException("Credenciais inválidas.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.ClientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new AuthResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiration
            };
        }
    }
}
