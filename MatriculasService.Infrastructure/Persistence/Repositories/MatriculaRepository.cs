using MatriculasService.Domain.Entities;
using MatriculasService.Domain.Enums;
using MatriculasService.Domain.Interfaces;
using MatriculasService.Domain.ValueObjects;
using MatriculasService.Infrastructure.Persistence.Entities;
using MatriculasService.Infrastructure.Persistence.Mappers.Base;
using Microsoft.EntityFrameworkCore;

namespace MatriculasService.Infrastructure.Persistence.Repositories;

public sealed class MatriculaRepository : IMatriculaRepository
{
    private readonly MatriculaDbContext _context;
    private readonly IEntityMapper<Matricula, MatriculaEntity> _mapper;

    public MatriculaRepository(
        MatriculaDbContext context,
        IEntityMapper<Matricula, MatriculaEntity> mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Matricula?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Matriculas
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        return entity is null ? null : _mapper.ToDomain(entity);
    }

    public async Task<bool> ExisteMatriculaAtivaAsync(
        Guid alunoId,
        Guid disciplinaId,
        PeriodoLetivo periodo,
        CancellationToken cancellationToken)
    {
        return await _context.Matriculas.AnyAsync(
            m => m.AlunoId == alunoId
              && m.DisciplinaId == disciplinaId
              && m.PeriodoAno == periodo.Ano
              && m.PeriodoSemestre == periodo.Semestre
              && m.Status == StatusMatricula.Confirmada.ToString(),
            cancellationToken);
    }

    public async Task AdicionarAsync(Matricula matricula, CancellationToken cancellationToken)
    {
        var entity = _mapper.ToInfrastructure(matricula);
        await _context.Matriculas.AddAsync(entity, cancellationToken);
    }

    public async Task AtualizarAsync(Matricula matricula, CancellationToken cancellationToken)
    {
        var entity = await _context.Matriculas
            .FirstOrDefaultAsync(m => m.Id == matricula.Id, cancellationToken);

        if (entity is null) return;

        entity.Status = matricula.Status.ToString();
        entity.UpdatedAt = matricula.UpdatedAt;
    }

    public async Task<IReadOnlyList<Matricula>> ListarPorAlunoAsync(
        Guid alunoId,
        CancellationToken cancellationToken)
    {
        var entities = await _context.Matriculas
            .AsNoTracking()
            .Where(m => m.AlunoId == alunoId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(_mapper.ToDomain).ToList().AsReadOnly();
    }
}
