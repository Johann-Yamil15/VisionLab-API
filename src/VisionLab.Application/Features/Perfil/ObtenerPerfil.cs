using VisionLab.Application.Abstractions;
using VisionLab.Domain.Gamificacion;

namespace VisionLab.Application.Features.Perfil;

public sealed record PerfilDto(
    string Id,
    string NombreUsuario,
    int Puntos,
    int HallazgosContador,
    string NivelActual,
    IReadOnlyList<Logro> Logros);

public sealed class ObtenerPerfilHandler(
    ICurrentUser currentUser,
    IUsuarioRepository usuarios,
    ILogroRepository logros,
    INivelRepository niveles)
{
    public const string NivelInicial = "Novato";

    public async Task<PerfilDto?> Handle(CancellationToken ct)
    {
        if (currentUser.UserId is not { } id) return null;

        var usuario = await usuarios.GetByIdAsync(id, ct);
        if (usuario is null) return null;

        var nivel = await niveles.GetByPuntosAsync(usuario.Puntos, ct);
        var desbloqueados = await logros.GetByIdsAsync(usuario.LogrosDesbloqueados, ct);

        return new PerfilDto(usuario.Id, usuario.NombreUsuario, usuario.Puntos,
            usuario.HallazgosContador, nivel?.Titulo ?? NivelInicial, desbloqueados);
    }
}
