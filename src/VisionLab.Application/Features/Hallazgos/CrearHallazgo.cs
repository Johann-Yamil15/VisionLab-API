using System.ComponentModel.DataAnnotations;
using VisionLab.Application.Abstractions;
using VisionLab.Application.Features.Gamificacion;
using VisionLab.Domain.Hallazgos;

namespace VisionLab.Application.Features.Hallazgos;

public sealed class CrearHallazgoRequest
{
    [Required]
    public string Tipo { get; init; } = string.Empty;
    public string NombreComun { get; init; } = string.Empty;
    public string NombreCientifico { get; init; } = string.Empty;
    public string Descripcion { get; init; } = string.Empty;

    [Range(0, 100)]
    public int NivelConfianza { get; init; }

    public string? ImagenBase64 { get; init; }

    [MaxLength(80)] public string? Habitat { get; init; }
    [MaxLength(80)] public string? Alimentacion { get; init; }
    [MaxLength(80)] public string? Colores { get; init; }
    [MaxLength(80)] public string? Tamano { get; init; }
    [MaxLength(160)] public string? Lema { get; init; }
}

public sealed class CrearHallazgoHandler(
    IHallazgoRepository hallazgos,
    ICurrentUser currentUser,
    OtorgarProgresoHandler progreso)
{
    public const string UsuarioAnonimo = "anonymous";

    public async Task<HallazgoDto> Handle(CrearHallazgoRequest request, CancellationToken ct)
    {
        var hallazgo = new Hallazgo
        {
            UsuarioId = currentUser.UserId ?? UsuarioAnonimo,
            Tipo = request.Tipo,
            NombreComun = request.NombreComun,
            NombreCientifico = request.NombreCientifico,
            Descripcion = request.Descripcion,
            NivelConfianza = request.NivelConfianza,
            ImagenBase64 = request.ImagenBase64,
            Habitat = request.Habitat,
            Alimentacion = request.Alimentacion,
            Colores = request.Colores,
            Tamano = request.Tamano,
            Lema = request.Lema,
            FechaCaptura = DateTime.UtcNow
        };

        await hallazgos.CreateAsync(hallazgo, ct);

        if (hallazgo.UsuarioId != UsuarioAnonimo)
            await progreso.Handle(hallazgo.UsuarioId, hallazgo, ct);

        return HallazgoDto.From(hallazgo);
    }
}
