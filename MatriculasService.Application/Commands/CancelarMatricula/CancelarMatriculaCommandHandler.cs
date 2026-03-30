using MatriculasService.Application.Common;
using MatriculasService.Application.DTOs;
using MatriculasService.Application.Interfaces;
using MatriculasService.Application.Primitives;
using MatriculasService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MatriculasService.Application.Commands.CancelarMatricula;

public sealed class CancelarMatriculaCommandHandler
    : ICommandHandler<CancelarMatriculaCommand, MatriculaResponse>
{
    private readonly IMatriculaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly ILogger<CancelarMatriculaCommandHandler> _logger;

    public CancelarMatriculaCommandHandler(
        IMatriculaRepository repository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher dispatcher,
        ILogger<CancelarMatriculaCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task<Result<MatriculaResponse>> Handle(
        CancelarMatriculaCommand command,
        CancellationToken cancellationToken)
    {
        var correlationId = command.CorrelationId.ToString();

        var matricula = await _repository.ObterPorIdAsync(command.Id, cancellationToken);

        if (matricula is null)
        {
            _logger.LogWarning(
                "Matrícula não encontrada para cancelamento. {MatriculaId} {CorrelationId}",
                command.Id, correlationId);

            return Result<MatriculaResponse>.Failure(
                "NotFound",
                $"Matrícula com ID '{command.Id}' não foi encontrada.");
        }

        await _unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            matricula.Cancelar();
            await _repository.AtualizarAsync(matricula, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            await _dispatcher.DispatchAsync(matricula.DomainEvents, ct);
        }, cancellationToken);

        _logger.LogInformation(
            "Matrícula cancelada com sucesso. {MatriculaId} {AlunoId} {CorrelationId}",
            matricula.Id, matricula.AlunoId, correlationId);

        return Result<MatriculaResponse>.Success(
            MatriculaResponse.FromDomain(matricula, correlationId));
    }
}
