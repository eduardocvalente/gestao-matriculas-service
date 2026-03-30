using MatriculasService.Domain.Exceptions;
using System.Text.Json;

namespace MatriculasService.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogWarning("Request cancelado pelo cliente. {TraceId}", traceId);
            context.Response.StatusCode = 499;
        }
        catch (DomainException ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogWarning(
                "Regra de domínio violada. {Message} {TraceId}",
                ex.Message, traceId);

            await WriteErrorResponse(context, StatusCodes.Status422UnprocessableEntity,
                "DomainException", ex.Message, traceId);
        }
        catch (BusinessException ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogWarning(
                "Regra de negócio violada. {Message} {TraceId}",
                ex.Message, traceId);

            await WriteErrorResponse(context, StatusCodes.Status409Conflict,
                "BusinessException", ex.Message, traceId);
        }
        catch (NotFoundException ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogWarning(
                "Recurso não encontrado. {Message} {TraceId}",
                ex.Message, traceId);

            await WriteErrorResponse(context, StatusCodes.Status404NotFound,
                "NotFoundException", ex.Message, traceId);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogError(
                "Erro inesperado. {TraceId} {ExceptionType}",
                traceId, ex.GetType().Name);

            await WriteErrorResponse(context, StatusCodes.Status500InternalServerError,
                "InternalServerError", "Ocorreu um erro interno. Tente novamente mais tarde.", traceId);
        }
    }

    private static async Task WriteErrorResponse(
        HttpContext context,
        int statusCode,
        string type,
        string message,
        string traceId)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            type,
            message,
            traceId
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
