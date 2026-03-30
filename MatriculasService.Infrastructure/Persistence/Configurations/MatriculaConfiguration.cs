using MatriculasService.Infrastructure.Persistence.Configurations.Base;
using MatriculasService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatriculasService.Infrastructure.Persistence.Configurations;

public sealed class MatriculaConfiguration : BasePersistenceEntityConfiguration<MatriculaEntity>
{
    public override void Configure(EntityTypeBuilder<MatriculaEntity> builder)
    {
        builder.ToTable("matriculas");

        base.Configure(builder);

        builder.Property(m => m.AlunoId)
            .IsRequired();

        builder.Property(m => m.DisciplinaId)
            .IsRequired();

        builder.Property(m => m.PeriodoAno)
            .IsRequired();

        builder.Property(m => m.PeriodoSemestre)
            .IsRequired();

        builder.Property(m => m.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(m => new { m.AlunoId, m.DisciplinaId, m.PeriodoAno, m.PeriodoSemestre })
            .HasDatabaseName("ix_matriculas_aluno_disciplina_periodo")
            .HasFilter("status = 'Confirmada'");
    }
}
