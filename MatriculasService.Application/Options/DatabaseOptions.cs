using System.ComponentModel.DataAnnotations;

namespace MatriculasService.Application.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required(ErrorMessage = "A connection string do banco de dados é obrigatória.")]
    public string ConnectionString { get; init; } = string.Empty;

    public int CommandTimeout { get; init; } = 30;
}
