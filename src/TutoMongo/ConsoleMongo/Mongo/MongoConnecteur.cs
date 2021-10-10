using ConsoleMongo.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleMongo.Mongo
{
	public class MongoConnecteur
	{
		#region Properties

		private MongoClient Client;
		
		/// <summary>
		/// Nom de la base de donnée connecté.
		/// </summary>
		public string DatabaseName { get; private set; }

		/// <summary>
		/// Base de donnée connecté.
		/// </summary>
		public IMongoDatabase MongoDatabase { get; private set; }

		#endregion

		#region Constructeur


		public MongoConnecteur(string serveurUrl, int port, string databaseName)
		{
			Client = new MongoClient($"mongodb://{serveurUrl}:{port}");

			MongoDatabase = Client.GetDatabase(databaseName);
			DatabaseName = databaseName;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Connexion à la base de donnée.
		/// </summary>
		/// <param name="name">Nom de la base de donnée</param>
		public void ConnectToDatabase(string name)
		{
			try
			{
				Client.GetDatabase(name);
				DatabaseName = name;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erreur sur la connexion à la base : {name}");
				Console.WriteLine($"StackTrace : {ex.StackTrace}");
			}
		}
				
		public async Task DropCollectionsAsync(params string[] collectionsName)
		{
			await Task.Factory.StartNew(() =>
			{
				foreach (var collection in collectionsName)
				{
					MongoDatabase.DropCollection(collection);
				}
			});
		}


		/// <summary>
		/// Ajoute des fichiers json dans la collection.
		/// </summary>
		/// <param name="collectionName"></param>
		/// <param name="pathFiles"></param>
		/// <returns></returns>
		public async Task ImportFiles(string collectionName, params string[] pathFiles)
		{
			IMongoCollection<BsonDocument> collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);

			List<BsonDocument> nouveauDoc = new List<BsonDocument>();
			foreach (var file in pathFiles)
			{
				string content = await File.ReadAllTextAsync(file);
				var document = BsonDocument.Parse(content);
				nouveauDoc.Add(document);
			}

			await collection.InsertManyAsync(nouveauDoc);
		}

		/// <summary>
		/// Recherche un client en fonction de son nom
		/// </summary>
		/// <param name="collectionClient"></param>
		/// <param name="nom"></param>
		/// <param name="prenom"></param>
		/// <returns></returns>
		internal async Task<string> GetClientDocument(string collectionName, string nom, string prenom)
		{
			IMongoCollection<BsonDocument> collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);

			// Sur quel document faire la mise à jour.
			var filterBuilder = Builders<BsonDocument>.Filter;
			var filter = filterBuilder.Eq("Nom", nom) & filterBuilder.Eq("Prenom", prenom);

			var document = await collection.Find(filter).FirstOrDefaultAsync();
			return document.ToJson();
		}

		/// <summary>
		/// Retourne le nombre de document dans cette collection.
		/// </summary>
		/// <param name="collectionName"></param>
		/// <returns></returns>
		internal long GetCountDocument(string collectionName)
		{
			IMongoCollection<BsonDocument> collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
			return collection.CountDocuments(new BsonDocument());
		}



		/// <summary>
		/// Permet d'insérer un client.
		/// </summary>
		/// <param name="collectionName">Nom de la collection</param>
		/// <param name="client">Nouveau client</param>
		/// <returns></returns>
		public async Task AddClientAsync(string collectionName, Client client)
		{
			IMongoCollection<BsonDocument> collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);

			BsonDocument bsonElement = client.ToBsonDocument();
			await collection.InsertOneAsync(bsonElement);
		}

		/// <summary>
		/// Met à jour une propriété d'un client, là son age.
		/// </summary>
		/// <param name="collectionClient"></param>
		/// <param name="nom"></param>
		/// <param name="prenom"></param>
		/// <param name="age"></param>
		/// <returns></returns>
		internal async Task UpdateClientAsync(string collectionName, string nom, string prenom, int age)
		{
			IMongoCollection<BsonDocument> collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);

			// Sur quel document faire la mise à jour.
			var filterBuilder = Builders<BsonDocument>.Filter;
			var filter = filterBuilder.Eq("Nom", nom) & filterBuilder.Eq("Prenom", prenom);

			// Quel est la mise à jour.
			var update = Builders<BsonDocument>.Update.Set("Age", age);

			await collection.UpdateOneAsync(filter, update);
		}

		/// <summary>
		/// Récupère toutes les clients, par rapport à la propriétée genre
		/// </summary>
		/// <param name="collectionName"></param>
		/// <returns></returns>
		internal async Task<IEnumerable<Client>> GetFemaleClients(string collectionName)
		{
			IMongoCollection<BsonDocument> collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);

			var filterFemale = Builders<BsonDocument>.Filter.Eq("Genre", "female");
			var documents = await collection.Find(filterFemale).ToListAsync();

			List<Client> clientes = new List<Client>();
			foreach (var doc in documents)
			{
				Client client = BsonSerializer.Deserialize<Client>(doc);
				clientes.Add(client);
			}

			return clientes;
		}

		/// <summary>
		/// Recherche les clients qui ont moins ou égal à l'age donné.
		/// </summary>
		/// <param name="collectionName"></param>
		/// <param name="age"></param>
		/// <returns></returns>
		internal async Task<IEnumerable<Client>> GetAgeClientsInfOuEgalTo(string collectionName, int age)
		{
			IMongoCollection<BsonDocument> collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);

			var filtreAge = Builders<BsonDocument>.Filter.Lte("Age", age);
			var documents = await collection.Find(filtreAge).ToListAsync();

			List<Client> clients = new List<Client>();
			foreach (var doc in documents)
			{
				Console.WriteLine(doc.ToJson());
				Console.WriteLine();

				Client client = BsonSerializer.Deserialize<Client>(doc);
				clients.Add(client);
			}

			return clients;
		}

		/// <summary>
		/// Retourne tous les documents de la collections.
		/// </summary>
		/// <param name="collectionName">Nom de la collection</param>
		/// <returns></returns>
		public async Task<List<BsonDocument>> GetClients(string collectionName)
		{
			IMongoCollection<BsonDocument> collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
			return await collection.Find(new BsonDocument()).ToListAsync();
		}

		/// <summary>
		/// Requête de jointure
		/// </summary>
		/// <param name="collectionClient"></param>
		/// <param name="collectionCommande"></param>
		/// <returns></returns>
		public async Task<List<ClientCommandes>> JointureEntreDeuxCollections(string collectionClient, string collectionCommande)
		{
			try
			{
				var command = new BsonDocument
				{
					{ "$lookup", new BsonDocument
						{
							{"from", collectionCommande },
							{"localField", "IdClient" },
							{"foreignField", "ClientId" },
							{"as", "CommandsAssociees" },
						}
					}
				};

				Console.WriteLine();
				Console.WriteLine(command.ToJson());
				// La requête en JSON:
				//{ "$lookup" : { "from" : "commandes", "localField" : "IdClient", "foreignField" : "ClientId", "as" : "commandsAssociees" } }
				Console.WriteLine();

				IMongoCollection<BsonDocument> collClient = MongoDatabase.GetCollection<BsonDocument>(collectionClient);
				var pipeline = new[] { command };
				return await collClient.Aggregate<ClientCommandes>(pipeline).ToListAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ERROR - {ex.Message}");
				return new List<ClientCommandes>();
			}
		}

		/// <summary>
		/// Supprime la base de donnée.
		/// </summary>
		/// <returns></returns>
		public async Task DropDatabaseAsync()
		{
			await Client.DropDatabaseAsync(DatabaseName);
		}

		#endregion

	}
}
