namespace Exchange.Application.Common
{
    public sealed record ResultError(string Code, string Message, object? Details = null);
}
