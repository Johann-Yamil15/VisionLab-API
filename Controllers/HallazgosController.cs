//using Microsoft.AspNetCore.Mvc;
//using VisionLabApi.Models;
//using MongoDB.Driver;
//using Microsoft.Extensions.Options;
//using VisionLabApi.Models.VisionLabApi.Models;

//namespace VisionLabApi.Controllers
//{
//    /// <summary>
//    /// Controlador para la gestión de hallazgos de VisionLab
//    /// </summary>
//    [ApiController]
//    [Route("api/[controller]")]
//    public class HallazgosController : ControllerBase
//    {
//        private readonly IMongoCollection<Hallazgo> _hallazgosCollection;

//        public HallazgosController(
//            IMongoClient mongoClient,
//            IOptions<VisionLabDatabaseSettings> settings)
//        {
//            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
//            _hallazgosCollection = database.GetCollection<Hallazgo>(
//                settings.Value.Collections.Hallazgos
//            );
//        }

//        // ---------------------------------------------------------------------
//        // GET: Obtener todos los hallazgos
//        // ---------------------------------------------------------------------
//        /// <summary>
//        /// Obtiene todos los hallazgos registrados
//        /// </summary>
//        /// <returns>Lista de hallazgos</returns>
//        [HttpGet]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        public async Task<List<Hallazgo>> Get()
//        {
//            return await _hallazgosCollection.Find(_ => true).ToListAsync();
//        }

//        // ---------------------------------------------------------------------
//        // GET: Obtener un hallazgo específico por ID
//        // ---------------------------------------------------------------------
//        /// <summary>
//        /// Obtiene un hallazgo específico por su ID
//        /// </summary>
//        /// <param name="id">ID del hallazgo (24 caracteres)</param>
//        /// <returns>El hallazgo solicitado</returns>
//        [HttpGet("{id:length(24)}", Name = "GetHallazgo")]
//        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Hallazgo))]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<ActionResult<Hallazgo>> Get(string id)
//        {
//            var hallazgo = await _hallazgosCollection.Find(h => h.Id == id).FirstOrDefaultAsync();

//            if (hallazgo == null)
//            {
//                return NotFound();
//            }

//            return hallazgo;
//        }

//        // ---------------------------------------------------------------------
//        // POST: Crear un nuevo hallazgo (solo datos de Gemini)
//        // ---------------------------------------------------------------------
//        /// <summary>
//        /// Crea un nuevo hallazgo con los datos del análisis de imagen
//        /// </summary>
//        /// <param name="request">Datos del análisis de Gemini</param>
//        /// <returns>El hallazgo creado</returns>
//        [HttpPost]
//        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Hallazgo))]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public async Task<IActionResult> Post([FromBody] HallazgoRequest request)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            // Crear el hallazgo completo con los datos recibidos
//            var nuevoHallazgo = new Hallazgo
//            {
//                Tipo = request.Tipo,
//                NombreComun = request.NombreComun,
//                NombreCientifico = request.NombreCientifico,
//                Descripcion = request.Descripcion,
//                NivelConfianza = request.NivelConfianza,
//                FechaCaptura = DateTime.UtcNow,
//                ImagenBase64 = request.ImagenBase64, // Si se envía desde el frontend
//                UsuarioId = request.UsuarioId ?? "anonymous" // Usuario por defecto si no se envía
//            };

//            await _hallazgosCollection.InsertOneAsync(nuevoHallazgo);

//            return CreatedAtRoute("GetHallazgo", new { id = nuevoHallazgo.Id }, nuevoHallazgo);
//        }

//        // ---------------------------------------------------------------------
//        // DELETE: Eliminar un hallazgo por ID
//        // ---------------------------------------------------------------------
//        /// <summary>
//        /// Elimina un hallazgo por su ID
//        /// </summary>
//        /// <param name="id">ID del hallazgo a eliminar</param>
//        [HttpDelete("{id:length(24)}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> Delete(string id)
//        {
//            var hallazgo = await _hallazgosCollection.Find(h => h.Id == id).FirstOrDefaultAsync();

//            if (hallazgo == null)
//            {
//                return NotFound();
//            }

