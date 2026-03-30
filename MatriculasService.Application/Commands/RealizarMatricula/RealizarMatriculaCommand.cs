using MatriculasService.Application.Common;
using MatriculasService.Application.DTOs;
using MatriculasService.Application.Primitives;
using MediatR;

namespace MatriculasService.Application.Commands.RealizarMatricula;

public sealed record RealizarMatriculaCommand(
    Guid AlunoId,
    Guid DisciplinaId,
    int PeriodoAno,
    int PeriodoSemestre) : BaseCommand, IRequest<Result<MatriculaResponse>>;
