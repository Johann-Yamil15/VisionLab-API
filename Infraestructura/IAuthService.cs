using VisionLabApi.Dtos;
using VisionLabApi.Models;

namespace VisionLabApi.Infraestructura
{
    public interface IAuthService
    {
        Task<Usuario?> RegisterAsync(RegisterDto dto);
        Task<Usuario?> LoginAsync(LoginDto dto);
    }
}
