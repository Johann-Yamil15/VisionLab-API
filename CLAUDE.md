# VisionLab API — Backend (memoria del proyecto)

> Archivo de memoria persistente. Se actualiza en cada cambio estructural (proyectos, endpoints,
> colecciones, decisiones). Leer esto antes de explorar el código.
> Frontend: `C:\Users\USUARIO\vision-lab` (Angular, tiene su propio `CLAUDE.md`).
> Repo: https://github.com/Johann-Yamil15/VisionLab-API

## Qué es
API del microscopio digital para niños VisionLab: usuarios, análisis de imágenes con Gemini,
hallazgos como tarjetas coleccionables (sin duplicados: si se repite, suma "veces encontrado"),
chat con IA sobre un hallazgo, gamificación (puntos, niveles, medallas).
Concurso: Rehilete Pachuca. Fecha límite aprox.: **2026-10-29**.

## Decisiones
- **Monolito** (un solo despliegue) con **Clean Architecture** + **vertical slices** en Application.
- **Stateless** para escalar horizontalmente: nada de estado en memoria; todo en Mongo; usuario
  por JWT (pendiente). `/health` para balanceadores. Config por variables de entorno.
- **MongoDB** única base de datos. Imágenes irán a **GridFS** (mismo Mongo).
- Gemini se llama SOLO desde aquí. Key en `dotnet user-secrets` (dev) o `Gemini__ApiKey` (prod).
- Auth: BCrypt + JWT (pendiente). El `usuarioId` sale del token, nunca del body.
- Sin MediatR (licencia comercial desde v13): cada caso de uso es una clase `*Handler` que se
  registra sola por reflexión (`Application/DependencyInjection.cs`).

## Git
Ramas: `main` (producción) ← `develop` (integración, default) ← `feature/*`, `fix/*` por PR.
Hook antisecretos: `git config core.hooksPath .githooks` (una vez por clon).

## Stack
.NET 9 (SDK 9.0.318), MongoDB.Driver 2.28, Swashbuckle 6.8. Mongo local 8.2 `localhost:27017`,
base `vision_lab`. Colecciones: `usuarios`, `hallazgos`, `logros` (vacía), `niveles` (vacía).

## Cómo correr
```
dotnet run --project src/VisionLab.Api --launch-profile https   # https://localhost:44312 (Swagger en /)
dotnet run --project src/VisionLab.Api --launch-profile http    # http://localhost:5110
```
Pruebas manuales: `src/VisionLab.Api/VisionLab.Api.http`.
Ojo: en Git Bash, `curl -d` con `ñ` se corrompe; mandar JSON desde archivo (`--data-binary @f.json`).

## Estructura (desde 2026-10-14, rama feature/jj-arquitectura-base)
```
VisionLab.sln · Directory.Build.props (net9.0, nullable, implicit usings)
src/
  VisionLab.Domain/            sin dependencias
    Usuarios/Usuario.cs
    Hallazgos/Hallazgo.cs
    Gamificacion/Logro.cs (EstaCumplidoPor), Nivel.cs, ReglasDePuntos.cs
  VisionLab.Application/       → Domain
    Abstractions/              ICurrentUser, IUsuarioRepository, IHallazgoRepository, ILogro/INivelRepository
    Common/Result.cs           Result<T> + ErrorType (Validation, NotFound, Conflict, Unauthorized)
    Features/
      Auth/                    RegistrarUsuario, IniciarSesion, UsuarioDto
      Hallazgos/               CrearHallazgo, ConsultarHallazgos (listar/id/tipo/tipos), EliminarHallazgo, HallazgoDto
      Gamificacion/            OtorgarProgreso (puntos, logros, nivel)
      Perfil/                  ObtenerPerfil
    DependencyInjection.cs     AddApplication(): registra *Handler (scoped)
  VisionLab.Infrastructure/    → Application
    Persistence/               MongoOptions, MongoContext, MongoMappings (BsonClassMap, sin atributos
                               en Domain), MongoIndexesInitializer, MongoHealthCheck
    Persistence/Repositories/  Usuario, Hallazgo, Logro/Nivel (singletons)
    DependencyInjection.cs     AddInfrastructure(config)
  VisionLab.Api/               → Application + Infrastructure
    Controllers/               Auth, Hallazgos, Usuarios (delgados, [FromServices] handler)
    Common/ResultExtensions.cs Error → ProblemDetails
    Security/DemoCurrentUser   ICurrentUser TEMPORAL (DemoUser:Id de appsettings)
    Program.cs                 CORS desde Cors:AllowedOrigins, ProblemDetails, ForwardedHeaders,
                               Swagger (solo Development), /health
```
Mapeo Mongo: nombres de campo PascalCase heredados; `Usuario.Contrasena` ↔ campo `Contraseña`.
Índices (se crean al arrancar): `usuarios.Email` único case-insensitive,
`hallazgos.UsuarioId+FechaCaptura`, `hallazgos.Tipo` case-insensitive, `niveles.PuntosRequeridos`.

## Endpoints (contrato igual al heredado; JSON camelCase)
| Método | Ruta | Notas |
|---|---|---|
| POST | /api/auth/register | body `{nombreUsuario,email,contraseña}` → 201 / 409 |
| POST | /api/auth/login | body `{email,contraseña}` → 200 / 401 |
| GET | /api/hallazgos | TODOS (aún sin filtrar por usuario) |
| GET | /api/hallazgos/{id} | id de 24 caracteres |
| POST | /api/hallazgos | usuarioId del body se IGNORA; usa ICurrentUser |
| DELETE | /api/hallazgos/{id} | 204 / 404 |
| GET | /api/hallazgos/tipo/{tipo} | igualdad case-insensitive (antes regex inyectable) |
| GET | /api/hallazgos/tipos/unique | |
| GET | /api/usuarios | perfil del usuario actual |
| GET | /health | Healthy / 503 |
Errores: ProblemDetails (RFC 9457).

## Pendiente / deuda conocida
- Contraseñas en texto plano y sin JWT → feature de seguridad.
- GET hallazgos devuelve los de todos → filtrar por usuario con JWT.
- Datos con `Tipo` inconsistente ("insecto"/"Insecto", "etc") → normalizar en el análisis.
- Imágenes base64 dentro del documento → GridFS + miniaturas.
- Tarjetas sin duplicados, endpoints de Gemini (/api/analisis, /api/chat), rate limiting.
- Docker: `Dockerfile`, `.dockerignore`, `docker-compose.yml` existen en disco SIN commit; pendiente de aprobación.
- Proyecto de tests.

## Bitácora de cambios
- 2026-10-14: creado este archivo. Repo en GitHub con `develop`/`main`, hook antisecretos.
- 2026-10-14: `feature/jj-arquitectura-base`: migración a Clean Architecture (4 proyectos) sin
  cambiar el contrato; probado contra Mongo real (todos los endpoints OK).
