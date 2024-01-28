using FluentValidation;
using iPractice.Domain.Pipeline;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace iPractice.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DefaultPipelineBehavior<,>));

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<IDomainMarker>();
        });

        services.Scan(x =>
        {
            x.FromAssemblyOf<IDomainMarker>()
                .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime();
        });

        return services;
    }
}

public interface IDomainMarker
{
}
