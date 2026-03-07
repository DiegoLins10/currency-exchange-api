using Exchange.API.Models;
using System.Text.Json;

namespace Exchange.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ArgumentException capturada");
                await WriteErrorAsync(httpContext, StatusCodes.Status400BadRequest, "VALIDATION_ERROR", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "InvalidOperationException capturada");
                await WriteErrorAsync(httpContext, StatusCodes.Status400BadRequest, "INVALID_OPERATION", ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "UnauthorizedAccessException capturada");
                await WriteErrorAsync(httpContext, StatusCodes.Status401Unauthorized, "UNAUTHORIZED", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado");
                await WriteErrorAsync(httpContext, StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "Ocorreu um erro inesperado.");
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, int statusCode, string code, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new ApiResponse<object>
            {
                Success = false,
                Data = null,
                Error = new ApiError(code, message),
                Meta = null
            });

            await context.Response.WriteAsync(result);
        }
    }
}
