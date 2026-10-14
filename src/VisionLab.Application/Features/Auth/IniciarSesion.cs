using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VisionLab.Application.Abstractions;
using VisionLab.Application.Common;

namespace VisionLab.Application.Features.Auth;

public sealed class IniciarSesionRequest
{
    [Required]
    public string Email { get; init; } = string.Empty;

    [Required]
    [JsonPropertyName("contraseña")]
    public string Contrasena { get; init; } = string.Empty;
}

public sealed class IniciarSesionHandler(IUsuarioRepository usuarios)
{
    public async Task<Result<UsuarioDto>> Handle(IniciarSesionRequest request, CancellationToken ct)
    {
        var usuario = await usuarios.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct);

        if (usuario is null || usuario.Contrasena != request.Contrasena)
            return Result<UsuarioDto>.Failure(ErrorType.Unauthorized, "Credenciales inválidas.");

        return Result<UsuarioDto>.Success(UsuarioDto.From(usuario));
    }
}
