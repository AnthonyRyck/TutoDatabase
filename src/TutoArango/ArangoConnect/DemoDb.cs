using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Arango.Protocol;
using ArangoConnect.Models;
using Core.Arango.Linq;
using System.Linq;
using Core.Arango;

namespace ArangoConnect
{
	public class DemoDb : ArangoLoader
	{

		public DemoDb(string url, int port, string projectName, string login, string password) 
			: base(url, port, projectName, login, password)
		{
		}

		/// <summary>
		/// Permet de créer une collection.
		/// </summary>
		/// <param name="collectionName"></param>
        public async Task CreateDocumentsCollection(string database, string collectionName)
        {
            await Arango.Collection.CreateAsync(database, collectionName, ArangoCollectionType.Document);
        }

		/// <summary>
		/// Ajoute des fichiers json dans la collection.
		/// </summary>
		/// <param name="collectionName"></param>
		/// <param name="pathFiles"></param>
		/// <returns></returns>
		public async Task ImportManyAsync<T>(string dataBase, string collectionName, params string [] pathFiles)
		{
			List<T> importFiles = new List<T>();
			foreach (var file in pathFiles)
			{
				string content = await File.ReadAllTextAsync(file);
				importFiles.Add(JsonSerializer.Deserialize<T>(content));
			}

			// Juste pour montrer le retour.
			List<ArangoUpdateResult<ArangoVoid>> collection = await Arango.Document.CreateManyAsync(dataBase, collectionName, importFiles);
		}
		

		/// <summary>
		/// Ajout d'un client.
		/// </summary>
		/// <param name="unNouveauClient"></param>
		/// <returns></returns>
		public async Task AddClientAsync(string dataBase, string collectionName, Client unNouveauClient)
		{
			await Arango.Document.CreateAsync(dataBase, collectionName,unNouveauClient);
		}

		/// <summary>
		/// Met à jour un client
		/// </summary>
		/// <param name="databaseName"></param>
		/// <param name="nomDuClient"></param>
		/// <param name="age"></param>
		/// <returns></returns>
		public async Task UpdateAgeClientByNameAsync(string databaseName, string nomDuClient, int age)
		{
			var clientToUpdate = Arango.Query<Client>(databaseName)
											.Where(x => x.Nom == nomDuClient)
											.Update(x => new
											{
												Age = age
											});

			await clientToUpdate.FirstOrDefaultAsync();
		}

		/// <summary>
		/// Retourne le client par rapport à son IdClient.
		/// </summary>
		/// <param name="dataBase"></param>
		/// <param name="collectionName"></param>
		/// <param name="idClient"></param>
		/// <returns></returns>
		public async Task<Client> GetClientByIdAsync(string dataBase, int idClient)
		{
			return await Arango.Query<Client>(dataBase)
								.FirstOrDefaultAsync(x => x.IdClient == idClient);
		}

		/// <summary>
		/// Retourne tous les clients de genre "female"
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<Client>> GetFemaleClientsAsync(string database)
		{
			return await Arango.Query<Client>(database)
						.Where(x => x.Genre == "female")
						.ToListAsync();
		}

		/// <summary>
		/// Retourne tous les clients qui ont un âge inférieur donné
		/// </summary>
		/// <param name="databaseName"></param>
		/// <param name="v"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public async Task<IEnumerable<Client>> GetAgeClientsInfOuEgalToAsync(string databaseName, int age)
		{
			return await Arango.Query<Client>(databaseName)
								.Where(x => x.Age <= age)
								.ToListAsync();
		}

		/// <summary>
		/// Fait la jointure entre 2 collections.
		/// </summary>
		/// <param name="databaseName"></param>
		/// <param name="collectionClient"></param>
		/// <param name="collectionCommandes"></param>
		/// <returns></returns>
		public async Task<IEnumerable<ClientCommandes>> JointureEntreDeuxCollectionsAsync(string databaseName, string collectionClient, string collectionCommandes)
		{
			// Requête AQL
			// FOR cli IN Client
			// FOR cmd IN Commande
			// FILTER cli.IdClient == cmd.ClientId
			// RETURN { client: cli, commande: cmd }

			FormattableString forPartClient = $"FOR cli IN {collectionClient:@}";
			FormattableString forPartCmd = $"FOR cmd IN {collectionCommandes:@}";
			FormattableString filterPart = $"FILTER cli.IdClient == cmd.ClientId";
			FormattableString returnPart = $"RETURN {{ Client: cli, Commande: cmd}}";
			ArangoList<ClientCommandes> resultJointure = await Arango.Query.ExecuteAsync<ClientCommandes>(databaseName, $"{forPartClient} {forPartCmd} {filterPart} {returnPart}");

			return resultJointure.ToList();
		}
	}
}
