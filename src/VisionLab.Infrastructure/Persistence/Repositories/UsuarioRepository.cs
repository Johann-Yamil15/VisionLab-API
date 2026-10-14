using MongoDB.Driver;
using VisionLab.Application.Abstractions;
using VisionLab.Domain.Usuarios;

namespace VisionLab.Infrastructure.Persistence.Repositories;

internal sealed class UsuarioRepository(MongoContext db) : IUsuarioRepository
{
    private IMongoCollection<Usuario> Usuarios => db.Usuarios;

    public async Task<Usuario?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await Usuarios.Find(u => u.Id == id).FirstOrDefaultAsync(ct);

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await Usuarios.Find(u => u.Email == email, new FindOptions { Collation = MongoContext.CaseInsensitive })
            .FirstOrDefaultAsync(ct);

    public Task CreateAsync(Usuario usuario, CancellationToken ct = default) =>
        Usuarios.InsertOneAsync(usuario, cancellationToken: ct);

    public Task AddProgressAsync(string id, int puntos, int hallazgos, CancellationToken ct = default) =>
        Usuarios.UpdateOneAsync(u => u.Id == id,
            Builders<Usuario>.Update.Inc(u => u.Puntos, puntos).Inc(u => u.HallazgosContador, hallazgos),
            cancellationToken: ct);

    public Task AddLogrosAsync(string id, IReadOnlyCollection<string> logroIds, int puntos, CancellationToken ct = default) =>
        Usuarios.UpdateOneAsync(u => u.Id == id,
            Builders<Usuario>.Update.AddToSetEach(u => u.LogrosDesbloqueados, logroIds).Inc(u => u.Puntos, puntos),
            cancellationToken: ct);

    public Task SetNivelAsync(string id, int nivel, CancellationToken ct = default) =>
        Usuarios.UpdateOneAsync(u => u.Id == id,
            Builders<Usuario>.Update.Set(u => u.Nivel, nivel),
            cancellationToken: ct);
}
