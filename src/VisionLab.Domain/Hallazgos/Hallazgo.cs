namespace VisionLab.Domain.Hallazgos;

/// <summary>
/// Un descubrimiento del niño analizado por la IA (insecto, hoja, roca...).
/// </summary>
public class Hallazgo
{
    public string Id { get; set; } = string.Empty;
    public string UsuarioId { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string NombreComun { get; set; } = string.Empty;
    public string NombreCientifico { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Confianza del análisis de 0 a 100.</summary>
    public int NivelConfianza { get; set; }

    public string? ImagenBase64 { get; set; }

    // Datos rápidos para la tarjeta (opcionales: hallazgos antiguos no los tienen).
    public string? Habitat { get; set; }
    public string? Alimentacion { get; set; }
    public string? Colores { get; set; }
    public string? Tamano { get; set; }

    /// <summary>Frase corta y divertida que aparece al pie de la tarjeta.</summary>
    public string? Lema { get; set; }
    // Dónde vive la especie (no dónde se tomó la foto): sirve para ubicarla en el Atlas personal.
    // Todos nulos si no aplica (objetos, fenómenos) o si el hallazgo es anterior a esta función.
    /// <summary>"fauna", "flora" o "mineral": grupo del Atlas en el que aparece.</summary>
    public string? GrupoAtlas { get; set; }
    public string? Region { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }

    public DateTime FechaCaptura { get; set; } = DateTime.UtcNow;
}
