namespace VisionLab.Domain.Gamificacion;

/// <summary>
/// Medalla que se desbloquea al cumplir un requisito (ej: 5 hallazgos totales).
/// </summary>
public class Logro
{
    public string Id { get; set; } = string.Empty;
    /// <summary>Identificador estable para el front y el catálogo (ej: "ojos-curiosos").</summary>
    public string Clave { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    /// <summary>El reto que se muestra al usuario.</summary>
    public string Descripcion { get; set; } = string.Empty;
    /// <summary>Imagen de la medalla dorada (ruta pública del front).</summary>
    public string Icono { get; set; } = string.Empty;
    /// <summary>Imagen plateada con candado; si está vacía, el front muestra la dorada en gris.</summary>
    public string IconoBloqueado { get; set; } = string.Empty;
    public int RequisitoValor { get; set; }
    public string RequisitoTipo { get; set; } = string.Empty;
    public int PuntosOtorgados { get; set; }

    public int ValorDe(Estadisticas e) => RequisitoTipo switch
    {
        TiposRequisito.HallazgosTotales => e.Total,
        TiposRequisito.EspeciesDistintas => e.Especies,
        TiposRequisito.CategoriasDistintas => e.Categorias,
        TiposRequisito.FaunaDistinta => e.Fauna,
        _ => 0
    };

    public bool EstaCumplidoPor(Estadisticas e) => ValorDe(e) >= RequisitoValor;
}

public static class TiposRequisito
{
    public const string HallazgosTotales = "HallazgosTotales";
    public const string EspeciesDistintas = "EspeciesDistintas";
    public const string CategoriasDistintas = "CategoriasDistintas";
    public const string FaunaDistinta = "FaunaDistinta";
}
