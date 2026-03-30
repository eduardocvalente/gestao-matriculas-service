using MatriculasService.Domain.Primitives;
using MatriculasService.Infrastructure.Persistence.Primitives;

namespace MatriculasService.Infrastructure.Persistence.Mappers.Base;

public interface IEntityMapper<TDomain, TEntity>
    where TDomain : BaseEntity
    where TEntity : BasePersistenceEntity
{
    TEntity ToInfrastructure(TDomain domain);
    TDomain ToDomain(TEntity entity);
}
