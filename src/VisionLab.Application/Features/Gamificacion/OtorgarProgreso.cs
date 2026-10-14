using VisionLab.Application.Abstractions;
using VisionLab.Domain.Gamificacion;
using VisionLab.Domain.Hallazgos;

namespace VisionLab.Application.Features.Gamificacion;

/// <summary>
/// Suma puntos por un hallazgo nuevo y desbloquea los logros y niveles alcanzados.
/// </summary>
public sealed class OtorgarProgresoHandler(
    IUsuarioRepository usuarios,
    ILogroRepository logros,
    INivelRepository niveles)
{
    public async Task Handle(string usuarioId, Hallazgo hallazgo, CancellationToken ct)
    {
        await usuarios.AddProgressAsync(usuarioId, ReglasDePuntos.PorHallazgo(hallazgo), 1, ct);

        var usuario = await usuarios.GetByIdAsync(usuarioId, ct);
        if (usuario is null) return;

        var nuevos = (await logros.GetPendientesAsync(usuario.LogrosDesbloqueados, ct))
            .Where(l => l.EstaCumplidoPor(usuario))
            .ToList();

        if (nuevos.Count > 0)
        {
            var puntosExtra = nuevos.Sum(l => l.PuntosOtorgados);
            await usuarios.AddLogrosAsync(usuario.Id, nuevos.Select(l => l.Id).ToList(), puntosExtra, ct);
            usuario.Puntos += puntosExtra;
        }

        var nivel = await niveles.GetByPuntosAsync(usuario.Puntos, ct);
        if (nivel is not null && nivel.Numero > usuario.Nivel)
            await usuarios.SetNivelAsync(usuario.Id, nivel.Numero, ct);
    }
}
