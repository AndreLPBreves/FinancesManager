using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Common
{
    public enum StatusCode
    {
        OK,
        EmailAlredyRegistered,
        PasswordConfirmationMismatch,
        InvalidCredentials,
        InvalidToken,
        ExpiredToken,
        InvalidUser,
        Unknown,
    }

    public class Result<T>(StatusCode statusCode, T? value)
    {
        public T? Value { get; init; } = value;
        public StatusCode StatusCode { get; init; } = statusCode;
    }
}
