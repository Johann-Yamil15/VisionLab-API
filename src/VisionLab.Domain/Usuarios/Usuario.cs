namespace VisionLab.Domain.Usuarios;

public class Usuario
{
    public string Id { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash BCrypt de la contraseña. Las cuentas heredadas en texto plano se migran a hash en su primer login.
    /// </summary>
    public string Contrasena { get; set; } = string.Empty;

    public int Puntos { get; set; }
    public int Nivel { get; set; } = 1;
    public List<string> LogrosDesbloqueados { get; set; } = [];
    public int HallazgosContador { get; set; }
}
