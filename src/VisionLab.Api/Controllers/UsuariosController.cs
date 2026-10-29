using Microsoft.AspNetCore.Mvc;
using VisionLab.Application.Features.Perfil;

namespace VisionLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class UsuariosController : ControllerBase
{
    /// <summary>Perfil de gamificación del usuario actual (puntos, nivel, logros).</summary>
    [HttpGet]
    [ProducesResponseType<PerfilDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PerfilDto>> GetProfile(
        [FromServices] ObtenerPerfilHandler handler, CancellationToken ct) =>
        await handler.Handle(ct) is { } perfil ? perfil : NotFound();
}
