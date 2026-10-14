namespace VisionLabApi.Models
{
    // Models/Hallazgo.cs
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    namespace VisionLabApi.Models
    {
        public class Hallazgo
        {
            // ObjectId es el tipo de ID primario de MongoDB
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } = string.Empty;

            // Propiedad requerida para la referencia al usuario (puedes usar un string simple)
            public string UsuarioId { get; set; } = string.Empty;

            // Campos del JSON de Gemini
            public string Tipo { get; set; } = string.Empty;
            public string NombreComun { get; set; } = string.Empty;
            public string NombreCientifico { get; set; } = string.Empty;

            // La descripción es larga, usamos string
            public string Descripcion { get; set; } = string.Empty;

            // Usamos int para poder realizar comparaciones numéricas
            public int NivelConfianza { get; set; }

            public string? ImagenBase64 { get; set; }


            // Campo adicional para la fecha de registro
            public DateTime FechaCaptura { get; set; } = DateTime.UtcNow;
        }
    }
}
