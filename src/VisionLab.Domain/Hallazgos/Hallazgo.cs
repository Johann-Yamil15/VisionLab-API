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
    public DateTime FechaCaptura { get; set; } = DateTime.UtcNow;
}
