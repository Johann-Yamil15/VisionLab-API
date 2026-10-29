using Microsoft.AspNetCore.Mvc;
using VisionLab.Application.Features.Hallazgos;

namespace VisionLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class HallazgosController : ControllerBase
{
    /// <summary>Lista los hallazgos (más recientes primero).</summary>
    [HttpGet]
    public Task<List<HallazgoDto>> Get([FromServices] ListarHallazgosHandler handler, CancellationToken ct) =>
        handler.Handle(ct);

    /// <summary>Obtiene un hallazgo por su id (ObjectId de 24 caracteres).</summary>
    [HttpGet("{id:length(24)}", Name = "GetHallazgo")]
    [ProducesResponseType<HallazgoDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HallazgoDto>> Get(
        string id, [FromServices] ObtenerHallazgoHandler handler, CancellationToken ct) =>
        await handler.Handle(id, ct) is { } hallazgo ? hallazgo : NotFound();

    /// <summary>Guarda un hallazgo analizado y otorga puntos/logros al usuario.</summary>
    [HttpPost]
    [ProducesResponseType<HallazgoDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        CrearHallazgoRequest request, [FromServices] CrearHallazgoHandler handler, CancellationToken ct)
    {
        var hallazgo = await handler.Handle(request, ct);
        return CreatedAtRoute("GetHallazgo", new { id = hallazgo.Id }, hallazgo);
    }

    /// <summary>Elimina un hallazgo.</summary>
    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        string id, [FromServices] EliminarHallazgoHandler handler, CancellationToken ct) =>
        await handler.Handle(id, ct) ? NoContent() : NotFound();

    /// <summary>Hallazgos de un tipo (sin distinguir mayúsculas).</summary>
    [HttpGet("tipo/{tipo}")]
    public Task<List<HallazgoDto>> GetByTipo(
        string tipo, [FromServices] ListarHallazgosPorTipoHandler handler, CancellationToken ct) =>
        handler.Handle(tipo, ct);

    /// <summary>Tipos distintos de hallazgos registrados.</summary>
    [HttpGet("tipos/unique")]
    public Task<List<string>> GetTiposUnicos([FromServices] ListarTiposHandler handler, CancellationToken ct) =>
        handler.Handle(ct);
}
