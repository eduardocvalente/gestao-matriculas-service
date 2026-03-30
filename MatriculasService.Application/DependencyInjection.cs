using FluentValidation;
using MatriculasService.Application.Behaviors;
using MatriculasService.Application.Commands.RealizarMatricula;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MatriculasService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));

        services.AddTransient<IValidator<RealizarMatriculaCommand>, RealizarMatriculaCommandValidator>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
