using VisionLab.Domain.Hallazgos;

namespace VisionLab.Application.Abstractions;

/// <summary>Las consultas están acotadas al dueño (usuarioId): nadie ve ni borra hallazgos ajenos.</summary>
public interface IHallazgoRepository
{
    Task<List<Hallazgo>> GetAllAsync(string usuarioId, CancellationToken ct = default);
    Task<Hallazgo?> GetByIdAsync(string id, string usuarioId, CancellationToken ct = default);
    /// <summary>Sin la imagen (para calcular el progreso).</summary>
    Task<List<Hallazgo>> GetByUsuarioAsync(string usuarioId, CancellationToken ct = default);
    Task<List<Hallazgo>> GetByTipoAsync(string tipo, string usuarioId, CancellationToken ct = default);
    Task<List<string>> GetTiposUnicosAsync(string usuarioId, CancellationToken ct = default);
    Task CreateAsync(Hallazgo hallazgo, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, string usuarioId, CancellationToken ct = default);
}
