namespace MatriculasService.Application.Primitives;

public abstract record BaseQuery
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
