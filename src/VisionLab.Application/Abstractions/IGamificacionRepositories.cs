using VisionLab.Domain.Gamificacion;

namespace VisionLab.Application.Abstractions;

public interface ILogroRepository
{
    Task<List<Logro>> GetPendientesAsync(IEnumerable<string> desbloqueados, CancellationToken ct = default);
    Task<List<Logro>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken ct = default);
}

public interface INivelRepository
{
    /// <summary>Nivel más alto alcanzable con los puntos dados.</summary>
    Task<Nivel?> GetByPuntosAsync(int puntos, CancellationToken ct = default);
}
