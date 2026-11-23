using VisionLab.Domain.Hallazgos;

namespace VisionLab.Domain.Gamificacion;

/// <summary>Lo que miden las medallas, calculado con los hallazgos del usuario.</summary>
/// <param name="Total">Hallazgos totales (el contador del usuario incluye los borrados).</param>
/// <param name="Especies">Especies distintas.</param>
/// <param name="Categorias">Categorías distintas (sin contar "Otro").</param>
/// <param name="Fauna">Especies de animales: insectos, aves y vida acuática.</param>
public sealed record Estadisticas(int Total, int Especies, int Categorias, int Fauna)
{
    private static readonly string[] CategoriasFauna = ["Insectos", "Aves y cielo", "Vida acuática"];

    public static Estadisticas De(IReadOnlyCollection<Hallazgo> hallazgos, int contador)
    {
        var especies = hallazgos.GroupBy(ClaveEspecie).Select(g => g.First()).ToList();
        return new Estadisticas(
            Total: Math.Max(contador, hallazgos.Count),
            Especies: especies.Count,
            Categorias: especies
                .Select(h => h.Tipo.Trim())
                .Where(t => t.Length > 0 && !t.Equals("Otro", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count(),
            Fauna: especies.Count(h => CategoriasFauna.Contains(h.Tipo.Trim(), StringComparer.OrdinalIgnoreCase)));
    }

    /// <summary>Misma especie = mismo nombre científico (o común si no tiene), sin importar mayúsculas.</summary>
    public static string ClaveEspecie(Hallazgo h) =>
        (string.IsNullOrWhiteSpace(h.NombreCientifico) ? h.NombreComun : h.NombreCientifico)
        .Trim().ToLowerInvariant();
}
