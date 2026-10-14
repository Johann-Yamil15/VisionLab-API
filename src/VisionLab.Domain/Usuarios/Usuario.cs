namespace VisionLab.Domain.Usuarios;

public class Usuario
{
    public string Id { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña heredada en texto plano. Se reemplaza por un hash en la feature de seguridad.
    /// </summary>
    public string Contrasena { get; set; } = string.Empty;

    public int Puntos { get; set; }
    public int Nivel { get; set; } = 1;
    public List<string> LogrosDesbloqueados { get; set; } = [];
    public int HallazgosContador { get; set; }
}
