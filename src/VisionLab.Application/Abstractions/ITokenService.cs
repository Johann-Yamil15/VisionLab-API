using VisionLab.Domain.Usuarios;

namespace VisionLab.Application.Abstractions;

public interface ITokenService
{
    /// <summary>Crea el JWT de sesión del usuario (el id viaja en el claim "sub").</summary>
    string Crear(Usuario usuario);
}
