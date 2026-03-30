namespace MatriculasService.Application.Common;

public abstract record BaseResponse
{
    public string TraceId { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
