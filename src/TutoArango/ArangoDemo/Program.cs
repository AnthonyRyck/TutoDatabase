using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ArangoConnect.Models;

namespace ArangoDemo
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			WriteLineInfo("######## Début de l'application Démo ########");
			WriteLineInfo("#:> Appuyer sur une touche pour commencer.");
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
			WriteLineInfo("######################################################");
			WriteLineInfo("####### 1ere étape : Connexion à ArangoDb. #######");
			WriteLineInfo("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			Console.WriteLine($"----> Connexion sur : {urlArango}:{port}");
			DemoDb arango = new DemoDb(urlArango, port, projectName, login, password);
			await arango.DeleteDatabase(databaseName);
			WriteLineResult($"Suppression de la base {databaseName} - OK");
			await arango.CreateDatabase(databaseName);
			WriteLineResult($"Création de la base {databaseName} - OK");

			Console.WriteLine();
			WriteLineInfo($"####### 2eme étape : Création de la collection : {collectioncClient}. #######");
			WriteLineInfo("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			await arango.CreateDocumentsCollection(databaseName, collectioncClient);
			WriteLineResult($"Création de la collection {collectioncClient} - OK");

			Console.WriteLine();
			WriteLineInfo($"####### 3eme étape : Ajoutons des documents à notre collection. #######");
			WriteLineInfo($"----> Ajout de 5 fichiers json dans la collection {collectioncClient}.");
			WriteLineInfo("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			string pathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
			await arango.ImportManyAsync<Client>(databaseName, collectioncClient,
								Path.Combine(pathBase, "client1.json"),
								Path.Combine(pathBase, "client2.json"),
								Path.Combine(pathBase, "client3.json"),
								Path.Combine(pathBase, "client4.json"),
								Path.Combine(pathBase, "client5.json"));
			WriteLineResult($"Ajout dans la collection {collectioncClient} de 5 clients - OK");

			Console.WriteLine();
			WriteLineInfo("----> Ajout d'un nouveau client.");
			WriteLineInfo("Appuyer sur une touche pour commencer.");
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
			WriteLineResult($"Ajout dans la collection {collectioncClient} d'un nouveau client - OK");

			Console.WriteLine();
			WriteLineInfo($"####### 4eme étape : Quelques requêtes sur notre collection. #######");

			Console.WriteLine();
			WriteLineInfo("----> Faire une mise à jour d'une propriétée sur un client. Changer l'age du client qu'on vient de créer");
			WriteLineInfo("de 18 à 21 ans.");
			WriteLineInfo(" Appuyer sur une touche pour commencer.");
			Console.ReadKey();
			await arango.UpdateAgeClientByNameAsync(databaseName, "Lepetitnouveau", 21);
			WriteLineResult($"Mise à jour de l'âge du client \"Lepetitnouveau\" - OK");

			Console.WriteLine();
			WriteLineInfo($"----> Retrouver un document par rapport à une propriété.");
			WriteLineInfo($"----> Par rapport à l'ID du client.");
			WriteLineInfo("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			Client clientId = await arango.GetClientByIdAsync(databaseName, 123);
			WriteLineResult($"Client trouvé pour ID = 123 : {clientId.Prenom} {clientId.Nom}.");

			Console.WriteLine();
			WriteLineInfo("#:> Récupération des clientes. Recherche sur la propriétée \"genre=female\"");
			WriteLineInfo("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			IEnumerable<Client> clientes = await arango.GetFemaleClientsAsync(databaseName);
			foreach (var cliente in clientes)
			{
				WriteLineResult($"Cliente trouvée : {cliente.Prenom} {cliente.Nom} - {cliente.Genre}.");
			}

			Console.WriteLine();
			WriteLineInfo("----> Récupération des clients qui ont un age inférieur ou égal à 30 ans");
			WriteLineInfo("----> Recherche sur la propriété Age.");
			WriteLineInfo("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			IEnumerable<Client> clientsAgeSup = await arango.GetAgeClientsInfOuEgalToAsync(databaseName, 30);
			foreach (var client in clientsAgeSup)
			{
				WriteLineResult($"Client trouvé : {client.Nom} {client.Prenom} et son age : {client.Age}.");
			}

			Console.WriteLine();
			WriteLineInfo($"####### Dernière étape : Jointure entre 2 collections. #######");
			WriteLineInfo("Appuyer sur une touche pour commencer");
			Console.ReadKey();

			WriteLineInfo($"----> Ajout d'une nouvelle collection dans la base : {collectionCommande}");
			WriteLineInfo("----> Ajout des commandes.");
			WriteLineInfo("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();
			await arango.CreateDocumentsCollection(databaseName, collectionCommande);
			WriteLineResult($"Création de la collection {collectionCommande} - OK");

			await arango.ImportManyAsync<Commande>(databaseName, collectionCommande,
								Path.Combine(pathBase, "Command1.json"),
								Path.Combine(pathBase, "Command2.json"),
								Path.Combine(pathBase, "Command3.json"),
								Path.Combine(pathBase, "Command4.json"),
								Path.Combine(pathBase, "Command5.json"),
								Path.Combine(pathBase, "Command6.json"));
			WriteLineResult($"Ajout dans la collection {collectionCommande} de 6 commandes - OK");

			Console.WriteLine();
			WriteLineInfo($"----> Jointure entre 2 collections");
			WriteLineInfo("----> Avoir les commandes des clients.");
			WriteLineInfo("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();
			var resultatJointure = await arango.JointureEntreDeuxCollectionsAsync(databaseName, collectioncClient, collectionCommande);
			foreach (var cmd in resultatJointure)
			{
				WriteLineResult($"Client : {cmd.Client.ToString()}");
				WriteLineResult($"Commande numéro : {cmd.Commande.IdCommand}");
				Console.WriteLine();
			}

			Console.WriteLine();
			WriteLineInfo("######## Fin de l'application Démo ########");
			Console.ReadKey();
		}



		private static void WriteLineInfo(string message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(message);
		}

		private static void WriteLineResult(string message)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(message);
		}
	}
}
