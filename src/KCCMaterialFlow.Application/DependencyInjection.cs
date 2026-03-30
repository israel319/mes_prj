using System.Reflection;
using FluentValidation;
using KCCMaterialFlow.Application.Common.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace KCCMaterialFlow.Application;

/// <summary>
/// Extension method pour l'enregistrement DI de la couche Application.
/// Appelé depuis Program.cs : services.AddApplication();
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR — découvre automatiquement tous les Handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation — découvre automatiquement tous les Validators
        services.AddValidatorsFromAssembly(assembly);

        // AutoMapper — découvre automatiquement tous les Profiles
        services.AddAutoMapper(cfg => cfg.AddMaps(assembly), assembly);

        // Pipeline Behaviours — exécutés dans l'ordre pour chaque request
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }
}
