using VisionLab.Application.Abstractions;

namespace VisionLab.Application.Features.Hallazgos;

public sealed class ListarHallazgosHandler(IHallazgoRepository hallazgos)
{
    public async Task<List<HallazgoDto>> Handle(CancellationToken ct) =>
        (await hallazgos.GetAllAsync(ct)).Select(HallazgoDto.From).ToList();
}

public sealed class ObtenerHallazgoHandler(IHallazgoRepository hallazgos)
{
    public async Task<HallazgoDto?> Handle(string id, CancellationToken ct) =>
        await hallazgos.GetByIdAsync(id, ct) is { } h ? HallazgoDto.From(h) : null;
}

public sealed class ListarHallazgosPorTipoHandler(IHallazgoRepository hallazgos)
{
    public async Task<List<HallazgoDto>> Handle(string tipo, CancellationToken ct) =>
        (await hallazgos.GetByTipoAsync(tipo, ct)).Select(HallazgoDto.From).ToList();
}

public sealed class ListarTiposHandler(IHallazgoRepository hallazgos)
{
    public async Task<List<string>> Handle(CancellationToken ct) =>
        (await hallazgos.GetTiposUnicosAsync(ct)).Order().ToList();
}
