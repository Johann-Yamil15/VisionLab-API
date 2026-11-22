namespace VisionLab.Application.Features.Ia;

/// <summary>
/// Las 12 categorías de la carta. Deben coincidir con CLAVES_IA de
/// vision-lab/src/app/shared/utils/tipos.ts (si cambian allá, actualizar aquí).
/// </summary>
public static class Categorias
{
    public static readonly (string Etiqueta, string Guia)[] Guias =
    [
        ("Insectos", "insectos y otros artrópodos: escarabajos, mariquitas, mariposas, orugas, hormigas, abejas, moscas, grillos, arañas, ciempiés, cochinillas."),
        ("Plantas", "plantas y sus partes: hojas, flores, ramas, cortezas, semillas, frutos, árboles, pasto, musgo, cactus."),
        ("Vida acuática", "vida del agua: peces, ranas, renacuajos, algas, conchas, cangrejos, plantas y animales de ríos, estanques o mar."),
        ("Tierra y suelos", "suelo y lo que vive en él: tierra, arena, lodo, composta, lombrices, caracoles de tierra, huellas; también lagartijas, mamíferos y otros animales terrestres que no sean aves ni insectos."),
        ("Rocas y minerales", "piedras, rocas, guijarros, cristales naturales, minerales y fósiles."),
        ("Metales y materiales", "materiales: metales, clavos, latas, monedas, plástico, vidrio, tela, papel, cartón."),
        ("Aves y cielo", "aves, plumas, nidos, huevos de ave, nubes y el cielo."),
        ("Luz y fenómenos", "fenómenos de luz y clima: arcoíris, sombras, reflejos, el sol, rayos, rocío, escarcha, hielo."),
        ("Hongos", "hongos, setas, champiñones, líquenes y moho."),
        ("Microscópico", "cosas vistas muy de cerca o a escala microscópica: células, polen, bacterias, texturas muy ampliadas."),
        ("Objetos", "objetos hechos por personas y tecnología: juguetes, herramientas, utensilios, aparatos, circuitos."),
        ("Química", "sustancias y mezclas: sal, azúcar, líquidos, burbujas, colorantes, cristales de sal o azúcar, reacciones."),
    ];

    public static readonly string[] Todas = Guias.Select(g => g.Etiqueta).ToArray();
}
