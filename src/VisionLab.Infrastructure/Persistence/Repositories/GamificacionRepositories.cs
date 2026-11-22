using MongoDB.Driver;
using VisionLab.Application.Abstractions;
using VisionLab.Domain.Gamificacion;

namespace VisionLab.Infrastructure.Persistence.Repositories;

internal sealed class LogroRepository(MongoContext db) : ILogroRepository
{
    public Task<List<Logro>> GetPendientesAsync(IEnumerable<string> desbloqueados, CancellationToken ct = default) =>
        db.Logros.Find(Builders<Logro>.Filter.Nin(l => l.Id, desbloqueados)).ToListAsync(ct);

    public Task<List<Logro>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken ct = default) =>
        db.Logros.Find(Builders<Logro>.Filter.In(l => l.Id, ids)).ToListAsync(ct);

    public Task<List<Logro>> GetAllAsync(CancellationToken ct = default) =>
        db.Logros.Find(FilterDefinition<Logro>.Empty).SortBy(l => l.RequisitoValor).ToListAsync(ct);
}

internal sealed class NivelRepository(MongoContext db) : INivelRepository
{
    public Task<List<Nivel>> GetAllAsync(CancellationToken ct = default) =>
        db.Niveles.Find(FilterDefinition<Nivel>.Empty).SortBy(n => n.Numero).ToListAsync(ct);

    public async Task<Nivel?> GetByPuntosAsync(int puntos, CancellationToken ct = default) =>
        await db.Niveles.Find(n => n.PuntosRequeridos <= puntos)
            .SortByDescending(n => n.Numero)
            .FirstOrDefaultAsync(ct);
}
