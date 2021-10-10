using MongoDB.Bson.Serialization.Attributes;

namespace ConsoleMongo.Models
{
	public class Panier
	{
		[BsonElement("IdItem")]
		public int IdItem { get; set; }

		[BsonElement("Quantite")]
		public int Quantite { get; set; }
	}
}
