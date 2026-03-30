using MatriculasService.Application.Common;
using MatriculasService.Application.DTOs;
using MatriculasService.Application.Interfaces;
using MatriculasService.Application.Primitives;
using MatriculasService.Domain.Entities;
using MatriculasService.Domain.Interfaces;
using MatriculasService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MatriculasService.Application.Commands.RealizarMatricula;

public sealed class RealizarMatriculaCommandHandler
    : ICommandHandler<RealizarMatriculaCommand, MatriculaResponse>
{
    private readonly IMatriculaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly ILogger<RealizarMatriculaCommandHandler> _logger;

    public RealizarMatriculaCommandHandler(
        IMatriculaRepository repository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher dispatcher,
        ILogger<RealizarMatriculaCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task<Result<MatriculaResponse>> Handle(
        RealizarMatriculaCommand command,
        CancellationToken cancellationToken)
    {
        var correlationId = command.CorrelationId.ToString();
        var periodo = new PeriodoLetivo(command.PeriodoAno, command.PeriodoSemestre);

        var jaMatriculado = await _repository.ExisteMatriculaAtivaAsync(
            command.AlunoId,
            command.DisciplinaId,
            periodo,
            cancellationToken);

        if (jaMatriculado)
        {
            _logger.LogWarning(
                "Tentativa de matrícula duplicada. {AlunoId} {DisciplinaId} {Periodo} {CorrelationId}",
                command.AlunoId, command.DisciplinaId, periodo, correlationId);

            return Result<MatriculaResponse>.Failure(
                "Conflict",
                "Aluno já está matriculado nesta disciplina neste período letivo.");
        }

        Matricula? matricula = null;
        await _unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            matricula = Matricula.Realizar(command.AlunoId, command.DisciplinaId, periodo);
            await _repository.AdicionarAsync(matricula, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            await _dispatcher.DispatchAsync(matricula.DomainEvents, ct);
        }, cancellationToken);

        _logger.LogInformation(
            "Matrícula realizada com sucesso. {MatriculaId} {AlunoId} {DisciplinaId} {CorrelationId}",
            matricula!.Id, command.AlunoId, command.DisciplinaId, correlationId);

        return Result<MatriculaResponse>.Success(
            MatriculaResponse.FromDomain(matricula!, correlationId));
    }
}
