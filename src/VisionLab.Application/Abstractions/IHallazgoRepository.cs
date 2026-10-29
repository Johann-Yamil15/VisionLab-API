using VisionLab.Domain.Hallazgos;

namespace VisionLab.Application.Abstractions;

public interface IHallazgoRepository
{
    Task<List<Hallazgo>> GetAllAsync(CancellationToken ct = default);
    Task<Hallazgo?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<List<Hallazgo>> GetByTipoAsync(string tipo, CancellationToken ct = default);
    Task<List<string>> GetTiposUnicosAsync(CancellationToken ct = default);
    Task CreateAsync(Hallazgo hallazgo, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}
