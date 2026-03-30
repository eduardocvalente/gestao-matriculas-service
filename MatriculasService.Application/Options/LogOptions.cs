namespace MatriculasService.Application.Options;

public sealed class LogOptions
{
    public const string SectionName = "Log";

    public string MinimumLevel { get; init; } = "Information";
    public bool EnableDetailedErrors { get; init; } = false;
}
