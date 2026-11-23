namespace VisionLab.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>Clave de firma (HS256), mínimo 32 caracteres. En producción: variable Jwt__Key.</summary>
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "VisionLab";
    public string Audience { get; set; } = "VisionLabApp";
    public int DiasValidez { get; set; } = 7;
}
