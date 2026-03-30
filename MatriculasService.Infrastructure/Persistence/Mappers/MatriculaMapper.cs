using MatriculasService.Domain.Entities;
using MatriculasService.Domain.Enums;
using MatriculasService.Domain.ValueObjects;
using MatriculasService.Infrastructure.Persistence.Entities;
using MatriculasService.Infrastructure.Persistence.Mappers.Base;

namespace MatriculasService.Infrastructure.Persistence.Mappers;

// Compile-time enforcement:
//   Matricula     : BaseEntity           (via IEntityMapper<TDomain, TEntity> where TDomain : BaseEntity)
//   MatriculaEntity : BasePersistenceEntity (via IEntityMapper<TDomain, TEntity> where TEntity : BasePersistenceEntity)
internal sealed class MatriculaMapper : IEntityMapper<Matricula, MatriculaEntity>
{
    public MatriculaEntity ToInfrastructure(Matricula domain) =>
        new()
        {
            Id = domain.Id,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
            AlunoId = domain.AlunoId,
            DisciplinaId = domain.DisciplinaId,
            PeriodoAno = domain.Periodo.Ano,
            PeriodoSemestre = domain.Periodo.Semestre,
            Status = domain.Status.ToString()
        };

    public Matricula ToDomain(MatriculaEntity entity) =>
        Matricula.Reconstituir(
            entity.Id,
            entity.AlunoId,
            entity.DisciplinaId,
            new PeriodoLetivo(entity.PeriodoAno, entity.PeriodoSemestre),
            Enum.Parse<StatusMatricula>(entity.Status),
            entity.CreatedAt,
            entity.UpdatedAt);
}
