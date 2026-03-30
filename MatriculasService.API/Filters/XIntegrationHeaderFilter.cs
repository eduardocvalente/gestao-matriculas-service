using MatriculasService.Application.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace MatriculasService.API.Filters;

public sealed class XIntegrationHeaderFilter : IAsyncActionFilter
{
    private const string HeaderName = "X-Integration";

    private readonly IntegrationOptions _options;
    private readonly ILogger<XIntegrationHeaderFilter> _logger;

    public XIntegrationHeaderFilter(
        IOptions<IntegrationOptions> options,
        ILogger<XIntegrationHeaderFilter> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var headerValue)
            || string.IsNullOrWhiteSpace(headerValue))
        {
            _logger.LogWarning(
                "Header {HeaderName} ausente na requisição. {Path}",
                HeaderName, context.HttpContext.Request.Path);

            context.Result = new BadRequestObjectResult(new
            {
                type = "IntegrationHeaderMissing",
                message = "Header X-Integration ausente ou não autorizado.",
                traceId = context.HttpContext.TraceIdentifier
            });
            return;
        }

        var integrationSource = headerValue.ToString();

        if (!_options.AllowedClients.Contains(integrationSource, StringComparer.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Header {HeaderName} com valor não autorizado. {IntegrationSource} {Path}",
                HeaderName, integrationSource, context.HttpContext.Request.Path);

            context.Result = new BadRequestObjectResult(new
            {
                type = "IntegrationHeaderMissing",
                message = "Header X-Integration ausente ou não autorizado.",
                traceId = context.HttpContext.TraceIdentifier
            });
            return;
        }

        _logger.LogInformation(
            "Requisição autorizada via integração. {IntegrationSource} {Path}",
            integrationSource, context.HttpContext.Request.Path);

        await next();
    }
}
