using VisionLabApi.Models;

namespace VisionLabApi.Infraestructura
{
    public interface IUsuarioRepository
    {
        Task CreateAsync(Usuario newUser);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByIdAsync(string id);
        Task UpdateLogrosAsync(string id, List<string> newLogrosIds, int pointsFromLogros);
        Task UpdateNivelAsync(string id, int newLevel);
        Task UpdateProgressAsync(string id, int pointsToAdd, int hallazgosCount);
    }
}
