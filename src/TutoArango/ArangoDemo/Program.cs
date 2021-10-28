using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ArangoConnect;
using ArangoConnect.Models;

namespace ArangoDemo
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("######## Début de l'application Démo ########");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();

			string urlArango = "localhost";
			int port = 8529;
			string projectName = "TutoArango";
			string databaseName = "ctrlaltsupprDb";
			string collectioncClient = "Client";
			string collectionCommande = "Commande";
			string login = "root";
			string password = "PassCtrlAltSuppr";

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("######################################################");
			Console.WriteLine("####### 1ere étape : Connexion à ArangoDb. #######");
			Console.WriteLine("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			Console.WriteLine($"----> Connexion sur : {urlArango}:{port}");
			DemoDb arango = new DemoDb(urlArango, port, projectName, login, password);
			await arango.DeleteDatabase(databaseName);
			await arango.CreateDatabase(databaseName);

			Console.WriteLine();
			Console.WriteLine($"####### 2eme étape : Création de la collection : {collectioncClient}. #######");
			Console.WriteLine("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			await arango.CreateDocumentsCollection(databaseName, collectioncClient);

			Console.WriteLine();
			Console.WriteLine($"####### 3eme étape : Ajoutons des documents à notre collection. #######");
			Console.WriteLine($"----> Ajout de 5 fichiers json dans la collection {collectioncClient}.");
			Console.WriteLine("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			string pathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
			await arango.ImportManyAsync<Client>(databaseName, collectioncClient,
								Path.Combine(pathBase, "client1.json"),
								Path.Combine(pathBase, "client2.json"),
								Path.Combine(pathBase, "client3.json"),
								Path.Combine(pathBase, "client4.json"),
								Path.Combine(pathBase, "client5.json"));

			Console.WriteLine();
			Console.WriteLine("----> Ajout d'un nouveau client.");
			Console.WriteLine("Appuyer sur une touche pour commencer.");
			Console.ReadKey();
			Client unNouveauClient = new Client()
			{
				IdClient = 159,
				Nom = "Lepetitnouveau",
				Prenom = "coucou",
				Age = 18,
				Adresse = new Adresse()
				{
					Numero = "2",
					Rue = "Rue du perdu",
					Ville = "Aussiloin"
				},
				Genre = "male",
				Telephone = new List<Telephone>()
				{
					new Telephone() { Number = "15488", Type="Maison"},
					new Telephone() { Number = "484848", Type="Pro"}
				}
			};
			await arango.AddClientAsync(databaseName, collectioncClient, unNouveauClient);

			Console.WriteLine();
			Console.WriteLine($"####### 4eme étape : Quelques requêtes sur notre collection. #######");

			Console.WriteLine();
			Console.WriteLine("----> Faire une mise à jour d'une propriétée sur un client. Changer l'age du client qu'on vient de créer");
			Console.WriteLine("de 18 à 21 ans.");
			Console.WriteLine(" Appuyer sur une touche pour commencer.");
			Console.ReadKey();
			await arango.UpdateAgeClientByNameAsync(databaseName, "Lepetitnouveau", 21);


			Console.WriteLine($"----> Retrouver un document par rapport à une propriété.");
			Console.WriteLine("Appuyer sur une touche pour commencer");
			Console.ReadKey();

			Console.WriteLine();
			Console.WriteLine($"----> Par rapport à l'ID du client.");
			Console.WriteLine("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			Client clientId = await arango.GetClientByIdAsync(databaseName, 123);
			Console.WriteLine($"Client trouvé pour ID = 123 : {clientId.Prenom} {clientId.Nom}.");

			Console.WriteLine();
			Console.WriteLine("#:> Récupération des clientes. Recherche sur la propriétée \"genre=female\"");
			Console.WriteLine("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			IEnumerable<Client> clientes = await arango.GetFemaleClientsAsync(databaseName);
			foreach (var cliente in clientes)
			{
				Console.WriteLine($"Cliente trouvée : {cliente.Prenom} {cliente.Nom} - {cliente.Genre}.");
			}

			Console.WriteLine();
			Console.WriteLine("----> Récupération des clients qui ont un age inférieur ou égal à 30 ans");
			Console.WriteLine("----> Recherche sur la propriété Age.");
			Console.WriteLine("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			IEnumerable<Client> clientsAgeSup = await arango.GetAgeClientsInfOuEgalToAsync(databaseName, 30);
			foreach (var client in clientsAgeSup)
			{
				Console.WriteLine($"Client trouvé : {client.Nom} {client.Prenom} et son age : {client.Age}.");
			}

			Console.WriteLine();
			Console.WriteLine($"####### Dernière étape : Jointure entre 2 collections. #######");
			Console.WriteLine("Appuyer sur une touche pour commencer");
			Console.ReadKey();

			Console.WriteLine($"----> Ajout d'une nouvelle collection dans la base : {collectionCommande}");
			Console.WriteLine("----> Ajout des commandes.");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();
			await arango.CreateDocumentsCollection(databaseName, collectionCommande);
			await arango.ImportManyAsync<Commande>(databaseName, collectionCommande,
								Path.Combine(pathBase, "Command1.json"),
								Path.Combine(pathBase, "Command2.json"),
								Path.Combine(pathBase, "Command3.json"),
								Path.Combine(pathBase, "Command4.json"),
								Path.Combine(pathBase, "Command5.json"),
								Path.Combine(pathBase, "Command6.json"));

			Console.WriteLine();
			Console.WriteLine($"----> Jointure entre 2 collections");
			Console.WriteLine("----> Avoir les commandes des clients.");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();
			var resultatJointure = await arango.JointureEntreDeuxCollectionsAsync(databaseName, collectioncClient, collectionCommande);
			foreach (var cmd in resultatJointure)
			{
				Console.WriteLine($"Client : {cmd.Client.ToString()}");
				Console.WriteLine($"Commande numéro : {cmd.Commande.IdCommand}");
			}

			Console.WriteLine(); 
			Console.WriteLine("######## Fin de l'application Démo ########");
			Console.ReadKey();
		}
	}
}
