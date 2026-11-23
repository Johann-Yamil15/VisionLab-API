using MongoDB.Driver;
using VisionLab.Application.Abstractions;
using VisionLab.Domain.Hallazgos;

namespace VisionLab.Infrastructure.Persistence.Repositories;

internal sealed class HallazgoRepository(MongoContext db) : IHallazgoRepository
{
    private IMongoCollection<Hallazgo> Hallazgos => db.Hallazgos;

    public Task<List<Hallazgo>> GetAllAsync(string usuarioId, CancellationToken ct = default) =>
        Hallazgos.Find(h => h.UsuarioId == usuarioId)
            .SortByDescending(h => h.FechaCaptura)
            .ToListAsync(ct);

    public async Task<Hallazgo?> GetByIdAsync(string id, string usuarioId, CancellationToken ct = default) =>
        await Hallazgos.Find(h => h.Id == id && h.UsuarioId == usuarioId).FirstOrDefaultAsync(ct);

    // Sin la imagen: solo se usa para calcular el progreso y las fotos pesan mucho.
    public Task<List<Hallazgo>> GetByUsuarioAsync(string usuarioId, CancellationToken ct = default) =>
        Hallazgos.Find(h => h.UsuarioId == usuarioId)
            .Project<Hallazgo>(Builders<Hallazgo>.Projection.Exclude(h => h.ImagenBase64))
            .ToListAsync(ct);

    // Igualdad con collation en lugar de regex: sin inyección de patrones y usa el índice.
    public Task<List<Hallazgo>> GetByTipoAsync(string tipo, string usuarioId, CancellationToken ct = default) =>
        Hallazgos.Find(h => h.UsuarioId == usuarioId && h.Tipo == tipo, new FindOptions { Collation = MongoContext.CaseInsensitive })
            .SortByDescending(h => h.FechaCaptura)
            .ToListAsync(ct);

    public async Task<List<string>> GetTiposUnicosAsync(string usuarioId, CancellationToken ct = default) =>
        await (await Hallazgos.DistinctAsync(h => h.Tipo, h => h.UsuarioId == usuarioId, cancellationToken: ct))
            .ToListAsync(ct);

    public Task CreateAsync(Hallazgo hallazgo, CancellationToken ct = default) =>
        Hallazgos.InsertOneAsync(hallazgo, cancellationToken: ct);

    public async Task<bool> DeleteAsync(string id, string usuarioId, CancellationToken ct = default) =>
        (await Hallazgos.DeleteOneAsync(h => h.Id == id && h.UsuarioId == usuarioId, ct)).DeletedCount > 0;
}
