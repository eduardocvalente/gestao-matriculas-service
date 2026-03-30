using MatriculasService.Domain.Interfaces;
using MatriculasService.Domain.ValueObjects;

namespace MatriculasService.Domain.Events;

public sealed class MatriculaRealizadaEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public Guid MatriculaId { get; }
    public Guid AlunoId { get; }
    public Guid DisciplinaId { get; }
    public PeriodoLetivo Periodo { get; }

    public MatriculaRealizadaEvent(Guid matriculaId, Guid alunoId, Guid disciplinaId, PeriodoLetivo periodo)
    {
        MatriculaId = matriculaId;
        AlunoId = alunoId;
        DisciplinaId = disciplinaId;
        Periodo = periodo;
    }
}
