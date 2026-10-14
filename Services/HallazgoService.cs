namespace VisionLabApi.Services
{
    using global::VisionLabApi.Infraestructura;
    using global::VisionLabApi.Models;
    using global::VisionLabApi.Models.VisionLabApi.Models;
    using Microsoft.Extensions.Options;
    using MongoDB.Bson; // Necesario para BsonRegularExpression
    using MongoDB.Driver;

    namespace VisionLabApi.Services
    {
        public class HallazgoService : IHallazgoService
        {
            // El servicio ES el que conoce y usa la colección de MongoDB
            private readonly IMongoCollection<Hallazgo> _hallazgosCollection;
            private readonly IGamificationService _gamificationService;

            public HallazgoService(
                IMongoClient mongoClient,
                IOptions<VisionLabDatabaseSettings> settings,
                IGamificationService gamificationService)
            {
                // Toda la lógica de inicialización de la colección va aquí
                var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
                _hallazgosCollection = database.GetCollection<Hallazgo>(
                    settings.Value.Collections.Hallazgos
                );
                _gamificationService = gamificationService;
            }

            // ---------------------------------------------------------------------
            // Implementación de los métodos
            // ---------------------------------------------------------------------

            public async Task<List<Hallazgo>> GetTodosAsync()
            {
                return await _hallazgosCollection.Find(_ => true).ToListAsync();
            }

            public async Task<Hallazgo?> GetPorIdAsync(string id)
            {
                return await _hallazgosCollection.Find(h => h.Id == id).FirstOrDefaultAsync();
            }

            public async Task<Hallazgo> CrearAsync(HallazgoRequest request)
            {
                // Lógica de Negocio: Construcción del Hallazgo a partir del DTO
                var nuevoHallazgo = new Hallazgo
                {
                    Tipo = request.Tipo,
                    NombreComun = request.NombreComun,
                    NombreCientifico = request.NombreCientifico,
                    Descripcion = request.Descripcion,
                    NivelConfianza = request.NivelConfianza,
                    FechaCaptura = DateTime.UtcNow,
                    ImagenBase64 = request.ImagenBase64,
                    //UsuarioId = request.UsuarioId ?? "anonymous"
                    UsuarioId = "69204cb755601dce19e87d61"
                };

                // 🔥 AQUI ESTABA EL ERROR: REEMPLAZA LA LÍNEA INVÁLIDA 🔥
                await _hallazgosCollection.InsertOneAsync(nuevoHallazgo); // AHORA USA LA COLECCIÓN QUE SÍ EXISTE

                // 🔥 LLAMADA AL SERVICIO DE GAMIFICACIÓN 🔥
                if (nuevoHallazgo.UsuarioId != "anonymous" && nuevoHallazgo.UsuarioId != null)
                {
                    await _gamificationService.CheckProgressAndAwardAsync(nuevoHallazgo.UsuarioId, nuevoHallazgo);
                }

                return nuevoHallazgo;
            }


            public async Task<bool> EliminarAsync(string id)
            {
                var result = await _hallazgosCollection.DeleteOneAsync(h => h.Id == id);
                // Retornamos true si se eliminó al menos un documento
                return result.DeletedCount > 0;
            }

            public async Task<List<Hallazgo>> GetPorTipoAsync(string tipo)
            {
                // Lógica de MongoDB (Filtro Regex) movida desde el controlador
                var filter = Builders<Hallazgo>.Filter.Regex(
                    h => h.Tipo,
                    new BsonRegularExpression($"^{tipo}$", "i")
                );
                return await _hallazgosCollection.Find(filter).ToListAsync();
            }

            public async Task<List<string>> GetTiposUnicosAsync()
            {
                // Lógica de MongoDB (Distinct) movida desde el controlador
                var tipos = await _hallazgosCollection
                    .Distinct<string>("Tipo", Builders<Hallazgo>.Filter.Empty)
                    .ToListAsync();

                return tipos.OrderBy(t => t).ToList();
            }
        }
    }
}
