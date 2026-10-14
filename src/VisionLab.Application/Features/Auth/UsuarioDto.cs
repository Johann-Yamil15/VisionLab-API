using VisionLab.Domain.Usuarios;

namespace VisionLab.Application.Features.Auth;

/// <summary>Usuario expuesto al cliente (nunca incluye la contraseña).</summary>
public sealed record UsuarioDto(
    string Id,
    string NombreUsuario,
    string Email,
    int Puntos,
    int Nivel,
    int HallazgosContador,
    IReadOnlyList<string> LogrosDesbloqueados)
{
    public static UsuarioDto From(Usuario u) =>
        new(u.Id, u.NombreUsuario, u.Email, u.Puntos, u.Nivel, u.HallazgosContador, u.LogrosDesbloqueados);
}
