using Microsoft.AspNetCore.Mvc;
using VisionLabApi.Dtos;
using VisionLabApi.Infraestructura;

namespace VisionLabApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ruta base: /api/usuarios
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _userRepository;
        private readonly ILogroRepository _logroRepository;
        private readonly INivelRepository _nivelRepository;

        public UsuariosController(
            IUsuarioRepository userRepository,
            ILogroRepository logroRepository,
            INivelRepository nivelRepository)
        {
            _userRepository = userRepository;
            _logroRepository = logroRepository;
            _nivelRepository = nivelRepository;
        }

        // ---------------------------------------------------------------------
        // GET: Obtener Perfil de Gamificación por ID de Usuario
        // ---------------------------------------------------------------------
        [HttpGet]
        // ... (Atributos ProducesResponseType) ...
        public async Task<ActionResult<UsuarioProfileDto>> GetProfile()
        {
            string id = "69204cb755601dce19e87d61";
            var usuario = await _userRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado.");
            }

            var nivelActual = await _nivelRepository.GetLevelByPointsAsync(usuario.Puntos);
            var logrosDetalles = await _logroRepository.GetLogrosDetailsByIdsAsync(usuario.LogrosDesbloqueados);

            var profileDto = new UsuarioProfileDto
            {
                Id = usuario.Id!,
                NombreUsuario = usuario.NombreUsuario ?? "Usuario Desconocido",
                Puntos = usuario.Puntos,
                HallazgosContador = usuario.HallazgosContador,
                NivelActual = nivelActual?.Titulo ?? "Novato",
                Logros = logrosDetalles
            };

            return Ok(profileDto);
        }
    }
}
