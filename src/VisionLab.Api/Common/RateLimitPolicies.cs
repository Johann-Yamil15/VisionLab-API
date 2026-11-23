namespace VisionLab.Api.Common;

public static class RateLimitPolicies
{
    /// <summary>Límite por usuario para los endpoints que gastan cuota de la IA.</summary>
    public const string Ia = "ia";

    /// <summary>Límite por IP para login y registro (frena fuerza bruta).</summary>
    public const string Auth = "auth";
}
