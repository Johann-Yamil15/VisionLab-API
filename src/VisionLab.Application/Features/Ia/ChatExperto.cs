using VisionLab.Application.Abstractions;
using VisionLab.Application.Common;

namespace VisionLab.Application.Features.Ia;

/// <summary>Datos mínimos de la especie sobre la que se pregunta.</summary>
public sealed record EspecieChat(string? NombreComun, string? NombreCientifico, string? Descripcion);

public sealed record ChatRequest(EspecieChat? Especie, string? Pregunta);

public sealed record ChatRespuesta(string Respuesta);

public sealed class ChatExpertoHandler(IVisionAi ia)
{
    public const int MaxPregunta = 300;

    public async Task<Result<ChatRespuesta>> Handle(ChatRequest request, CancellationToken ct)
    {
        var pregunta = request.Pregunta?.Trim();
        if (string.IsNullOrEmpty(pregunta))
            return Result<ChatRespuesta>.Failure(ErrorType.Validation, "Escribe una pregunta.");
        if (pregunta.Length > MaxPregunta)
            return Result<ChatRespuesta>.Failure(ErrorType.Validation, $"La pregunta admite máximo {MaxPregunta} caracteres.");
        if (request.Especie is null || string.IsNullOrWhiteSpace(request.Especie.NombreComun))
            return Result<ChatRespuesta>.Failure(ErrorType.Validation, "Falta la especie sobre la que se pregunta.");

        try
        {
            var texto = await ia.ChatAsync(request.Especie, pregunta, ct);
            return Result<ChatRespuesta>.Success(new ChatRespuesta(texto));
        }
        catch (IaNoDisponibleException ex)
        {
            return Result<ChatRespuesta>.Failure(ErrorType.ServiceUnavailable, ex.Message);
        }
    }
}
