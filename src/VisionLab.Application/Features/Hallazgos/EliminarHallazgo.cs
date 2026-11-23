using VisionLab.Application.Abstractions;

namespace VisionLab.Application.Features.Hallazgos;

public sealed class EliminarHallazgoHandler(IHallazgoRepository hallazgos, ICurrentUser currentUser)
{
    /// <returns>false si el hallazgo no existe o no es del usuario.</returns>
    public async Task<bool> Handle(string id, CancellationToken ct) =>
        currentUser.UserId is { } uid && await hallazgos.DeleteAsync(id, uid, ct);
}
