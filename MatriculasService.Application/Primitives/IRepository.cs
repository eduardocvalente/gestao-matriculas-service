using MatriculasService.Domain.Primitives;

namespace MatriculasService.Application.Primitives;

// Alternativa aceita: parâmetro único TDomain com constraint BaseEntity.
// A constraint TEntity : BasePersistenceEntity é enforçada em compile-time
// via IEntityMapper<TDomain, TEntity> na camada de Infrastructure.
// IMatriculaRepository (Domain) não herda desta interface para preservar a
// direção correta de dependências (Domain não pode depender de Application).
public interface IRepository<TDomain>
    where TDomain : BaseEntity
{
    Task<TDomain?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task AdicionarAsync(TDomain domain, CancellationToken cancellationToken);
    Task AtualizarAsync(TDomain domain, CancellationToken cancellationToken);
}
