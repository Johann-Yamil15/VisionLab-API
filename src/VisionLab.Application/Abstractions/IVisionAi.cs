using VisionLab.Application.Features.Ia;

namespace VisionLab.Application.Abstractions;

/// <summary>Se lanza cuando la IA falla, tarda demasiado o no devuelve algo utilizable.</summary>
public sealed class IaNoDisponibleException(string message, Exception? inner = null) : Exception(message, inner);

/// <summary>Puerto hacia la IA. La implementación actual usa el CLI de Claude.</summary>
public interface IVisionAi
{
    /// <exception cref="IaNoDisponibleException">La IA no pudo responder.</exception>
    Task<AnalisisDto> AnalizarAsync(byte[] imagen, string extension, CancellationToken ct);

    /// <exception cref="IaNoDisponibleException">La IA no pudo responder.</exception>
    Task<string> ChatAsync(EspecieChat especie, string pregunta, CancellationToken ct);
}
