﻿using ArangoGraphDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArangoGraphDemo
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
			string graphName = "EveGraph";
			string login = "root";
			string password = "PassCtrlAltSuppr";

			Console.WriteLine();
			Console.WriteLine();
			WriteLineInfo("######################################################");
			WriteLineInfo("####### 1ere étape : Connexion à ArangoDb. #######");
			WriteLineInfo("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			Console.WriteLine($"----> Connexion sur : {urlArango}:{port}");
			GraphDemoDb arango = new GraphDemoDb(urlArango, port, projectName, login, password);
			await arango.DeleteDatabase(databaseName);
			WriteLineResult($"Suppression de la base {databaseName} - OK");
			await arango.CreateDatabase(databaseName);
			WriteLineResult($"Création de la base {databaseName} - OK");

			Console.WriteLine();
			WriteLineInfo($"####### 2eme étape : Création du Graph {graphName} #######");
			WriteLineInfo("Appuyer sur une touche pour commencer");
			Console.ReadKey();
			await arango.CreateGraph(databaseName, graphName);
			WriteLineResult("Graph créé - OK");

			Console.WriteLine();
			WriteLineInfo($"####### 3eme étape : Chargeons les systèmes et les Jumps en mémoire. #######");
			WriteLineInfo("Appuyer sur une touche pour commencer");

			string pathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
			string pathSystems = Path.Combine(pathBase, "AllSystemsTheForge.json");
			string pathJumps = Path.Combine(pathBase, "AllJumpsTheForge.json");

			WriteLineInfo("Chargement des systèmes solaire...");
			var jsonContentSystems = await File.ReadAllTextAsync(pathSystems);
			List<SolarSystem> allSolarSystems = JsonSerializer.Deserialize<List<SolarSystem>>(jsonContentSystems);
			WriteLineResult($"# Il y a {allSolarSystems.Count} systèmes solaires.");

			WriteLineInfo("Chargement du fichier des jumps...");
			string jsonContentJumps = await File.ReadAllTextAsync(pathJumps);
			List<Jumps> allJumps = JsonSerializer.Deserialize<List<Jumps>>(jsonContentJumps);
			WriteLineResult($"# Il y a {allJumps.Count} sauts interstellaires possibles.");

			WriteLineInfo("Fin du chargement des fichiers...");

			Console.WriteLine();
			WriteLineInfo(":> Injection des données pour créer le graph");
			WriteLineInfo("Appuyer sur une touche pour continuer...");
			Console.ReadKey();
			
			await arango.PopulateAsync(databaseName, graphName, allSolarSystems, allJumps);
			WriteLineResult("Graph créé !");
			
			Console.WriteLine();
			WriteLineInfo($"####### 3eme étape : Faisons quelques requêtes. #######");
			WriteLineInfo("Appuyer sur une touche pour commencer");
			Console.ReadKey();

			WriteLineInfo("Récupération des systèmes avec une sécurité au moins de 0.5");
			var systemes = await arango.GetSystems(databaseName, 0.5);
			foreach (var sys in systemes)
			{
				WriteLineResult("Nom : " + sys.SolarSystemName + " - Sécurité : " + sys.Securite);
			}

			Console.WriteLine();
			Console.WriteLine("######## Fin de l'application Démo ########");
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
