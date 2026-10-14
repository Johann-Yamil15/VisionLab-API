namespace VisionLab.Infrastructure.Persistence;

/// <summary>
/// Sección "VisionLabDatabase" de la configuración. En Docker/producción se sobreescribe con
/// variables de entorno, ej: VisionLabDatabase__ConnectionString.
/// </summary>
public sealed class MongoOptions
{
    public const string SectionName = "VisionLabDatabase";

    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public CollectionNames Collections { get; set; } = new();

    public sealed class CollectionNames
    {
        public string Hallazgos { get; set; } = "hallazgos";
        public string Usuarios { get; set; } = "usuarios";
        public string Niveles { get; set; } = "niveles";
        public string Logros { get; set; } = "logros";
    }
}
