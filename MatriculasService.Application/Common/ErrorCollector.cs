using MatriculasService.Domain.Exceptions;

namespace MatriculasService.Application.Common;

public sealed class ErrorCollector
{
    private readonly List<ResultError> _errors = new();

    public bool HasErrors => _errors.Count > 0;
    public IReadOnlyList<ResultError> Errors => _errors;

    public void Add(string code, string message) =>
        _errors.Add(new ResultError(code, message));

    public void AddRange(IEnumerable<ResultError> errors) =>
        _errors.AddRange(errors);

    public Result<T> ToFailureResult<T>() where T : BaseResponse =>
        Result<T>.Failure(_errors);

    public void ThrowIfHasErrors()
    {
        if (HasErrors)
            throw new DomainException(
                string.Join(" | ", _errors.Select(e => $"[{e.Code}] {e.Message}")));
    }
}
