using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VisionLab.Application.Abstractions;
using VisionLab.Infrastructure.Ai;
using VisionLab.Infrastructure.Persistence;
using VisionLab.Infrastructure.Persistence.Repositories;
using VisionLab.Infrastructure.Security;

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

        // --- Seguridad ---
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(o => o.Key.Length >= 32, "Jwt:Key debe tener al menos 32 caracteres (user-secrets o variable Jwt__Key)")
            .ValidateOnStart();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.Configure<ClaudeCliOptions>(configuration.GetSection(ClaudeCliOptions.SectionName));
        services.AddSingleton<ClaudeCliRunner>();
        services.AddSingleton<IVisionAi, ClaudeCliVisionAi>();

        services.AddHostedService<MongoIndexesInitializer>();
        services.AddHostedService<GamificacionSeeder>();
        services.AddHealthChecks().AddCheck<MongoHealthCheck>("mongodb");

        return services;
    }
}
