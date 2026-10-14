using System.ComponentModel.DataAnnotations;

namespace VisionLabApi.Dtos
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Contraseña { get; set; } = null!;
    }
}