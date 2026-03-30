namespace MatriculasService.Domain.Interfaces;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
