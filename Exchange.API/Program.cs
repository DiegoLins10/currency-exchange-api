using Exchange.API.Middleware;
using Exchange.Application.Interfaces;
using Exchange.Application.UseCases.AuthenticateClient;
using Exchange.Application.UseCases.ConvertCurrency;
using Exchange.Application.UseCases.GetConversionHistory;
using Exchange.Application.UseCases.GetExchangeRate;
using Exchange.Application.UseCases.GetSupportedCurrencies;
using Exchange.Domain.Entities;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure.Persistences;
using Exchange.Infrastructure.Repositories;
using Exchange.Infrastructure.Services.Authentication;
using Exchange.Infrastructure.Services.Bacen;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ExchangeDbContext>(options =>
    options.UseInMemoryDatabase("ExchangeDb"));

builder.Services.AddScoped<IExchangeRateProvider, ExchangeRateProvider>();
builder.Services.AddScoped<IConvertCurrencyUseCase, ConvertCurrencyUseCase>();
builder.Services.AddScoped<IGetConversionHistoryUseCase, GetConversionHistoryUseCase>();
builder.Services.AddScoped<IGetExchangeRateUseCase, GetExchangeRateUseCase>();
builder.Services.AddScoped<IGetSupportedCurrenciesUseCase, GetSupportedCurrenciesUseCase>();
builder.Services.AddScoped<IAuthenticateClientUseCase, AuthenticateClientUseCase>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IConversionRepository, ConversionRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddHttpClient<IExchangeRateProvider, ExchangeRateProvider>();

builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var scopedServices = scope.ServiceProvider;
    var repository = scopedServices.GetRequiredService<IClientRepository>();

    await repository.AddClientAsync(new Client
    {
        ClientId = "3f29b6e7-1c4b-4f9a-b8b4-2f5e2f4d5c6a",
        Secret = "f8d9a7b6-2c3e-4f7a-8b1d-3e2f4a5b6c7d"
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
