using VisionLabApi.Models;
using VisionLabApi.Models.VisionLabApi.Models;

namespace VisionLabApi.Infraestructura
{
    public interface IHallazgoService
    {
        Task<Hallazgo> CrearAsync(HallazgoRequest request);
        Task<bool> EliminarAsync(string id);
        Task<Hallazgo?> GetPorIdAsync(string id);
        Task<List<Hallazgo>> GetPorTipoAsync(string tipo);
        Task<List<string>> GetTiposUnicosAsync();
        Task<List<Hallazgo>> GetTodosAsync();
    }
}
