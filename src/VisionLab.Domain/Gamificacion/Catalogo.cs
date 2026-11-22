namespace VisionLab.Domain.Gamificacion;

/// <summary>
/// Niveles y medallas oficiales (documento técnico, sección 5.2.2). Se guardan en Mongo al
/// arrancar la API; cambiar algo aquí lo actualiza en la base en el siguiente arranque.
/// </summary>
public static class Catalogo
{
    public static readonly Nivel[] Niveles =
    [
        new() { Numero = 1, Titulo = "Cadete de Campo", PuntosRequeridos = 0 },
        new() { Numero = 2, Titulo = "Analista Táctico", PuntosRequeridos = 500 },
        new() { Numero = 3, Titulo = "Especialista Biométrico", PuntosRequeridos = 1200 },
        new() { Numero = 4, Titulo = "Director de Laboratorio", PuntosRequeridos = 2500 },
        new() { Numero = 5, Titulo = "Comandante Científico", PuntosRequeridos = 5000 },
    ];

    public static readonly Logro[] Logros =
    [
        new()
        {
            Clave = "hola-explorador", Nombre = "¡Hola, explorador!",
            Descripcion = "Realiza tu primer análisis en OmniScan.",
            RequisitoTipo = TiposRequisito.HallazgosTotales, RequisitoValor = 1, PuntosOtorgados = 20,
            Icono = "profile/medalla-hola-explorador.webp",
        },
        new()
        {
            Clave = "ojos-curiosos", Nombre = "Ojos curiosos",
            Descripcion = "Analiza 5 muestras en total.",
            RequisitoTipo = TiposRequisito.HallazgosTotales, RequisitoValor = 5, PuntosOtorgados = 50,
            Icono = "profile/medalla-ojos-curiosos.webp",
        },
        new()
        {
            Clave = "pequeno-investigador", Nombre = "Investigador de campo",
            Descripcion = "Identifica 10 especies diferentes.",
            RequisitoTipo = TiposRequisito.EspeciesDistintas, RequisitoValor = 10, PuntosOtorgados = 100,
            Icono = "profile/medalla-investigador.webp",
            IconoBloqueado = "profile/medalla-investigador-bloqueada.webp",
        },
        new()
        {
            Clave = "amigo-fauna", Nombre = "Amigo de la fauna",
            Descripcion = "Identifica 5 animales: insectos, aves o vida acuática.",
            RequisitoTipo = TiposRequisito.FaunaDistinta, RequisitoValor = 5, PuntosOtorgados = 75,
            Icono = "profile/medalla-fauna.webp",
            IconoBloqueado = "profile/medalla-fauna-bloqueada.webp",
        },
        new()
        {
            Clave = "cientifico-accion", Nombre = "Científico en acción",
            Descripcion = "Explora 5 bases de datos distintas.",
            RequisitoTipo = TiposRequisito.CategoriasDistintas, RequisitoValor = 5, PuntosOtorgados = 100,
            Icono = "profile/medalla-cientifico.webp",
            IconoBloqueado = "profile/medalla-cientifico-bloqueada.webp",
        },
    ];
}
