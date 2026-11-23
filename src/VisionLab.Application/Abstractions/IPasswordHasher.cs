namespace VisionLab.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string contrasena);

    /// <summary>Valida la contraseña; acepta también el formato heredado (texto plano).</summary>
    bool Verify(string contrasena, string almacenada);

    /// <summary>true si lo almacenado aún está en el formato heredado y debe migrarse a hash.</summary>
    bool NecesitaMigrar(string almacenada);
}
