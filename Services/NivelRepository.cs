using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VisionLabApi.Infraestructura;
using VisionLabApi.Models;

namespace VisionLabApi.Services
{
    public class NivelRepository : INivelRepository
    {
        private readonly IMongoCollection<Nivel> _nivelesCollection;

        public NivelRepository(IMongoClient mongoClient, IOptions<VisionLabDatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _nivelesCollection = database.GetCollection<Nivel>(settings.Value.Collections.Niveles);
        }

        /// <summary>
        /// Obtiene el nivel más alto que el usuario ha alcanzado con sus puntos actuales.
        /// </summary>
        public async Task<Nivel?> GetLevelByPointsAsync(int points)
        {
            // Filtro: Puntos Requeridos deben ser menores o iguales a los puntos del usuario.
            // Ordena Descendentemente y toma el primero (el nivel más alto alcanzado).
            return await _nivelesCollection
                .Find(n => n.PuntosRequeridos <= points)
                .SortByDescending(n => n.Numero)
                .FirstOrDefaultAsync();
        }
    }
}