namespace VisionLab.Application.Abstractions;

/// <summary>
/// Usuario que hace la petición. La API lo resuelve (hoy: usuario demo; después: JWT).
/// Los casos de uso nunca confían en un usuarioId enviado por el cliente.
/// </summary>
public interface ICurrentUser
{
    string? UserId { get; }
}
