using Exchange.Application.Dtos.Requests;
using Exchange.Application.Dtos.Responses;
using Exchange.Application.Interfaces;
using Exchange.Application.UseCases.AuthenticateClient;
using Moq;
using Xunit;

namespace Exchange.Unit.Tests.Application.UseCases
{
    public class AuthenticateClientUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldDelegateToAuthenticationService()
        {
            var serviceMock = new Mock<IAuthenticationService>();
            var useCase = new AuthenticateClientUseCase(serviceMock.Object);
            var request = new AuthRequest { ClientId = "client", ClientSecret = "secret" };
            var expected = new AuthResponse { AccessToken = "token", ExpiresAt = DateTime.UtcNow.AddHours(1) };

            serviceMock.Setup(s => s.Authenticate(request))
                .ReturnsAsync(expected);

            var result = await useCase.ExecuteAsync(request);

            Assert.Equal(expected.AccessToken, result.AccessToken);
            Assert.Equal(expected.ExpiresAt, result.ExpiresAt);
            serviceMock.Verify(s => s.Authenticate(request), Times.Once);
        }
    }
}
