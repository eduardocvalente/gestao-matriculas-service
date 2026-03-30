namespace MatriculasService.Application.Primitives;

public abstract record BaseCommand
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
