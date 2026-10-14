using System.Collections.Generic;
using VisionLabApi.Models; // Necesario para hacer referencia a la clase Logro

namespace VisionLabApi.Dtos
{
    /// <summary>
    /// Modelo de datos para mostrar el perfil público de un usuario, incluyendo su estado de Gamificación.
    /// Este DTO se usa para enviar datos de la API al frontend.
    /// </summary>
    public class UsuarioProfileDto
    {
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de usuario o nombre visible.
        /// </summary>
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Puntos totales acumulados por el usuario.
        /// </summary>
        public int Puntos { get; set; }

        /// <summary>
        /// Total de hallazgos que el usuario ha subido.
        /// </summary>
        public int HallazgosContador { get; set; }

        /// <summary>
        /// Título del nivel actual (ej: "Novato", "Explorador").
        /// </summary>
        public string NivelActual { get; set; } = string.Empty;

        /// <summary>
        /// Lista de logros desbloqueados. Se espera que la clase Logro contenga Nombre, Descripción, etc.
        /// </summary>
        public List<Logro> Logros { get; set; } = new List<Logro>();
    }
}