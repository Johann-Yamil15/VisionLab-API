using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using VisionLab.Domain.Gamificacion;

namespace VisionLab.Infrastructure.Persistence;

/// <summary>
/// Guarda en Mongo los niveles y medallas de <see cref="Catalogo"/> al arrancar (upsert por
/// Numero y por Clave: idempotente y conserva los Id, así no se pierden medallas ya otorgadas).
/// </summary>
internal sealed class GamificacionSeeder(MongoContext db, ILogger<GamificacionSeeder> logger) : BackgroundService
{
    private static readonly UpdateOptions Upsert = new() { IsUpsert = true };

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        try
        {
            foreach (var n in Catalogo.Niveles)
                await db.Niveles.UpdateOneAsync(x => x.Numero == n.Numero, Builders<Nivel>.Update
                    .Set(x => x.Titulo, n.Titulo)
                    .Set(x => x.PuntosRequeridos, n.PuntosRequeridos), Upsert, ct);

            foreach (var l in Catalogo.Logros)
                await db.Logros.UpdateOneAsync(x => x.Clave == l.Clave, Builders<Logro>.Update
                    .Set(x => x.Nombre, l.Nombre)
                    .Set(x => x.Descripcion, l.Descripcion)
                    .Set(x => x.Icono, l.Icono)
                    .Set(x => x.IconoBloqueado, l.IconoBloqueado)
                    .Set(x => x.RequisitoTipo, l.RequisitoTipo)
                    .Set(x => x.RequisitoValor, l.RequisitoValor)
                    .Set(x => x.PuntosOtorgados, l.PuntosOtorgados), Upsert, ct);

            logger.LogInformation("Catálogo de gamificación listo: {Niveles} niveles, {Logros} medallas",
                Catalogo.Niveles.Length, Catalogo.Logros.Length);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "No se pudo sembrar el catálogo de gamificación");
        }
    }
}
