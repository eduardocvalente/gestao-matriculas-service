using MatriculasService.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace MatriculasService.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected async Task<IActionResult> ExecuteAsync<T>(
        Func<Task<Result<T>>> action,
        Func<T, IActionResult> onSuccess)
        where T : BaseResponse
    {
        var result = await action();

        if (result.IsFailure)
        {
            var firstError = result.Errors.FirstOrDefault();
            return firstError?.Code switch
            {
                "NotFound" => NotFound(BuildErrorResponse(result.Errors)),
                "Conflict" => Conflict(BuildErrorResponse(result.Errors)),
                _ => UnprocessableEntity(BuildErrorResponse(result.Errors))
            };
        }

        return onSuccess(result.Value!);
    }

    protected async Task<IActionResult> ExecuteAsync(
        Func<Task> action,
        IActionResult onSuccess)
    {
        await action();
        return onSuccess;
    }

    protected IActionResult OkResponse<T>(T value) => Ok(value);

    protected IActionResult CreatedResponse<T>(T value) =>
        StatusCode(StatusCodes.Status201Created, value);

    protected IActionResult NoContentResponse() => NoContent();

    private static object BuildErrorResponse(IReadOnlyList<ResultError> errors) =>
        new
        {
            errors = errors.Select(e => new { e.Code, e.Message })
        };
}
