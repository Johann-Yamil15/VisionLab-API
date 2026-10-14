using VisionLab.Application.Abstractions;

namespace VisionLab.Application.Features.Hallazgos;

public sealed class EliminarHallazgoHandler(IHallazgoRepository hallazgos)
{
    /// <returns>false si el hallazgo no existe.</returns>
    public Task<bool> Handle(string id, CancellationToken ct) => hallazgos.DeleteAsync(id, ct);
}
