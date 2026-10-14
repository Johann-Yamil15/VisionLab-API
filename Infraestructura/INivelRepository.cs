using VisionLabApi.Models;

namespace VisionLabApi.Infraestructura
{
    public interface INivelRepository
    {
        Task<Nivel?> GetLevelByPointsAsync(int points);
    }
}
