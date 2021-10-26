using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.Arango.Protocol;
using ArangoConnect.Models;
using Core.Arango;
using Core.Arango.Linq;
using System.Linq;

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
		public async Task ImportFiles(string dataBase, string collectionName, params string[] pathFiles)
		{
			List<Client> clients = new List<Client>();
			foreach (var file in pathFiles)
			{
				string content = await File.ReadAllTextAsync(file);
				clients.Add(JsonSerializer.Deserialize<Client>(content));
			}

			// Juste pour montrer le retour.
			List<ArangoUpdateResult<ArangoVoid>> collection = await Arango.Document.CreateManyAsync(dataBase, collectionName, clients);
		}

		/// <summary>
		/// Ajout un client via une instance.
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
		public async Task UpdateAgeClientByName(string databaseName, string nomDuClient, int age)
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
		public async Task<Client> GetClientById(string dataBase, string idClient)
		{
			return await Arango.Query<Client>(dataBase)
								.FirstOrDefaultAsync(x => x.IdClient == idClient);
		}


	}
}
