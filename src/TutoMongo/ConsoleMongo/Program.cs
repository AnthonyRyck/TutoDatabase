using ConsoleMongo.Models;
using ConsoleMongo.Mongo;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace ConsoleMongo
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("######## Début de l'application Démo ########");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();

			string urlMongo = "localhost";
			string databaseName = "ctrlaltsupprDb";
			string collectionClient = "clients";
			string collectionCommandes = "commandes";

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("######################################################");
			Console.WriteLine("####### 1ere étape : Connexion à MongoDb. #######");
			Console.WriteLine("----> Connexion à la base : " + databaseName + $" sur : mongodb://{urlMongo}:27017");
			MongoConnecteur mongo = new MongoConnecteur(urlMongo, 27017, databaseName);

			Console.WriteLine("----> Suppressions de toutes les collections.");
			mongo.DropCollectionsAsync(collectionClient, collectionCommandes).Wait();
			await mongo.DropDatabaseAsync();

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("######################################################");
			Console.WriteLine("####### 2eme étape : Connexion à MongoDb. #######");
			Console.WriteLine("----> Ajoutons des données.");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();

			Console.WriteLine($"----> Ajout de 5 fichiers json dans la collection {collectionClient}.");
			string pathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonFiles");
			mongo.ImportFiles(collectionClient,
								Path.Combine(pathBase, "client1.json"),
								Path.Combine(pathBase, "client2.json"),
								Path.Combine(pathBase, "client3.json"),
								Path.Combine(pathBase, "client4.json"),
								Path.Combine(pathBase, "client5.json")).Wait();
			Console.WriteLine("#:> Terminé !");
			long nbreDocument = mongo.GetCountDocument(collectionClient);
			Console.WriteLine($"#:> Il y a {nbreDocument} documents dans la collection {collectionClient}.");

			Console.WriteLine();
			Console.WriteLine("----> Ajout d'un nouveau client, via une instance Client");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
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
				Genre = "Male",
				Telephone = new List<Telephone>()
				{
					new Telephone() { Number = "15488", Type="Maison"},
					new Telephone() { Number = "484848", Type="Pro"}
				}
			};

			await mongo.AddClientAsync(collectionClient, unNouveauClient);
			Console.WriteLine(await mongo.GetClientDocument(collectionClient, unNouveauClient.Nom, unNouveauClient.Prenom));
			Console.WriteLine($"#:> Done ! Client : {unNouveauClient.Nom} ajouté.");

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("######################################################");
			Console.WriteLine("####### 3eme étape : Mette à jour les données. #######");
			Console.WriteLine("----> Faire une mise à jour d'une propriétée sur un client. Changer l'age du client qu'on vient de créer");
			Console.WriteLine("de 18 à 21 ans.");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();

			await mongo.UpdateClientAsync(collectionClient, "Lepetitnouveau", "coucou", 21);
			Console.WriteLine(await mongo.GetClientDocument(collectionClient, "Lepetitnouveau", "coucou"));
			Console.WriteLine($"#:> Done ! Age modifié.");
			Console.WriteLine();

			long nbre = mongo.GetCountDocument(collectionClient);
			Console.WriteLine($"#:> Il y a {nbre} documents dans la collection {collectionClient}.");

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("############################################################");
			Console.WriteLine("####### 4eme étape : Faire des requêtes sur la base. #######");
			Console.WriteLine("#-----> Maintenant faisons des requêtes trouver un client particulier.");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();

			Console.WriteLine("-----> Récupération de tous les clients.");
			var allClients = await mongo.GetClients(collectionClient);
			Console.WriteLine($"#:>{allClients.Count} clients en mémoire.");
			Console.WriteLine($"#:>Voilà à quoi cela ressemble.");
			Console.WriteLine("#:> Appuyer sur une touche pour voir.");
			Console.ReadKey();
			foreach (var client in allClients)
			{
				Console.WriteLine(client);
				Console.WriteLine();
			}

			Console.WriteLine();
			Console.WriteLine("#:> Appuyer sur une touche pour continuer.");
			Console.ReadKey();

			Console.WriteLine("#:> Récupération des clientes. Recherche sur la propriétée \"genre=female\"");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();

			IEnumerable<Client> clientes = await mongo.GetFemaleClients(collectionClient);
			foreach (var cliente in clientes)
			{
				Console.WriteLine($"Clientes trouvées : {cliente.Nom} {cliente.Prenom}.");
			}

			Console.WriteLine();
			Console.WriteLine("----> Récupération des clients qui ont un age inférieur ou égal à 30 ans");
			Console.WriteLine("----> Recherche sur la propriété Age.");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();
			IEnumerable<Client> clientsAgeSup = await mongo.GetAgeClientsInfOuEgalTo(collectionClient, 30);
			foreach (var client in clientsAgeSup)
			{
				Console.WriteLine($"Client trouvé : {client.Nom} {client.Prenom} et son age : {client.Age}.");
			}


			Console.WriteLine();
			Console.WriteLine($"----> Ajout d'une nouvelle collection dans la base : {collectionCommandes}");
			Console.WriteLine("----> Ajout des commandes.");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();

			mongo.ImportFiles(collectionCommandes,
								Path.Combine(pathBase, "Command1.json"),
								Path.Combine(pathBase, "Command2.json"),
								Path.Combine(pathBase, "Command3.json"),
								Path.Combine(pathBase, "Command4.json"),
								Path.Combine(pathBase, "Command5.json")).Wait();
			Console.WriteLine("#:> Terminé !");
			long nbrCommande = mongo.GetCountDocument(collectionCommandes);
			Console.WriteLine($"#:> Il y a {nbrCommande} documents dans la collection {collectionCommandes}.");


			Console.WriteLine();
			Console.WriteLine($"----> Jointure entre 2 collections");
			Console.WriteLine("----> Avoir les commandes des clients.");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();
			await mongo.JointureEntreDeuxCollections(collectionClient, collectionCommandes);

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("###############################################################");
			Console.WriteLine("####### Dernière étape : Suppression de la base donnée. #######");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();
			await mongo.DropDatabaseAsync();

			Console.WriteLine();
			Console.WriteLine("######## Fin de l'application Démo ########");
		}
	}
}
