using VisionLabApi.Dtos;
using VisionLabApi.Infraestructura;
using VisionLabApi.Models;

namespace VisionLabApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _userRepository;

        public AuthService(IUsuarioRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Usuario?> RegisterAsync(RegisterDto dto)
        {
            // 1. Verificar si el usuario ya existe por Email (Usando el Repositorio)
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                // Devolvemos null para indicar que el registro falló por duplicidad
                return null;
            }

            // 2. Crear el nuevo modelo de Usuario
            var newUser = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                Email = dto.Email,
                Contraseña = dto.Contraseña, // NOTA: Deberías hashear la contraseña aquí.
                Puntos = 0,
                Nivel = 1,
                HallazgosContador = 0
            };

            // 3. Guardar en la base de datos (Usando el Repositorio)
            await _userRepository.CreateAsync(newUser);
            return newUser;
        }

        public async Task<Usuario?> LoginAsync(LoginDto dto)
        {
            // 1. Buscar el usuario por Email (Usando el Repositorio)
            var usuario = await _userRepository.GetByEmailAsync(dto.Email);

            // 2. Verificar existencia y contraseña
            if (usuario == null || usuario.Contraseña != dto.Contraseña)
            {
                return null; // Credenciales inválidas
            }

            // 3. Éxito
            return usuario;
        }
    }
}