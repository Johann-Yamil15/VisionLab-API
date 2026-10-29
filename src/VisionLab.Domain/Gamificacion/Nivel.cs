namespace VisionLab.Domain.Gamificacion;

public class Nivel
{
    public string Id { get; set; } = string.Empty;
    public int Numero { get; set; }
    public int PuntosRequeridos { get; set; }
    public string Titulo { get; set; } = string.Empty;
}
