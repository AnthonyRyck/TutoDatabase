using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ConsoleMongo.Models
{
	public class Commande : MongoId
	{
		[BsonElement("ClientId")]
		public string ClientId { get; set; }

		[BsonElement("IdCommand")]
		public int IdCommand { get; set; }

		[BsonElement("Panier")]
		public List<Panier> Panier { get; set; }

	}
}
