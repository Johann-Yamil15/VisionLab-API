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

public sealed class IniciarSesionHandler(IUsuarioRepository usuarios, IPasswordHasher hasher, ITokenService tokens)
{
    public async Task<Result<SesionDto>> Handle(IniciarSesionRequest request, CancellationToken ct)
    {
        var usuario = await usuarios.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct);

        if (usuario is null || !hasher.Verify(request.Contrasena, usuario.Contrasena))
            return Result<SesionDto>.Failure(ErrorType.Unauthorized, "Credenciales inválidas.");

        // Usuarios heredados con contraseña en texto plano: se migran a hash en su primer login.
        if (hasher.NecesitaMigrar(usuario.Contrasena))
        {
            usuario.Contrasena = hasher.Hash(request.Contrasena);
            await usuarios.SetContrasenaAsync(usuario.Id, usuario.Contrasena, ct);
        }

        return Result<SesionDto>.Success(new SesionDto(tokens.Crear(usuario), UsuarioDto.From(usuario)));
    }
}
