using MatriculasService.Application.Interfaces;
using MatriculasService.Application.Options;
using MatriculasService.Domain.Entities;
using MatriculasService.Domain.Interfaces;
using MatriculasService.Infrastructure.Messaging;
using MatriculasService.Infrastructure.Persistence;
using MatriculasService.Infrastructure.Persistence.Entities;
using MatriculasService.Infrastructure.Persistence.Mappers;
using MatriculasService.Infrastructure.Persistence.Mappers.Base;
using MatriculasService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MatriculasService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<MatriculaDbContext>((provider, options) =>
        {
            var dbOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options
                .UseNpgsql(dbOptions.ConnectionString, npgsql =>
                {
                    npgsql.CommandTimeout(dbOptions.CommandTimeout);
                    npgsql.EnableRetryOnFailure(3);
                })
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IEntityMapper<Matricula, MatriculaEntity>, MatriculaMapper>();
        services.AddScoped<IMatriculaRepository, MatriculaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
