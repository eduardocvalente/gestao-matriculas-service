using MatriculasService.Application.Common;
using MatriculasService.Application.DTOs;
using MatriculasService.Application.Primitives;
using MediatR;

namespace MatriculasService.Application.Commands.CancelarMatricula;

public sealed record CancelarMatriculaCommand(Guid Id) : BaseCommand, IRequest<Result<MatriculaResponse>>;
