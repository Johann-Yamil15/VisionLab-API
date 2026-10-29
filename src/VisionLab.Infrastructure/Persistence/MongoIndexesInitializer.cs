using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using VisionLab.Domain.Gamificacion;
using VisionLab.Domain.Hallazgos;
using VisionLab.Domain.Usuarios;

namespace VisionLab.Infrastructure.Persistence;

/// <summary>
/// Crea los índices al arrancar (idempotente). Si Mongo no está disponible o hay datos que
/// violan un índice, solo registra la advertencia: la API no se cae por esto.
/// </summary>
internal sealed class MongoIndexesInitializer(MongoContext db, ILogger<MongoIndexesInitializer> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await TryCreate("usuarios.Email (único)", () => db.Usuarios.Indexes.CreateOneAsync(
            new CreateIndexModel<Usuario>(
                Builders<Usuario>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true, Collation = MongoContext.CaseInsensitive }),
            cancellationToken: ct));

        await TryCreate("hallazgos.UsuarioId+FechaCaptura", () => db.Hallazgos.Indexes.CreateOneAsync(
            new CreateIndexModel<Hallazgo>(Builders<Hallazgo>.IndexKeys
                .Ascending(h => h.UsuarioId).Descending(h => h.FechaCaptura)),
            cancellationToken: ct));

        await TryCreate("hallazgos.Tipo", () => db.Hallazgos.Indexes.CreateOneAsync(
            new CreateIndexModel<Hallazgo>(Builders<Hallazgo>.IndexKeys.Ascending(h => h.Tipo),
                new CreateIndexOptions { Collation = MongoContext.CaseInsensitive }),
            cancellationToken: ct));

        await TryCreate("niveles.PuntosRequeridos", () => db.Niveles.Indexes.CreateOneAsync(
            new CreateIndexModel<Nivel>(Builders<Nivel>.IndexKeys.Ascending(n => n.PuntosRequeridos)),
            cancellationToken: ct));
    }

    private async Task TryCreate(string name, Func<Task> create)
    {
        try
        {
            await create();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "No se pudo crear el índice {Index}", name);
        }
    }
}
