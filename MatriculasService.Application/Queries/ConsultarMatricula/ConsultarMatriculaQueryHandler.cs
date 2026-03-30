using MatriculasService.Application.Common;
using MatriculasService.Application.DTOs;
using MatriculasService.Application.Primitives;
using MatriculasService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MatriculasService.Application.Queries.ConsultarMatricula;

public sealed class ConsultarMatriculaQueryHandler
    : IQueryHandler<ConsultarMatriculaQuery, MatriculaResponse>
{
    private readonly IMatriculaRepository _repository;
    private readonly ILogger<ConsultarMatriculaQueryHandler> _logger;

    public ConsultarMatriculaQueryHandler(
        IMatriculaRepository repository,
        ILogger<ConsultarMatriculaQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<MatriculaResponse>> Handle(
        ConsultarMatriculaQuery query,
        CancellationToken cancellationToken)
    {
        var correlationId = query.CorrelationId.ToString();

        var matricula = await _repository.ObterPorIdAsync(query.Id, cancellationToken);

        if (matricula is null)
        {
            _logger.LogWarning(
                "Matrícula não encontrada. {MatriculaId} {CorrelationId}",
                query.Id, correlationId);

            return Result<MatriculaResponse>.Failure(
                "NotFound",
                $"Matrícula com ID '{query.Id}' não foi encontrada.");
        }

        _logger.LogInformation(
            "Matrícula consultada. {MatriculaId} {AlunoId} {CorrelationId}",
            matricula.Id, matricula.AlunoId, correlationId);

        return Result<MatriculaResponse>.Success(
            MatriculaResponse.FromDomain(matricula, correlationId));
    }
}
