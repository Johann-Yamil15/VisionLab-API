namespace VisionLab.Application.Common;

public enum ErrorType { Validation, NotFound, Conflict, Unauthorized }

public sealed record Error(ErrorType Type, string Message);

/// <summary>
/// Resultado de un caso de uso: éxito con valor o un error de negocio esperado
/// (las excepciones quedan para fallas técnicas).
/// </summary>
public sealed class Result<T>
{
    private Result(T? value, Error? error) { Value = value; Error = error; }

    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess => Error is null;

    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(ErrorType type, string message) => new(default, new Error(type, message));
}
