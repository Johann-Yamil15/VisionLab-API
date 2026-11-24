namespace VisionLab.Application.Features.Ia;

/// <summary>Reglas del Atlas personal: grupo y coordenadas válidos, o nada.</summary>
public static class UbicacionAtlas
{
    public static readonly string[] Grupos = ["fauna", "flora", "mineral"];
    public const int MaxRegion = 80;

    /// <summary>
    /// Devuelve la ubicación saneada o todo nulo. Es todo o nada: sin grupo válido o sin coordenadas
    /// dentro de rango, el hallazgo no aparece en el Atlas (y nunca se guarda una ubicación a medias).
    /// </summary>
    public static (string? Grupo, string? Region, double? Latitud, double? Longitud) Sanear(
        string? grupo, string? region, double? latitud, double? longitud)
    {
        var g = Grupos.FirstOrDefault(x => x.Equals(grupo?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (g is null || latitud is not { } lat || longitud is not { } lng) return (null, null, null, null);
        if (double.IsNaN(lat) || double.IsNaN(lng) || lat is < -90 or > 90 || lng is < -180 or > 180)
            return (null, null, null, null);

        var texto = string.Join(' ', (region ?? "").Where(c => !char.IsControl(c) || c == ' ')
            .ToArray().AsSpan().ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        if (texto.Length > MaxRegion) texto = texto[..MaxRegion].TrimEnd();

        return (g, texto.Length > 0 ? texto : null, Math.Round(lat, 4), Math.Round(lng, 4));
    }
}
