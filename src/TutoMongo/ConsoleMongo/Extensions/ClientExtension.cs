using ConsoleMongo.Models;
using MongoDB.Bson;

namespace ConsoleMongo.Extensions
{
	public static class ClientExtension
	{
		/// <summary>
		/// Convertit l'objet Client en BsonDocument
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static BsonDocument ToBsonDocument(this Client source)
		{
			BsonArray bsonTelephone = new BsonArray();

			foreach (var tel in source.Telephone)
			{
				BsonDocument phone = new BsonDocument
					{
						{ "Type", tel.Type },
						{ "Number", tel.Number }
					};

				bsonTelephone.Add(phone);
			}

			return new BsonDocument
			{
				{ "ClientId", source.IdClient },
				{ "Prenom", source.Prenom },
				{ "Nom", source.Nom },
				{ "Genre", source.Genre },
				{ "Nom", source.Age },
				{ "Adresse", new BsonDocument
					{
						{ "Numero", source.Adresse.Numero },
						{ "Rue", source.Adresse.Rue },
						{ "Ville", source.Adresse.Ville },
					}
				},
				{ "Telephone", bsonTelephone }
			};
		}

	}
}
