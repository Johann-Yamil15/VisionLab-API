using System.ComponentModel.DataAnnotations;

namespace VisionLabApi.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string NombreUsuario { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Contraseña { get; set; } = null!;
    }
}