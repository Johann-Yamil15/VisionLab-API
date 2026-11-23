using VisionLab.Domain.Gamificacion;

namespace VisionLab.Application.Features.Gamificacion;

public sealed record NivelDto(int Numero, string Titulo, int Desde)
{
    public static NivelDto De(Nivel n) => new(n.Numero, n.Titulo, n.PuntosRequeridos);
}

/// <summary>Nivel actual, el siguiente y cuánto falta (porcentaje 0–100 dentro del nivel actual).</summary>
public sealed record ProgresoNivelDto(NivelDto Actual, NivelDto? Siguiente, int Porcentaje, int Faltan)
{
    public static ProgresoNivelDto De(IReadOnlyList<Nivel> niveles, int puntos)
    {
        var ordenados = niveles.OrderBy(n => n.PuntosRequeridos).ToList();
        var actual = ordenados.LastOrDefault(n => n.PuntosRequeridos <= puntos) ?? ordenados[0];
        var siguiente = ordenados.FirstOrDefault(n => n.PuntosRequeridos > puntos);
        if (siguiente is null) return new(NivelDto.De(actual), null, 100, 0);

        var tramo = siguiente.PuntosRequeridos - actual.PuntosRequeridos;
        var porcentaje = (int)Math.Round((puntos - actual.PuntosRequeridos) * 100.0 / tramo);
        return new(NivelDto.De(actual), NivelDto.De(siguiente), porcentaje, siguiente.PuntosRequeridos - puntos);
    }
}

/// <summary>
/// Medalla lista para mostrar. <see cref="Imagen"/> ya es la dorada o la bloqueada según el estado;
/// <see cref="Gris"/> indica que no hay imagen bloqueada propia y se muestra la dorada en gris.
/// </summary>
public sealed record MedallaDto(
    string Id, string Nombre, string Reto, int Meta, int Progreso, bool Desbloqueada,
    string Imagen, bool Gris, int Puntos)
{
    public static MedallaDto De(Logro l, Estadisticas e, bool desbloqueada)
    {
        var bloqueadaPropia = !string.IsNullOrEmpty(l.IconoBloqueado);
        return new(l.Clave, l.Nombre, l.Descripcion, l.RequisitoValor,
            Math.Min(l.RequisitoValor, l.ValorDe(e)), desbloqueada,
            desbloqueada || !bloqueadaPropia ? l.Icono : l.IconoBloqueado,
            !desbloqueada && !bloqueadaPropia, l.PuntosOtorgados);
    }
}

/// <summary>Lo que ganó el usuario con un hallazgo nuevo (para las notificaciones del front).</summary>
/// <param name="Veces">Capturas de esta especie contando la nueva; si es mayor que 1 es repetida y no da puntos.</param>
public sealed record ProgresoDto(int PuntosGanados, IReadOnlyList<MedallaDto> MedallasNuevas, NivelDto? NivelNuevo, int Veces = 1);
