using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VisionLab.Domain.Gamificacion;
using VisionLab.Domain.Hallazgos;
using VisionLab.Domain.Usuarios;

namespace VisionLab.Infrastructure.Persistence;

/// <summary>Punto único de acceso a las colecciones de Mongo.</summary>
public sealed class MongoContext
{
    public MongoContext(IMongoClient client, IOptions<MongoOptions> options)
    {
        var o = options.Value;
        Database = client.GetDatabase(o.DatabaseName);
        Usuarios = Database.GetCollection<Usuario>(o.Collections.Usuarios);
        Hallazgos = Database.GetCollection<Hallazgo>(o.Collections.Hallazgos);
        Logros = Database.GetCollection<Logro>(o.Collections.Logros);
        Niveles = Database.GetCollection<Nivel>(o.Collections.Niveles);
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<Usuario> Usuarios { get; }
    public IMongoCollection<Hallazgo> Hallazgos { get; }
    public IMongoCollection<Logro> Logros { get; }
    public IMongoCollection<Nivel> Niveles { get; }

    /// <summary>Comparación de texto sin distinguir mayúsculas (emails).</summary>
    public static readonly Collation CaseInsensitive = new("es", strength: CollationStrength.Secondary);
}
