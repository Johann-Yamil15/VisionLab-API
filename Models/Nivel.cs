using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VisionLabApi.Models
{
    public class Nivel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// Número del nivel (ej: 1, 2, 3...)
        /// </summary>
        [BsonElement("Numero")]
        public int Numero { get; set; }

        /// <summary>
        /// Puntos totales requeridos para alcanzar este nivel
        /// </summary>
        [BsonElement("PuntosRequeridos")]
        public int PuntosRequeridos { get; set; }

        /// <summary>
        /// Título del nivel (ej: Becario, Científico Junior, Experto)
        /// </summary>
        [BsonElement("Titulo")]
        public string Titulo { get; set; } = null!;
    }
}