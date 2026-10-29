using VisionLab.Domain.Hallazgos;

namespace VisionLab.Domain.Gamificacion;

public static class ReglasDePuntos
{
    public const int PuntosBasePorHallazgo = 10;

    /// <summary>10 puntos base + 1 punto por cada 10% de confianza.</summary>
    public static int PorHallazgo(Hallazgo hallazgo) =>
        PuntosBasePorHallazgo + (int)Math.Round(hallazgo.NivelConfianza / 10.0);
}
