using MatriculasService.Domain.Enums;
using MatriculasService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MatriculasService.Infrastructure.Persistence;

public sealed class DatabaseSeeder
{
    private readonly MatriculaDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    // GUIDs fixos para facilitar testes manuais no Swagger/Postman
    public static readonly Guid AlunoId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid AlunoId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid AlunoId3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid DisciplinaId1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid DisciplinaId2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid DisciplinaId3 = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    public DatabaseSeeder(MatriculaDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Matriculas.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Seed ignorado: banco de dados já contém dados.");
            return;
        }

        _logger.LogInformation("Iniciando seed do banco de dados...");

        var matriculas = new List<MatriculaEntity>
        {
            // Aluno 1 — 2 matrículas confirmadas no mesmo período
            new()
            {
                Id = Guid.Parse("a0000001-0000-0000-0000-000000000001"),
                AlunoId = AlunoId1,
                DisciplinaId = DisciplinaId1,
                PeriodoAno = 2025,
                PeriodoSemestre = 1,
                Status = StatusMatricula.Confirmada.ToString(),
                CreatedAt = new DateTime(2025, 2, 1, 8, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("a0000001-0000-0000-0000-000000000002"),
                AlunoId = AlunoId1,
                DisciplinaId = DisciplinaId2,
                PeriodoAno = 2025,
                PeriodoSemestre = 1,
                Status = StatusMatricula.Confirmada.ToString(),
                CreatedAt = new DateTime(2025, 2, 1, 8, 5, 0, DateTimeKind.Utc)
            },
            // Aluno 1 — matrícula cancelada em período anterior
            new()
            {
                Id = Guid.Parse("a0000001-0000-0000-0000-000000000003"),
                AlunoId = AlunoId1,
                DisciplinaId = DisciplinaId3,
                PeriodoAno = 2024,
                PeriodoSemestre = 2,
                Status = StatusMatricula.Cancelada.ToString(),
                CreatedAt = new DateTime(2024, 8, 1, 10, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 9, 15, 14, 30, 0, DateTimeKind.Utc)
            },

            // Aluno 2 — matrícula confirmada no semestre atual
            new()
            {
                Id = Guid.Parse("a0000002-0000-0000-0000-000000000001"),
                AlunoId = AlunoId2,
                DisciplinaId = DisciplinaId1,
                PeriodoAno = 2025,
                PeriodoSemestre = 1,
                Status = StatusMatricula.Confirmada.ToString(),
                CreatedAt = new DateTime(2025, 2, 3, 9, 0, 0, DateTimeKind.Utc)
            },
            // Aluno 2 — matrícula confirmada em período anterior
            new()
            {
                Id = Guid.Parse("a0000002-0000-0000-0000-000000000002"),
                AlunoId = AlunoId2,
                DisciplinaId = DisciplinaId2,
                PeriodoAno = 2024,
                PeriodoSemestre = 2,
                Status = StatusMatricula.Confirmada.ToString(),
                CreatedAt = new DateTime(2024, 8, 5, 11, 0, 0, DateTimeKind.Utc)
            },

            // Aluno 3 — sem matrículas ativas (todas canceladas)
            new()
            {
                Id = Guid.Parse("a0000003-0000-0000-0000-000000000001"),
                AlunoId = AlunoId3,
                DisciplinaId = DisciplinaId1,
                PeriodoAno = 2025,
                PeriodoSemestre = 1,
                Status = StatusMatricula.Cancelada.ToString(),
                CreatedAt = new DateTime(2025, 2, 10, 7, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 3, 1, 16, 0, 0, DateTimeKind.Utc)
            }
        };

        await _context.Matriculas.AddRangeAsync(matriculas, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seed concluído: {Count} matrículas inseridas.", matriculas.Count);
    }
}
