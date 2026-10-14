using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VisionLabApi.Infraestructura;
using VisionLabApi.Models;

namespace VisionLabApi.Services
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _usuariosCollection;

        public UsuarioRepository(IMongoClient mongoClient, IOptions<VisionLabDatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _usuariosCollection = database.GetCollection<Usuario>(settings.Value.Collections.Usuarios);
        }

        public async Task<Usuario?> GetByIdAsync(string id)
        {
            return await _usuariosCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtiene un usuario por su dirección de correo electrónico (usado para el Login).
        /// </summary>
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _usuariosCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        // --- MÉTODOS DE ESCRITURA (Registro y Gamificación) ---

        /// <summary>
        /// Crea un nuevo usuario en la base de datos (usado para el Registro).
        /// </summary>
        public async Task CreateAsync(Usuario newUser)
        {
            await _usuariosCollection.InsertOneAsync(newUser);
        }
        public async Task UpdateProgressAsync(string id, int pointsToAdd, int hallazgosCount)
        {
            var updateDefinition = Builders<Usuario>.Update
                .Inc(u => u.Puntos, pointsToAdd)
                .Inc(u => u.HallazgosContador, hallazgosCount);

            await _usuariosCollection.UpdateOneAsync(u => u.Id == id, updateDefinition);
        }

        public async Task UpdateLogrosAsync(string id, List<string> newLogrosIds, int pointsFromLogros)
        {
            var update = Builders<Usuario>.Update
                .PushEach(u => u.LogrosDesbloqueados, newLogrosIds)
                .Inc(u => u.Puntos, pointsFromLogros);

            await _usuariosCollection.UpdateOneAsync(u => u.Id == id, update);
        }

        public async Task UpdateNivelAsync(string id, int newLevel)
        {
            await _usuariosCollection.UpdateOneAsync(
                u => u.Id == id,
                Builders<Usuario>.Update.Set(u => u.Nivel, newLevel)
            );
        }
    }
}