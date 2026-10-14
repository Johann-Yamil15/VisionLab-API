using VisionLab.Domain.Usuarios;

namespace VisionLab.Domain.Gamificacion;

/// <summary>
/// Medalla que se desbloquea al cumplir un requisito (ej: 5 hallazgos totales).
/// </summary>
public class Logro
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Icono { get; set; } = string.Empty;
    public int RequisitoValor { get; set; }
    public string RequisitoTipo { get; set; } = string.Empty;
    public int PuntosOtorgados { get; set; }

    public bool EstaCumplidoPor(Usuario usuario) => RequisitoTipo switch
    {
        TiposRequisito.HallazgosTotales => usuario.HallazgosContador >= RequisitoValor,
        _ => false
    };
}

public static class TiposRequisito
{
    public const string HallazgosTotales = "HallazgosTotales";
}
