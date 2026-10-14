using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VisionLabApi.Models
{
    public class Logro
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// Nombre de la insignia (ej: Explorador Novato)
        /// </summary>
        [BsonElement("Nombre")]
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Descripción del logro (ej: Sube tu primer hallazgo verificado.)
        /// </summary>
        [BsonElement("Descripcion")]
        public string Descripcion { get; set; } = null!;

        /// <summary>
        /// Icono o URL de la imagen del logro
        /// </summary>
        [BsonElement("Icono")]
        public string Icono { get; set; } = null!;

        /// <summary>
        /// Requisito de la condición (ej: 5 para 5 hallazgos)
        /// </summary>
        [BsonElement("RequisitoValor")]
        public int RequisitoValor { get; set; }

        /// <summary>
        /// Tipo de requisito (ej: HallazgosTotales, ConfianzaAlta, TiposUnicos)
        /// </summary>
        [BsonElement("RequisitoTipo")]
        public string RequisitoTipo { get; set; } = null!;

        /// <summary>
        /// Puntos otorgados al desbloquear el logro
        /// </summary>
        [BsonElement("PuntosOtorgados")]
        public int PuntosOtorgados { get; set; }
    }
}