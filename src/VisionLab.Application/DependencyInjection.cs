using Microsoft.Extensions.DependencyInjection;

namespace VisionLab.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra todos los casos de uso (clases *Handler) de las features.
    /// Agregar una feature nueva no requiere tocar este archivo.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var handlers = typeof(DependencyInjection).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.Name.EndsWith("Handler"));

        foreach (var handler in handlers)
            services.AddScoped(handler);

        return services;
    }
}
