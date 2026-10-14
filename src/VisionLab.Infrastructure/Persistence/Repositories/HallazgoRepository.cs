using MongoDB.Driver;
using VisionLab.Application.Abstractions;
using VisionLab.Domain.Hallazgos;

namespace VisionLab.Infrastructure.Persistence.Repositories;

internal sealed class HallazgoRepository(MongoContext db) : IHallazgoRepository
{
    private IMongoCollection<Hallazgo> Hallazgos => db.Hallazgos;

    public Task<List<Hallazgo>> GetAllAsync(CancellationToken ct = default) =>
        Hallazgos.Find(FilterDefinition<Hallazgo>.Empty)
            .SortByDescending(h => h.FechaCaptura)
            .ToListAsync(ct);

    public async Task<Hallazgo?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await Hallazgos.Find(h => h.Id == id).FirstOrDefaultAsync(ct);

    // Igualdad con collation en lugar de regex: sin inyección de patrones y usa el índice.
    public Task<List<Hallazgo>> GetByTipoAsync(string tipo, CancellationToken ct = default) =>
        Hallazgos.Find(h => h.Tipo == tipo, new FindOptions { Collation = MongoContext.CaseInsensitive })
            .SortByDescending(h => h.FechaCaptura)
            .ToListAsync(ct);

    public async Task<List<string>> GetTiposUnicosAsync(CancellationToken ct = default) =>
        await (await Hallazgos.DistinctAsync(h => h.Tipo, FilterDefinition<Hallazgo>.Empty, cancellationToken: ct))
            .ToListAsync(ct);

    public Task CreateAsync(Hallazgo hallazgo, CancellationToken ct = default) =>
        Hallazgos.InsertOneAsync(hallazgo, cancellationToken: ct);

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default) =>
        (await Hallazgos.DeleteOneAsync(h => h.Id == id, ct)).DeletedCount > 0;
}
