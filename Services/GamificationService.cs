// Services/GamificationService.cs
using VisionLabApi.Models;
using VisionLabApi.Infraestructura;
using VisionLabApi.Models.VisionLabApi.Models;

namespace VisionLabApi.Services
{
    public class GamificationService : IGamificationService
    {
        private readonly IUsuarioRepository _userRepository;
        private readonly ILogroRepository _logroRepository;
        private readonly INivelRepository _nivelRepository;

        public GamificationService(
            IUsuarioRepository userRepository,
            ILogroRepository logroRepository,
            INivelRepository nivelRepository)
        {
            _userRepository = userRepository;
            _logroRepository = logroRepository;
            _nivelRepository = nivelRepository;
        }

        public Task<int> CalculateBasePoints(Hallazgo hallazgo)
        {
            // 10 puntos base + bonus de confianza (ej: 1 punto por cada 10% de confianza)
            int points = 10 + (int)Math.Round(hallazgo.NivelConfianza / 10.0);
            return Task.FromResult(points);
        }

        public async Task CheckProgressAndAwardAsync(string userId, Hallazgo newHallazgo)
        {
            if (userId == "anonymous") return;

            int points = await CalculateBasePoints(newHallazgo);

            // 1. Actualizar puntos y contador
            await _userRepository.UpdateProgressAsync(userId, points, 1);

            // Obtener el usuario actualizado para verificar logros
            var usuario = await _userRepository.GetByIdAsync(userId);
            if (usuario == null) return;

            // 2. Verificar Logros
            await CheckForLogros(usuario);

            // 3. Verificar Nivel
            await CheckForLevelUp(usuario);
        }

        private async Task CheckForLogros(Usuario usuario)
        {
            var logrosPendientes = await _logroRepository.GetAllPendingForUserAsync(usuario.LogrosDesbloqueados);

            List<string> newLogrosIds = new List<string>();
            int puntosPorLogros = 0;

            foreach (var logro in logrosPendientes)
            {
                // Lógica de verificación:
                if (logro.RequisitoTipo == "HallazgosTotales" && usuario.HallazgosContador >= logro.RequisitoValor)
                {
                    newLogrosIds.Add(logro.Id!);
                    puntosPorLogros += logro.PuntosOtorgados;
                }
                // ... (Agregar lógica para otros tipos de logro aquí)
            }

            if (newLogrosIds.Any())
            {
                await _userRepository.UpdateLogrosAsync(usuario.Id!, newLogrosIds, puntosPorLogros);
            }
        }

        public async Task<int> CheckForLevelUp(Usuario usuario)
        {
            var nivelAlcanzado = await _nivelRepository.GetLevelByPointsAsync(usuario.Puntos);

            if (nivelAlcanzado != null && usuario.Nivel < nivelAlcanzado.Numero)
            {
                await _userRepository.UpdateNivelAsync(usuario.Id!, nivelAlcanzado.Numero);
                return nivelAlcanzado.Numero;
            }
            return usuario.Nivel;
        }
    }
}