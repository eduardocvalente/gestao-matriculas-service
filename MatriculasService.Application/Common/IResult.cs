namespace MatriculasService.Application.Common;

public interface IResult<T> where T : BaseResponse
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    T? Value { get; }
    IReadOnlyList<ResultError> Errors { get; }
}
