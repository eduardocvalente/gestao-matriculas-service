using FluentValidation;

namespace MatriculasService.Application.Commands.RealizarMatricula;

public sealed class RealizarMatriculaCommandValidator : AbstractValidator<RealizarMatriculaCommand>
{
    public RealizarMatriculaCommandValidator()
    {
        RuleFor(x => x.AlunoId)
            .NotEmpty()
            .WithMessage("O ID do aluno é obrigatório.");

        RuleFor(x => x.DisciplinaId)
            .NotEmpty()
            .WithMessage("O ID da disciplina é obrigatório.");

        RuleFor(x => x.PeriodoAno)
            .InclusiveBetween(2000, 2100)
            .WithMessage("O ano letivo deve estar entre 2000 e 2100.");

        RuleFor(x => x.PeriodoSemestre)
            .Must(s => s == 1 || s == 2)
            .WithMessage("O semestre deve ser 1 ou 2.");
    }
}
