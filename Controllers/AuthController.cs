using Microsoft.AspNetCore.Mvc;
using VisionLabApi.Models;
using VisionLabApi.Dtos;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Net; // Para usar HttpStatusCode

namespace VisionLabApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<Usuario> _usuariosCollection;

        public AuthController(
            IMongoClient mongoClient,
            IOptions<VisionLabDatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _usuariosCollection = database.GetCollection<Usuario>(
                settings.Value.Collections.Usuarios
            );
        }

        // ---------------------------------------------------------------------
        // POST: /api/auth/register
        // ---------------------------------------------------------------------
        /// <summary>
        /// Registra un nuevo usuario en la base de datos
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Usuario))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // 1. Verificar si el usuario ya existe por Email
            var existingUser = await _usuariosCollection.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                
                return BadRequest(new { message = "El email ya está registrado." });
            }

            // 2. Crear el nuevo modelo de Usuario
            var newUser = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                Email = dto.Email,
                Contraseña = dto.Contraseña, // Almacenamiento en texto plano (según solicitud)
                Puntos = 0, // Inicia con 0 puntos
                Nivel = 1,  // Inicia en Nivel 1
                HallazgosContador = 0
            };

            // 3. Guardar en la base de datos
            await _usuariosCollection.InsertOneAsync(newUser);

            // 4. Retornar el usuario creado (sin la contraseña)
            newUser.Contraseña = string.Empty;
            return StatusCode((int)HttpStatusCode.Created, newUser);
        }

        // ---------------------------------------------------------------------
        // POST: /api/auth/login
        // ---------------------------------------------------------------------
        /// <summary>
        /// Inicia sesión y verifica las credenciales del usuario
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Usuario))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // 1. Buscar el usuario por Email
            var usuario = await _usuariosCollection
                .Find(u => u.Email == dto.Email)
                .FirstOrDefaultAsync();

            // 2. Verificar existencia y contraseña
            if (usuario == null || usuario.Contraseña != dto.Contraseña)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            // 3. Éxito: Retornar el usuario (sin la contraseña)
            usuario.Contraseña = string.Empty;
            return Ok(usuario);
        }
    }
}