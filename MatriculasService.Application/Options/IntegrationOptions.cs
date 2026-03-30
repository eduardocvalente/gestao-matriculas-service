namespace MatriculasService.Application.Options;

public sealed class IntegrationOptions
{
    public const string SectionName = "Integration";

    public List<string> AllowedClients { get; init; } = new();
}
