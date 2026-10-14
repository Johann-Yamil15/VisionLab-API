# VisionLab API — Backend (memoria del proyecto)

> Archivo de memoria persistente. Se actualiza en cada cambio estructural (proyectos, endpoints,
> colecciones, decisiones). Leer esto antes de explorar el código.
> Frontend: `C:\Users\USUARIO\vision-lab` (Angular, tiene su propio `CLAUDE.md`).

## Qué es
API del microscopio digital para niños VisionLab: usuarios, análisis de imágenes con Gemini,
hallazgos como tarjetas coleccionables (sin duplicados: si se repite, suma "veces encontrado"),
chat con IA sobre un hallazgo, gamificación (puntos, niveles, medallas).
Concurso: Rehilete Pachuca. Fecha límite aprox.: **2026-10-29**.

## Decisiones
- **Monolito** (un solo despliegue) con **Clean Architecture** interna.
- **MongoDB** como única base de datos (no Postgres). Imágenes en **GridFS** (mismo Mongo).
- Gemini se llama SOLO desde aquí. La key vive en `dotnet user-secrets` (dev) o variable de
  entorno `Gemini__ApiKey` (prod/Docker). Nunca en appsettings ni en git.
- Auth: contraseñas con BCrypt + JWT. El `usuarioId` sale del token, nunca del body.
- Preparado para Docker (`docker compose`: mongo + api + web), opcional para la entrega.

## Stack
.NET 9 (SDK 9.0.318), MongoDB.Driver 2.28, Swashbuckle. Mongo local 8.2 en `localhost:27017`,
base `vision_lab`, colecciones: `hallazgos`, `usuarios`, `niveles`, `logros`.

## Cómo correr
```
dotnet run --launch-profile https     # https://localhost:7131  (swagger en /)
# IIS Express usa https://localhost:44312 (el que hoy llama el front)
```

## Estado actual (2026-10-14) — estructura heredada (un solo proyecto, compila OK)
```
Controllers/     AuthController (usa Mongo directo, duplica AuthService), HallazgosController,
                 UsuariosController (id de usuario HARDCODEADO)
Services/        AuthService, GamificationService, HallazgoService (UsuarioId HARDCODEADO),
                 *Repository (Usuario, Logro, Nivel)
Infraestructura/ interfaces (IAuthService, IHallazgoService, I*Repository...)
Models/          Usuario, Hallazgo (namespace anidado raro VisionLabApi.Models.VisionLabApi.Models),
                 Logro, Nivel, HallazgoRequest, settings
Dtos/            LoginDto, RegisterDto, UsuarioProfileDto
```
Problemas conocidos: contraseñas en texto plano; sin JWT ni [Authorize]; GET hallazgos devuelve
los de todos; imágenes base64 dentro del documento; regex sin escapar en GetPorTipoAsync;
sin duplicados de tarjetas; **el proyecto no estaba en git**.

## Arquitectura objetivo
```
VisionLab.sln
src/
  VisionLab.Domain/          entidades + reglas (Usuario, Tarjeta/Hallazgo, Evidencia, Logro, Nivel)
  VisionLab.Application/     casos de uso por feature: Auth/, Analisis/, Coleccion/, Chat/,
                             Gamificacion/ + interfaces (IRepos, IVisionAiClient, IPasswordHasher,
                             ITokenService, IImageStore)
  VisionLab.Infrastructure/  Mongo (repos, índices, GridFS), GeminiClient, BCrypt, JWT
  VisionLab.Api/             controllers delgados, auth, CORS, rate limiting, Swagger
tests/ (si da tiempo)
```
Dependencias: Api → Application + Infrastructure; Infrastructure → Application; Application → Domain.

## Bitácora de cambios
- 2026-10-14: creado este archivo. Análisis inicial de front y back. Plan aprobado pendiente.
