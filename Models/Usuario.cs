using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace VisionLabApi.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string NombreUsuario { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Contraseña { get; set; } = null!;




        // --- CAMPOS DE GAMIFICACIÓN ---

        /// <summary>
        /// Puntos acumulados por el usuario
        /// </summary>
        [BsonElement("Puntos")]
        public int Puntos { get; set; } = 0;

        /// <summary>
        /// Nivel actual del usuario (calculado en base a los Puntos)
        /// </summary>
        [BsonElement("Nivel")]
        public int Nivel { get; set; } = 1;

        /// <summary>
        /// IDs de los logros desbloqueados por el usuario
        /// </summary>
        [BsonElement("LogrosDesbloqueados")]
        public List<string> LogrosDesbloqueados { get; set; } = new List<string>();

        /// <summary>
        /// Contador de hallazgos subidos (útil para logros)
        /// </summary>
        [BsonElement("HallazgosContador")]
        public int HallazgosContador { get; set; } = 0;
    }
}