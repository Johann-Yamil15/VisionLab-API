using VisionLab.Application.Abstractions;
using VisionLab.Domain.Gamificacion;
using VisionLab.Domain.Hallazgos;

namespace VisionLab.Application.Features.Gamificacion;

/// <summary>
/// Suma puntos por un hallazgo nuevo, desbloquea las medallas cumplidas y sube de nivel.
/// Devuelve lo ganado para que el front muestre las notificaciones.
/// </summary>
public sealed class OtorgarProgresoHandler(
    IUsuarioRepository usuarios,
    IHallazgoRepository hallazgos,
    ILogroRepository logros,
    INivelRepository niveles)
{
    public async Task<ProgresoDto> Handle(string usuarioId, Hallazgo hallazgo, CancellationToken ct)
    {
        // La lista ya incluye el hallazgo recién guardado.
        var todos = await hallazgos.GetByUsuarioAsync(usuarioId, ct);
        var clave = Estadisticas.ClaveEspecie(hallazgo);
        var veces = Math.Max(1, todos.Count(h => Estadisticas.ClaveEspecie(h) == clave));

        // Repetida: la foto se guarda en su galería pero no da puntos (evita farmear XP).
        var puntos = veces > 1 ? 0 : ReglasDePuntos.PorHallazgo(hallazgo);
        await usuarios.AddProgressAsync(usuarioId, puntos, 1, ct);

        var usuario = await usuarios.GetByIdAsync(usuarioId, ct);
        if (usuario is null) return new ProgresoDto(puntos, [], null, veces);

        var stats = Estadisticas.De(todos, usuario.HallazgosContador);
        var nuevas = (await logros.GetPendientesAsync(usuario.LogrosDesbloqueados, ct))
            .Where(l => l.EstaCumplidoPor(stats))
            .ToList();

        if (nuevas.Count > 0)
        {
            var extra = nuevas.Sum(l => l.PuntosOtorgados);
            await usuarios.AddLogrosAsync(usuario.Id, nuevas.Select(l => l.Id).ToList(), extra, ct);
            usuario.Puntos += extra;
            puntos += extra;
        }

        NivelDto? nivelNuevo = null;
        var nivel = await niveles.GetByPuntosAsync(usuario.Puntos, ct);
        if (nivel is not null && nivel.Numero > usuario.Nivel)
        {
            await usuarios.SetNivelAsync(usuario.Id, nivel.Numero, ct);
            nivelNuevo = NivelDto.De(nivel);
        }

        return new ProgresoDto(puntos, nuevas.Select(l => MedallaDto.De(l, stats, true)).ToList(), nivelNuevo, veces);
    }
}
