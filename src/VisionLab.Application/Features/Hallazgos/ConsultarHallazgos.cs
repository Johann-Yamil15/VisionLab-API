using VisionLab.Application.Abstractions;

namespace VisionLab.Application.Features.Hallazgos;

public sealed class ListarHallazgosHandler(IHallazgoRepository hallazgos, ICurrentUser currentUser)
{
    public async Task<List<HallazgoDto>> Handle(CancellationToken ct) =>
        currentUser.UserId is not { } uid
            ? []
            : (await hallazgos.GetAllAsync(uid, ct)).Select(HallazgoDto.From).ToList();
}

public sealed class ObtenerHallazgoHandler(IHallazgoRepository hallazgos, ICurrentUser currentUser)
{
    public async Task<HallazgoDto?> Handle(string id, CancellationToken ct) =>
        currentUser.UserId is { } uid && await hallazgos.GetByIdAsync(id, uid, ct) is { } h
            ? HallazgoDto.From(h)
            : null;
}

public sealed class ListarHallazgosPorTipoHandler(IHallazgoRepository hallazgos, ICurrentUser currentUser)
{
    public async Task<List<HallazgoDto>> Handle(string tipo, CancellationToken ct) =>
        currentUser.UserId is not { } uid
            ? []
            : (await hallazgos.GetByTipoAsync(tipo, uid, ct)).Select(HallazgoDto.From).ToList();
}

public sealed class ListarTiposHandler(IHallazgoRepository hallazgos, ICurrentUser currentUser)
{
    public async Task<List<string>> Handle(CancellationToken ct) =>
        currentUser.UserId is not { } uid
            ? []
            : (await hallazgos.GetTiposUnicosAsync(uid, ct)).Order().ToList();
}
