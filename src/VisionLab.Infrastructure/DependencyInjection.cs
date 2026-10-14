using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VisionLab.Application.Abstractions;
using VisionLab.Infrastructure.Persistence;
using VisionLab.Infrastructure.Persistence.Repositories;

namespace VisionLab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        MongoMappings.Register();

        services.AddOptions<MongoOptions>()
            .Bind(configuration.GetSection(MongoOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString), "Falta VisionLabDatabase:ConnectionString")
            .Validate(o => !string.IsNullOrWhiteSpace(o.DatabaseName), "Falta VisionLabDatabase:DatabaseName")
            .ValidateOnStart();

        // El cliente de Mongo es thread-safe y maneja su propio pool: una instancia por proceso.
        services.AddSingleton<IMongoClient>(sp =>
            new MongoClient(sp.GetRequiredService<IOptions<MongoOptions>>().Value.ConnectionString));
        services.AddSingleton<MongoContext>();

        services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
        services.AddSingleton<IHallazgoRepository, HallazgoRepository>();
        services.AddSingleton<ILogroRepository, LogroRepository>();
        services.AddSingleton<INivelRepository, NivelRepository>();

        services.AddHostedService<MongoIndexesInitializer>();
        services.AddHealthChecks().AddCheck<MongoHealthCheck>("mongodb");

        return services;
    }
}
