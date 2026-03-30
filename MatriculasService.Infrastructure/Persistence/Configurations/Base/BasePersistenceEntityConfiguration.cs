using MatriculasService.Infrastructure.Persistence.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatriculasService.Infrastructure.Persistence.Configurations.Base;

public abstract class BasePersistenceEntityConfiguration<TEntity>
    : IEntityTypeConfiguration<TEntity>
    where TEntity : BasePersistenceEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);
    }
}
