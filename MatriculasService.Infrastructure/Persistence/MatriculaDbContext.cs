using MatriculasService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatriculasService.Infrastructure.Persistence;

public sealed class MatriculaDbContext : DbContext
{
    public MatriculaDbContext(DbContextOptions<MatriculaDbContext> options) : base(options) { }

    public DbSet<MatriculaEntity> Matriculas => Set<MatriculaEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MatriculaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
