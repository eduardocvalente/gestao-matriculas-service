namespace MatriculasService.Infrastructure.Persistence.Primitives;

public abstract class BasePersistenceEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
