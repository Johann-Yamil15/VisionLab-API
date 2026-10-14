using VisionLabApi.Models;
using VisionLabApi.Models.VisionLabApi.Models;

namespace VisionLabApi.Infraestructura
{
    public interface IGamificationService
    {
        Task<int> CalculateBasePoints(Hallazgo hallazgo);
        Task<int> CheckForLevelUp(Usuario usuario);
        Task CheckProgressAndAwardAsync(string userId, Hallazgo newHallazgo);
    }
}
