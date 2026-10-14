namespace VisionLabApi.Models
{
    // Models/VisionLabDatabaseSettings.cs
    public class VisionLabDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;

        // Objeto que contendrá los nombres de todas las colecciones
        public CollectionNames Collections { get; set; } = null!;
    }
}
