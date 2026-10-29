using Microsoft.AspNetCore.Mvc;
using VisionLab.Api.Common;
using VisionLab.Application.Features.Auth;

namespace VisionLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    /// <summary>Registra un nuevo usuario.</summary>
    [HttpPost("register")]
    [ProducesResponseType<UsuarioDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioDto>> Register(
        RegistrarUsuarioRequest request, [FromServices] RegistrarUsuarioHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : this.ToProblem(result.Error!);
    }

    /// <summary>Inicia sesión con email y contraseña.</summary>
    [HttpPost("login")]
    [ProducesResponseType<UsuarioDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UsuarioDto>> Login(
        IniciarSesionRequest request, [FromServices] IniciarSesionHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblem(result.Error!);
    }
}
