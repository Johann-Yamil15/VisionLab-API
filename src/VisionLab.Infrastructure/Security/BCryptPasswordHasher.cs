using VisionLab.Application.Abstractions;

namespace VisionLab.Infrastructure.Security;

internal sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string contrasena) => BCrypt.Net.BCrypt.HashPassword(contrasena);

    public bool Verify(string contrasena, string almacenada) =>
        NecesitaMigrar(almacenada)
            ? almacenada == contrasena // formato heredado (texto plano)
            : BCrypt.Net.BCrypt.Verify(contrasena, almacenada);

    // Los hashes de BCrypt empiezan con $2a$, $2b$ o $2y$.
    public bool NecesitaMigrar(string almacenada) => !almacenada.StartsWith("$2", StringComparison.Ordinal);
}
