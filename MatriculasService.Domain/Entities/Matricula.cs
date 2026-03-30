using MatriculasService.Domain.Enums;
using MatriculasService.Domain.Events;
using MatriculasService.Domain.Exceptions;
using MatriculasService.Domain.Primitives;
using MatriculasService.Domain.ValueObjects;

namespace MatriculasService.Domain.Entities;

public sealed class Matricula : BaseEntity
{
    public Guid AlunoId { get; private set; }
    public Guid DisciplinaId { get; private set; }
    public PeriodoLetivo Periodo { get; private set; } = null!;
    public StatusMatricula Status { get; private set; }

    private Matricula() { }

    public static Matricula Realizar(Guid alunoId, Guid disciplinaId, PeriodoLetivo periodo)
    {
        var matricula = new Matricula
        {
            Id = Guid.NewGuid(),
            AlunoId = alunoId,
            DisciplinaId = disciplinaId,
            Periodo = periodo,
            Status = StatusMatricula.Confirmada,
            CreatedAt = DateTime.UtcNow
        };

        matricula.RaiseDomainEvent(new MatriculaRealizadaEvent(
            matricula.Id,
            alunoId,
            disciplinaId,
            periodo));

        return matricula;
    }

    public static Matricula Reconstituir(
        Guid id,
        Guid alunoId,
        Guid disciplinaId,
        PeriodoLetivo periodo,
        StatusMatricula status,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new Matricula
        {
            Id = id,
            AlunoId = alunoId,
            DisciplinaId = disciplinaId,
            Periodo = periodo,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void Cancelar()
    {
        if (Status == StatusMatricula.Cancelada)
            throw new DomainException("Matrícula já está cancelada e não pode ser cancelada novamente.");

        Status = StatusMatricula.Cancelada;
        SetUpdatedAt();

        RaiseDomainEvent(new MatriculaCanceladaEvent(Id, AlunoId, DisciplinaId, Periodo));
    }
}
