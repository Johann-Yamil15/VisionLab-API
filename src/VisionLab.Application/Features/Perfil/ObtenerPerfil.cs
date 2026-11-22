using VisionLab.Application.Abstractions;
using VisionLab.Application.Features.Gamificacion;
using VisionLab.Domain.Gamificacion;

namespace VisionLab.Application.Features.Perfil;

/// <param name="NivelActual">Título del nivel (se conserva por compatibilidad).</param>
/// <param name="Descubrimientos">Hallazgos totales, incluidos los borrados.</param>
public sealed record PerfilDto(
    string Id,
    string NombreUsuario,
    int Puntos,
    int HallazgosContador,
    string NivelActual,
    ProgresoNivelDto Nivel,
    int Descubrimientos,
    IReadOnlyList<MedallaDto> Medallas);

public sealed class ObtenerPerfilHandler(
    ICurrentUser currentUser,
    IUsuarioRepository usuarios,
    IHallazgoRepository hallazgos,
    ILogroRepository logros,
    INivelRepository niveles)
{
    public async Task<PerfilDto?> Handle(CancellationToken ct)
    {
        if (currentUser.UserId is not { } id) return null;

        var usuario = await usuarios.GetByIdAsync(id, ct);
        if (usuario is null) return null;

        var stats = Estadisticas.De(await hallazgos.GetByUsuarioAsync(id, ct), usuario.HallazgosContador);
        var listaNiveles = await niveles.GetAllAsync(ct);
        var nivel = listaNiveles.Count > 0
            ? ProgresoNivelDto.De(listaNiveles, usuario.Puntos)
            : ProgresoNivelDto.De(Catalogo.Niveles, usuario.Puntos); // por si aún no se sembró la base

        // Desbloqueada si ya se otorgó o si el usuario ya cumple el reto (hallazgos anteriores al catálogo).
        var medallas = (await logros.GetAllAsync(ct))
            .OrderBy(l => Array.FindIndex(Catalogo.Logros, c => c.Clave == l.Clave))
            .Select(l => MedallaDto.De(l, stats,
                usuario.LogrosDesbloqueados.Contains(l.Id) || l.EstaCumplidoPor(stats)))
            .ToList();

        return new PerfilDto(usuario.Id, usuario.NombreUsuario, usuario.Puntos, usuario.HallazgosContador,
            nivel.Actual.Titulo, nivel, stats.Total, medallas);
    }
}