//            await _hallazgosCollection.DeleteOneAsync(h => h.Id == id);
//            return NoContent();
//        }
//        // ---------------------------------------------------------------------
//        // GET: Obtener hallazgos filtrados por tipo
//        // Ruta: GET /api/hallazgos/tipo/{tipo}
//        // ---------------------------------------------------------------------
//        /// <summary>
//        /// Obtiene todos los hallazgos de un tipo específico
//        /// </summary>
//        /// <param name="tipo">Tipo de hallazgo (ej: Insecto, Planta, Ave, Hoja, Roca, Hongo, Rama, Objeto Artificial)</param>
//        /// <returns>Lista de hallazgos del tipo especificado</returns>
//        [HttpGet("tipo/{tipo}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        public async Task<ActionResult<List<Hallazgo>>> GetByTipo(string tipo)
//        {
//            // Filtro case-insensitive para buscar por tipo
//            var filter = Builders<Hallazgo>.Filter.Regex(
//                h => h.Tipo,
//                new MongoDB.Bson.BsonRegularExpression($"^{tipo}$", "i")
//            );

//            var hallazgos = await _hallazgosCollection.Find(filter).ToListAsync();

//            return Ok(hallazgos);
//        }

//        // ---------------------------------------------------------------------
//        // GET: Obtener todos los tipos únicos de hallazgos
//        // Ruta: GET /api/hallazgos/tipos/unique
//        // ---------------------------------------------------------------------
//        /// <summary>
//        /// Obtiene todos los tipos únicos de hallazgos registrados
//        /// </summary>
//        /// <returns>Lista de tipos únicos</returns>
//        [HttpGet("tipos/unique")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        public async Task<ActionResult<List<string>>> GetTiposUnicos()
//        {
//            var tipos = await _hallazgosCollection
//                .Distinct<string>("Tipo", Builders<Hallazgo>.Filter.Empty)
//                .ToListAsync();

//            return Ok(tipos.OrderBy(t => t).ToList());
//        }
//    }



//}



using Microsoft.AspNetCore.Mvc;
using VisionLabApi.Models;
using VisionLabApi.Services; // Usamos el servicio
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using VisionLabApi.Models.VisionLabApi.Models;
using VisionLabApi.Infraestructura; // Si este using es necesario para HallazgoRequest

namespace VisionLabApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HallazgosController : ControllerBase
    {
        // 1. Reemplazamos IMongoCollection por la interfaz del Servicio
        private readonly IHallazgoService _hallazgoService;

        // 2. Modificamos el constructor para inyectar el Servicio
        public HallazgosController(IHallazgoService hallazgoService)
        {
            _hallazgoService = hallazgoService;
        }

        // ---------------------------------------------------------------------
        // GET: Obtener todos los hallazgos
        // ---------------------------------------------------------------------
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<Hallazgo>> Get()
        {
            // Delegamos al servicio
            return await _hallazgoService.GetTodosAsync();
        }

        // ---------------------------------------------------------------------
        // GET: Obtener un hallazgo específico por ID
        // ---------------------------------------------------------------------
        [HttpGet("{id:length(24)}", Name = "GetHallazgo")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Hallazgo))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Hallazgo>> Get(string id)
        {
            var hallazgo = await _hallazgoService.GetPorIdAsync(id);

            if (hallazgo == null)
            {
                return NotFound();
            }

            return hallazgo;
        }

        // ---------------------------------------------------------------------
        // POST: Crear un nuevo hallazgo
        // ---------------------------------------------------------------------
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Hallazgo))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] HallazgoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Delegamos la creación al servicio
            var nuevoHallazgo = await _hallazgoService.CrearAsync(request);

            return CreatedAtRoute("GetHallazgo", new { id = nuevoHallazgo.Id }, nuevoHallazgo);
        }

        // ---------------------------------------------------------------------
        // DELETE: Eliminar un hallazgo por ID
        // ---------------------------------------------------------------------
        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            // Delegamos la eliminación al servicio
            var result = await _hallazgoService.EliminarAsync(id);

            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // ---------------------------------------------------------------------
        // GET: Obtener hallazgos filtrados por tipo
        // ---------------------------------------------------------------------
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Hallazgo>>> GetByTipo(string tipo)
        {
            // Delegamos la búsqueda por tipo al servicio
            var hallazgos = await _hallazgoService.GetPorTipoAsync(tipo);
            return Ok(hallazgos);
        }

        // ---------------------------------------------------------------------
        // GET: Obtener todos los tipos únicos de hallazgos
        // ---------------------------------------------------------------------
        [HttpGet("tipos/unique")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<string>>> GetTiposUnicos()
        {
            // Delegamos la obtención de tipos únicos al servicio
            var tipos = await _hallazgoService.GetTiposUnicosAsync();
            return Ok(tipos);
        }
    }


}