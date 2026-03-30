using MatriculasService.Infrastructure.Persistence.Primitives;

namespace MatriculasService.Infrastructure.Persistence.Entities;

public sealed class MatriculaEntity : BasePersistenceEntity
{
    public Guid AlunoId { get; set; }
    public Guid DisciplinaId { get; set; }
    public int PeriodoAno { get; set; }
    public int PeriodoSemestre { get; set; }
    public string Status { get; set; } = string.Empty;
}
