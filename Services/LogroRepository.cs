using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VisionLabApi.Infraestructura;
using VisionLabApi.Models;

namespace VisionLabApi.Services
{
    public class LogroRepository : ILogroRepository
    {
        private readonly IMongoCollection<Logro> _logrosCollection;

        public LogroRepository(IMongoClient mongoClient, IOptions<VisionLabDatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _logrosCollection = database.GetCollection<Logro>(settings.Value.Collections.Logros);
        }

        /// <summary>
        /// Obtiene todos los logros que un usuario AÚN NO ha desbloqueado.
        /// </summary>
        public async Task<List<Logro>> GetAllPendingForUserAsync(List<string> unlockedLogroIds)
        {
            // Filtro: Trae los logros cuyo ID NO está en la lista de IDs ya desbloqueados.
            var filter = Builders<Logro>.Filter.Nin(l => l.Id, unlockedLogroIds);
            return await _logrosCollection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Obtiene los detalles de los logros que un usuario ya desbloqueó (para el perfil).
        /// </summary>
        public async Task<List<Logro>> GetLogrosDetailsByIdsAsync(List<string> logroIds)
        {
            // Filtro: Trae los logros cuyo ID SÍ está en la lista de IDs proporcionada.
            var filter = Builders<Logro>.Filter.In(l => l.Id, logroIds);
            return await _logrosCollection.Find(filter).ToListAsync();
        }
    }
}