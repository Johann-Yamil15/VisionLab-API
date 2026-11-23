using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VisionLab.Application.Abstractions;
using VisionLab.Application.Common;
using VisionLab.Domain.Usuarios;

namespace VisionLab.Application.Features.Auth;

public sealed class RegistrarUsuarioRequest
{
    [Required]
    public string NombreUsuario { get; init; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(6)]
    [JsonPropertyName("contraseña")]
    public string Contrasena { get; init; } = string.Empty;
}

public sealed class RegistrarUsuarioHandler(IUsuarioRepository usuarios, IPasswordHasher hasher, ITokenService tokens)
{
    public async Task<Result<SesionDto>> Handle(RegistrarUsuarioRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await usuarios.GetByEmailAsync(email, ct) is not null)
            return Result<SesionDto>.Failure(ErrorType.Conflict, "El email ya está registrado.");

        var usuario = new Usuario
        {
            NombreUsuario = request.NombreUsuario.Trim(),
            Email = email,
            Contrasena = hasher.Hash(request.Contrasena)
        };

        await usuarios.CreateAsync(usuario, ct);
        return Result<SesionDto>.Success(new SesionDto(tokens.Crear(usuario), UsuarioDto.From(usuario)));
    }
}
