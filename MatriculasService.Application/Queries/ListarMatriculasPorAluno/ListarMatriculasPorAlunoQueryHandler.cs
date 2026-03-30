using MatriculasService.Application.Common;
using MatriculasService.Application.DTOs;
using MatriculasService.Application.Primitives;
using MatriculasService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MatriculasService.Application.Queries.ListarMatriculasPorAluno;

public sealed class ListarMatriculasPorAlunoQueryHandler
    : IQueryHandler<ListarMatriculasPorAlunoQuery, MatriculaListResponse>
{
    private readonly IMatriculaRepository _repository;
    private readonly ILogger<ListarMatriculasPorAlunoQueryHandler> _logger;

    public ListarMatriculasPorAlunoQueryHandler(
        IMatriculaRepository repository,
        ILogger<ListarMatriculasPorAlunoQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<MatriculaListResponse>> Handle(
        ListarMatriculasPorAlunoQuery query,
        CancellationToken cancellationToken)
    {
        var correlationId = query.CorrelationId.ToString();

        var matriculas = await _repository.ListarPorAlunoAsync(query.AlunoId, cancellationToken);

        var response = MatriculaListResponse.FromDomain(matriculas, correlationId);

        _logger.LogInformation(
            "Matrículas listadas para aluno. {AlunoId} {Total} {CorrelationId}",
            query.AlunoId, response.Total, correlationId);

        return Result<MatriculaListResponse>.Success(response);
    }
}
