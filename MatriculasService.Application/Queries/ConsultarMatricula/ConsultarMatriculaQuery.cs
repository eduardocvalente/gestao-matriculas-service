using MatriculasService.Application.Common;
using MatriculasService.Application.DTOs;
using MatriculasService.Application.Primitives;
using MediatR;

namespace MatriculasService.Application.Queries.ConsultarMatricula;

public sealed record ConsultarMatriculaQuery(Guid Id) : BaseQuery, IRequest<Result<MatriculaResponse>>;
