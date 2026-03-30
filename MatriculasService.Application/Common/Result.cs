namespace MatriculasService.Application.Common;

public sealed class Result<T> : IResult<T> where T : BaseResponse
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public IReadOnlyList<ResultError> Errors { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Errors = Array.Empty<ResultError>();
    }

    private Result(IReadOnlyList<ResultError> errors)
    {
        IsSuccess = false;
        Value = default;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(string code, string message) =>
        new(new[] { new ResultError(code, message) });

    public static Result<T> Failure(ResultError error) =>
        new(new[] { error });

    public static Result<T> Failure(IReadOnlyList<ResultError> errors) =>
        new(errors);
}
