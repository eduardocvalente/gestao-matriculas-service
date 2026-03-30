using MatriculasService.API.Controllers.Base;
using MatriculasService.Application.Commands.CancelarMatricula;
using MatriculasService.Application.Commands.RealizarMatricula;
using MatriculasService.Application.DTOs;
using MatriculasService.Application.Queries.ConsultarMatricula;
using MatriculasService.Application.Queries.ListarMatriculasPorAluno;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MatriculasService.API.Controllers;

/// <summary>
/// Gerencia o ciclo de vida das matrículas acadêmicas.
/// </summary>
[Route("api/matriculas")]
public sealed class MatriculasController : BaseApiController
{
    private readonly IMediator _mediator;

    public MatriculasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Realiza uma nova matrícula.
    /// </summary>
    /// <remarks>
    /// Cria uma matrícula para o aluno na disciplina indicada para o período letivo informado.
    /// Retorna conflito (409) se já existir uma matrícula ativa do mesmo aluno na mesma disciplina e período.
    /// </remarks>
    /// <param name="request">Dados da matrícula a ser realizada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A matrícula criada com status <c>Ativa</c>.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MatriculaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RealizarMatricula(
        [FromBody] RealizarMatriculaRequest request,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(
            () => _mediator.Send(request.ToCommand(), cancellationToken),
            result => CreatedResponse(result));
    }

    /// <summary>
    /// Cancela uma matrícula existente.
    /// </summary>
    /// <remarks>
    /// Altera o status da matrícula para <c>Cancelada</c>. Não é possível cancelar uma matrícula
    /// que já esteja cancelada.
    /// </remarks>
    /// <param name="id">Identificador único da matrícula.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A matrícula com status atualizado para <c>Cancelada</c>.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(MatriculaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CancelarMatricula(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(
            () => _mediator.Send(new CancelarMatriculaCommand(id), cancellationToken),
            result => OkResponse(result));
    }

    /// <summary>
    /// Consulta uma matrícula pelo seu identificador.
    /// </summary>
    /// <remarks>
    /// Retorna os dados completos da matrícula, incluindo aluno, disciplina, período e status atual.
    /// </remarks>
    /// <param name="id">Identificador único da matrícula.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Os dados da matrícula encontrada.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MatriculaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarMatricula(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(
            () => _mediator.Send(new ConsultarMatriculaQuery(id), cancellationToken),
            result => OkResponse(result));
    }

    /// <summary>
    /// Lista todas as matrículas de um aluno.
    /// </summary>
    /// <remarks>
    /// Retorna todas as matrículas associadas ao aluno, independentemente do status (ativas ou canceladas).
    /// </remarks>
    /// <param name="alunoId">Identificador único do aluno.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de matrículas do aluno e o total de registros.</returns>
    [HttpGet("aluno/{alunoId:guid}")]
    [ProducesResponseType(typeof(MatriculaListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarMatriculasPorAluno(
        [FromRoute] Guid alunoId,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(
            () => _mediator.Send(new ListarMatriculasPorAlunoQuery(alunoId), cancellationToken),
            result => OkResponse(result));
    }
}

/// <summary>
/// Dados necessários para realizar uma matrícula.
/// </summary>
/// <param name="AlunoId">Identificador único do aluno.</param>
/// <param name="DisciplinaId">Identificador único da disciplina.</param>
/// <param name="PeriodoAno">Ano letivo (entre 2000 e 2100).</param>
/// <param name="PeriodoSemestre">Semestre letivo (1 ou 2).</param>
public sealed record RealizarMatriculaRequest(
    Guid AlunoId,
    Guid DisciplinaId,
    int PeriodoAno,
    int PeriodoSemestre)
{
    public RealizarMatriculaCommand ToCommand() =>
        new(AlunoId, DisciplinaId, PeriodoAno, PeriodoSemestre);
}
