using VisionLab.Application.Features.Gamificacion;
using VisionLab.Domain.Hallazgos;

namespace VisionLab.Application.Features.Hallazgos;

public sealed record HallazgoDto(
    string Id,
    string UsuarioId,
    string Tipo,
    string NombreComun,
    string NombreCientifico,
    string Descripcion,
    int NivelConfianza,
    string? ImagenBase64,
    DateTime FechaCaptura,
    string? Habitat,
    string? Alimentacion,
    string? Colores,
    string? Tamano,
    string? Lema,
    string? GrupoAtlas,
    string? Region,
    double? Latitud,
    double? Longitud)
{
    /// <summary>Solo al crear: puntos, medallas y nivel ganados con este hallazgo.</summary>
    public ProgresoDto? Progreso { get; init; }

    public static HallazgoDto From(Hallazgo h) =>
        new(h.Id, h.UsuarioId, h.Tipo, h.NombreComun, h.NombreCientifico, h.Descripcion,
            h.NivelConfianza, h.ImagenBase64, h.FechaCaptura,
            h.Habitat, h.Alimentacion, h.Colores, h.Tamano, h.Lema,
            h.GrupoAtlas, h.Region, h.Latitud, h.Longitud);
}
