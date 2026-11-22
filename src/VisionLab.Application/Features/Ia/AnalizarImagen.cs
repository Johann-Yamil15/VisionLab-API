using VisionLab.Application.Abstractions;
using VisionLab.Application.Common;

namespace VisionLab.Application.Features.Ia;

public sealed record AnalizarImagenRequest(string? Imagen);

/// <summary>Una identificación alternativa, para cuando la IA no está segura.</summary>
public sealed record OpcionAnalisis(string Tipo, string NombreComun, string NombreCientifico);

/// <summary>Ficha que devuelve la IA. Los campos opcionales pueden venir vacíos.</summary>
public sealed record AnalisisDto
{
    public string Tipo { get; init; } = "Objetos";
    public string NombreComun { get; init; } = "";
    public string NombreCientifico { get; init; } = "";
    public string Descripcion { get; init; } = "";
    public int NivelConfianza { get; init; }
    public string? Habitat { get; init; }
    public string? Alimentacion { get; init; }
    public string? Colores { get; init; }
    public string? Tamano { get; init; }
    public string? Lema { get; init; }
    public string? Amenazas { get; init; }
    public string? ImpactoEcologico { get; init; }
    /// <summary>Bajo, Medio o Alto.</summary>
    public string? Peligro { get; init; }
    /// <summary>Solo con NivelConfianza &lt; 60: 3 identificaciones posibles. Vacío si hay confianza.</summary>
    public List<OpcionAnalisis> Opciones { get; init; } = [];
}

public sealed class AnalizarImagenHandler(IVisionAi ia)
{
    public const int MaxBytes = 5 * 1024 * 1024;
    public const int UmbralConfianza = 60;

    public async Task<Result<AnalisisDto>> Handle(AnalizarImagenRequest request, CancellationToken ct)
    {
        if (!TryDecodificar(request.Imagen, out var bytes))
            return Result<AnalisisDto>.Failure(ErrorType.Validation, "La imagen no es válida (base64 o data URL).");
        if (bytes.Length > MaxBytes)
            return Result<AnalisisDto>.Failure(ErrorType.Validation, "La imagen pesa más de 5 MB.");

        // El tipo se decide por el contenido real (firma del archivo), no por lo que declare el cliente.
        var extension = DetectarExtension(bytes);
        if (extension is null)
            return Result<AnalisisDto>.Failure(ErrorType.Validation, "Solo se aceptan imágenes jpeg, png o webp.");

        try
        {
            var analisis = await ia.AnalizarAsync(bytes, extension, ct);
            return Result<AnalisisDto>.Success(Normalizar(analisis));
        }
        catch (IaNoDisponibleException ex)
        {
            return Result<AnalisisDto>.Failure(ErrorType.ServiceUnavailable, ex.Message);
        }
    }

    internal static AnalisisDto Normalizar(AnalisisDto a) => a with
    {
        NivelConfianza = Math.Clamp(a.NivelConfianza, 0, 100),
        Tipo = Categorias.Todas.FirstOrDefault(c => c.Equals(a.Tipo, StringComparison.OrdinalIgnoreCase)) ?? "Objetos",
        Opciones = a.NivelConfianza < UmbralConfianza ? a.Opciones.Take(3).ToList() : [],
    };

    internal static bool TryDecodificar(string? imagen, out byte[] bytes)
    {
        bytes = [];
        if (string.IsNullOrWhiteSpace(imagen)) return false;
        var base64 = imagen.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
            ? imagen[(imagen.IndexOf(',') + 1)..]
            : imagen;
        try { bytes = Convert.FromBase64String(base64); return bytes.Length > 0; }
        catch (FormatException) { return false; }
    }

    internal static string? DetectarExtension(byte[] b) => b switch
    {
        [0xFF, 0xD8, 0xFF, ..] => "jpg",
        [0x89, 0x50, 0x4E, 0x47, ..] => "png",
        [(byte)'R', (byte)'I', (byte)'F', (byte)'F', _, _, _, _, (byte)'W', (byte)'E', (byte)'B', (byte)'P', ..] => "webp",
        _ => null,
    };
}
