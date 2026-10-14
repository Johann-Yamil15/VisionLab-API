using VisionLabApi.Models;

namespace VisionLabApi.Infraestructura
{
    public interface ILogroRepository
    {
        Task<List<Logro>> GetAllPendingForUserAsync(List<string> unlockedLogroIds);
        Task<List<Logro>> GetLogrosDetailsByIdsAsync(List<string> logroIds);
    }
}
