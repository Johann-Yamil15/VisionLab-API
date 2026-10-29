using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using VisionLab.Domain.Gamificacion;
using VisionLab.Domain.Hallazgos;
using VisionLab.Domain.Usuarios;

namespace VisionLab.Infrastructure.Persistence;

/// <summary>
/// Mapeo entidades ↔ documentos. Mantiene los nombres de campo existentes en la base
/// para no perder datos, sin poner atributos de Mongo en el Domain.
/// </summary>
internal static class MongoMappings
{
    private static int _registered;

    public static void Register()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1) return;

        Map<Usuario>(cm => cm.MapMember(u => u.Contrasena).SetElementName("Contraseña"));
        Map<Hallazgo>();
        Map<Logro>();
        Map<Nivel>();
    }

    private static void Map<T>(Action<BsonClassMap<T>>? configure = null) where T : class
    {
        BsonClassMap.RegisterClassMap<T>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.MapIdMember(typeof(T).GetProperty("Id"))
              .SetIdGenerator(StringObjectIdGenerator.Instance)
              .SetSerializer(new StringSerializer(BsonType.ObjectId));
            configure?.Invoke(cm);
        });
    }
}
