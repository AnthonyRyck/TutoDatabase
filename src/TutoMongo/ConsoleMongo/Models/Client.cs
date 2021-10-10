using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ConsoleMongo.Models
{
    public class Client : MongoId
    {
        [BsonElement("IdClient")]
		public int IdClient { get; set; }

		[BsonElement("Prenom")]
        public string Prenom { get; set; }

        [BsonElement("Nom")]
        public string Nom { get; set; }

        [BsonElement("Genre")]
        public string Genre { get; set; }

        [BsonElement("Age")]
        public int Age { get; set; }

        [BsonElement("Adresse")]
        public Adresse Adresse { get; set; }

        [BsonElement("Telephone")]
        public List<Telephone> Telephone { get; set; }

        [BsonConstructor]
        public Client(int idClient, string prenom, string nom, string genre, int age, Adresse adresse, List<Telephone> telephone)
        {
            IdClient = idClient;
            Prenom = prenom;
            Nom = nom;
            Genre = genre;
            Age = age;
            Adresse = adresse;
            Telephone = telephone;
        }

		public Client() {}
    }

    public class Adresse
    {
        [BsonElement("Numero")]
        public string Numero { get; set; }

        [BsonElement("Rue")]
        public string Rue { get; set; }

        [BsonElement("Ville")]
        public string Ville { get; set; }
    }

    public class Telephone
    {
        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("Number")]
        public string Number { get; set; }
    }
}
