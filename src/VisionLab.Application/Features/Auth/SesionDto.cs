namespace VisionLab.Application.Features.Auth;

/// <summary>Respuesta de login/registro: token JWT + datos del usuario.</summary>
public sealed record SesionDto(string Token, UsuarioDto Usuario);
