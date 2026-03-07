using Exchange.API.Models;
using Exchange.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result, object? meta = null)
        {
            if (result.IsSuccess)
            {
                return controller.Ok(new ApiResponse<T>
                {
                    Success = true,
                    Data = result.Value,
                    Error = null,
                    Meta = meta
                });
            }

            var statusCode = result.Error?.Code switch
            {
                "NOT_FOUND" => StatusCodes.Status404NotFound,
                "UNAUTHORIZED" => StatusCodes.Status401Unauthorized,
                "FORBIDDEN" => StatusCodes.Status403Forbidden,
                "CONFLICT" => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };

            return controller.StatusCode(statusCode, new ApiResponse<object>
            {
                Success = false,
                Data = null,
                Error = new ApiError(
                    result.Error?.Code ?? "BAD_REQUEST",
                    result.Error?.Message ?? "Não foi possível processar a requisição.",
                    result.Error?.Details),
                Meta = meta
            });
        }
    }
}
