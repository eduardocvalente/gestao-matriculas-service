using MatriculasService.Domain.Entities;
using MatriculasService.Domain.ValueObjects;

namespace MatriculasService.Domain.Interfaces;

// IMatriculaRepository não herda de IRepository<T> (Application/Primitives) para preservar
// a direção correta de dependências: Domain não pode depender de Application.
// O compile-time enforcement de BaseEntity está garantido pelo próprio tipo Matricula,
// e de BasePersistenceEntity via IEntityMapper<Matricula, MatriculaEntity> na Infrastructure.
public interface IMatriculaRepository
{
    Task<Matricula?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExisteMatriculaAtivaAsync(
        Guid alunoId,
        Guid disciplinaId,
        PeriodoLetivo periodo,
        CancellationToken cancellationToken);

    Task AdicionarAsync(Matricula matricula, CancellationToken cancellationToken);

    Task AtualizarAsync(Matricula matricula, CancellationToken cancellationToken);

    Task<IReadOnlyList<Matricula>> ListarPorAlunoAsync(Guid alunoId, CancellationToken cancellationToken);
}
