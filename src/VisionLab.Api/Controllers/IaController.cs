using Microsoft.AspNetCore.Mvc;
using VisionLab.Api.Common;
using VisionLab.Application.Features.Ia;

namespace VisionLab.Api.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public sealed class IaController : ControllerBase
{
    /// <summary>
    /// Analiza una foto (jpeg/png/webp, máx. 5 MB, base64 o data URL) y devuelve la ficha de la tarjeta.
    /// Si la confianza es menor a 60 incluye 3 opciones alternativas.
    /// </summary>
    [HttpPost("analisis")]
    [RequestSizeLimit(8 * 1024 * 1024)]
    [ProducesResponseType<AnalisisDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<AnalisisDto>> Analizar(
        AnalizarImagenRequest request, [FromServices] AnalizarImagenHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        return result.IsSuccess ? result.Value! : this.ToProblem(result.Error!);
    }

    /// <summary>Responde una pregunta (máx. 300 caracteres) sobre la especie, en máximo 4 oraciones.</summary>
    [HttpPost("chat")]
    [ProducesResponseType<ChatRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ChatRespuesta>> Chat(
        ChatRequest request, [FromServices] ChatExpertoHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        return result.IsSuccess ? result.Value! : this.ToProblem(result.Error!);
    }
}
