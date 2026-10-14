namespace VisionLabApi.Models
{
    /// <summary>
    /// Modelo de datos para crear un nuevo hallazgo
    /// </summary>
    public class HallazgoRequest
    {
        /// <summary>
        /// Tipo de hallazgo (ej: Insecto, Planta, Ave)
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>
        /// Nombre común del espécimen
        /// </summary>
        public string NombreComun { get; set; } = string.Empty;

        /// <summary>
        /// Nombre científico del espécimen
        /// </summary>
        public string NombreCientifico { get; set; } = string.Empty;

        /// <summary>
        /// Descripción detallada del hallazgo
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Nivel de confianza del análisis (0-100)
        /// </summary>
        public int NivelConfianza { get; set; }

        /// <summary>
        /// Imagen en formato Base64 (opcional)
        /// </summary>
        public string? ImagenBase64 { get; set; }

        /// <summary>
        /// ID del usuario que registra el hallazgo (opcional)
        /// </summary>
        public string? UsuarioId { get; set; }
    }
}
