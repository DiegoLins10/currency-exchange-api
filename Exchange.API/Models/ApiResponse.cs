namespace Exchange.API.Models
{
    public sealed record ApiError(string Code, string Message, object? Details = null);

    public sealed class ApiResponse<T>
    {
        public required bool Success { get; init; }
        public T? Data { get; init; }
        public ApiError? Error { get; init; }
        public object? Meta { get; init; }
    }
}
