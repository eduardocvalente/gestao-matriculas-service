using MatriculasService.Application.Common;
using MatriculasService.Domain.Entities;

namespace MatriculasService.Application.DTOs;

public sealed record MatriculaResponse : BaseResponse
{
    public Guid Id { get; init; }
    public Guid AlunoId { get; init; }
    public Guid DisciplinaId { get; init; }
    public int PeriodoAno { get; init; }
    public int PeriodoSemestre { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CriadoEm { get; init; }
    public DateTime? AtualizadoEm { get; init; }

    public static MatriculaResponse FromDomain(Matricula matricula, string traceId = "") =>
        new()
        {
            Id = matricula.Id,
            AlunoId = matricula.AlunoId,
            DisciplinaId = matricula.DisciplinaId,
            PeriodoAno = matricula.Periodo.Ano,
            PeriodoSemestre = matricula.Periodo.Semestre,
            Status = matricula.Status.ToString(),
            CriadoEm = matricula.CreatedAt,
            AtualizadoEm = matricula.UpdatedAt,
            TraceId = traceId
        };
}

public sealed record MatriculaListResponse : BaseResponse
{
    public IReadOnlyList<MatriculaResponse> Items { get; init; } = Array.Empty<MatriculaResponse>();
    public int Total => Items.Count;

    public static MatriculaListResponse FromDomain(
        IReadOnlyList<Matricula> matriculas,
        string traceId = "") =>
        new()
        {
            Items = matriculas
                .Select(m => MatriculaResponse.FromDomain(m))
                .ToList()
                .AsReadOnly(),
            TraceId = traceId
        };
}
