using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VisionLab.Api.Common;
using VisionLab.Application.Features.Auth;

namespace VisionLab.Api.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting(RateLimitPolicies.Auth)]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    /// <summary>Registra un nuevo usuario y devuelve su sesión (token JWT).</summary>
    [HttpPost("register")]
    [ProducesResponseType<SesionDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SesionDto>> Register(
        RegistrarUsuarioRequest request, [FromServices] RegistrarUsuarioHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : this.ToProblem(result.Error!);
    }

    /// <summary>Inicia sesión con email y contraseña y devuelve su sesión (token JWT).</summary>
    [HttpPost("login")]
    [ProducesResponseType<SesionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SesionDto>> Login(
        IniciarSesionRequest request, [FromServices] IniciarSesionHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblem(result.Error!);
    }
}
