using MatriculasService.Application.Common;
using MatriculasService.Application.DTOs;
using MatriculasService.Application.Primitives;
using MediatR;

namespace MatriculasService.Application.Queries.ListarMatriculasPorAluno;

public sealed record ListarMatriculasPorAlunoQuery(Guid AlunoId)
    : BaseQuery, IRequest<Result<MatriculaListResponse>>;
