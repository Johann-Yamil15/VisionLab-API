using VisionLab.Domain.Usuarios;

namespace VisionLab.Application.Abstractions;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task CreateAsync(Usuario usuario, CancellationToken ct = default);
    Task AddProgressAsync(string id, int puntos, int hallazgos, CancellationToken ct = default);
    Task AddLogrosAsync(string id, IReadOnlyCollection<string> logroIds, int puntos, CancellationToken ct = default);
    Task SetNivelAsync(string id, int nivel, CancellationToken ct = default);
}
