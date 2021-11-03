using ArangoGraphDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArangoGraphDemo
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			"######## Début de l'application Démo ########".ToConsoleInfo();
			"#:> Appuyer sur une touche pour commencer.".ToConsoleInfo();
			Console.ReadKey();

			string urlArango = "localhost";
			int port = 8529;
			string projectName = "TutoArangoGraph";
			string databaseName = "ctrlaltsupprGraphDb";
			string graphName = "EveGraph";
			string login = "root";
			string password = "PassCtrlAltSuppr";

			Console.WriteLine();
			Console.WriteLine();
			"######################################################".ToConsoleInfo();
			"####### 1ere étape : Connexion à ArangoDb. #######".ToConsoleInfo();
			"Appuyer sur une touche pour commencer".ToConsoleInfo();
			Console.ReadKey();
			$"----> Connexion sur : {urlArango}:{port}".ToConsoleInfo();

			GraphDemoDb arango = new GraphDemoDb(urlArango, port, projectName, login, password);
			//await arango.DeleteDatabase(databaseName);
			//"Suppression de la base {databaseName} - OK".ToConsoleResult();

			//await arango.CreateDatabase(databaseName);
			//$"Création de la base {databaseName} - OK".ToConsoleResult();

			//Console.WriteLine();
			//$"####### 2eme étape : Création du Graph {graphName} #######".ToConsoleInfo();
			//"Appuyer sur une touche pour commencer".ToConsoleInfo();
			//Console.ReadKey();
			//await arango.CreateGraph(databaseName, graphName);
			//"Graph créé - OK".ToConsoleResult();

			//Console.WriteLine();
			//"####### 3eme étape : Chargeons les systèmes et les Jumps en mémoire. #######".ToConsoleInfo();
			//"Appuyer sur une touche pour commencer".ToConsoleInfo();

			//string pathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
			//// --> Juste une Région : The Forge
			////string pathSystems = Path.Combine(pathBase, "AllSystemsTheForge.json");
			////string pathJumps = Path.Combine(pathBase, "AllJumpsTheForge.json");
			//// --> Ou tout l'univers !
			//string pathSystems = Path.Combine(pathBase, "systemSolar.json");
			//string pathJumps = Path.Combine(pathBase, "Jumps.json");

			//"Chargement des systèmes solaire...".ToConsoleInfo();
			//var jsonContentSystems = await File.ReadAllTextAsync(pathSystems);
			//List<SolarSystem> allSolarSystems = JsonSerializer.Deserialize<List<SolarSystem>>(jsonContentSystems);
			//$"# Il y a {allSolarSystems.Count} systèmes solaires.".ToConsoleResult();

			//"Chargement du fichier des jumps...".ToConsoleInfo();
			//string jsonContentJumps = await File.ReadAllTextAsync(pathJumps);
			//List<Jumps> allJumps = JsonSerializer.Deserialize<List<Jumps>>(jsonContentJumps);
			//$"# Il y a {allJumps.Count} sauts interstellaires possibles.".ToConsoleResult();

			//"Fin du chargement des fichiers...".ToConsoleInfo();

			//Console.WriteLine();
			//":> Injection des données pour créer le graph".ToConsoleInfo();
			//"Appuyer sur une touche pour continuer...".ToConsoleInfo();
			//Console.ReadKey();

			//"En cours d'injection....".ToConsoleInfo();
			//await arango.PopulateAsync(databaseName, graphName, allSolarSystems, allJumps);
			//"Graph créé !".ToConsoleResult();

			//Console.WriteLine();
			//"####### 3eme étape : Faisons quelques requêtes. #######".ToConsoleInfo();
			//"Appuyer sur une touche pour commencer".ToConsoleInfo();
			//Console.ReadKey();

			//"Récupération des systèmes avec une sécurité au moins de 0.5".ToConsoleInfo();
			//IEnumerable<SolarSystem> systemes = await arango.GetSystems(databaseName, 0.5);
			//$"Il y a {systemes.Count()} systèmes qui ont une sécurité supérieur ou égal à 0.5".ToConsoleInfo();

			"Faire un itinéraire le plus rapide entre 2 systèmes".ToConsoleInfo();
			"Appuyer sur une touche pour commencer".ToConsoleInfo();
			Console.ReadKey();
			List<SolarSystem> systemsPath = await arango.GetItineraireAsync(databaseName, graphName, "Airaken", "Reisen");
			for (int i = 0; i < systemsPath.Count(); i++)
			{
				$"Etape {i+1} - {systemsPath[i].SolarSystemName} - sécurité : {systemsPath[i].Securite}".ToConsoleResult();
			}

			Console.WriteLine();
			"######## Fin de l'application Démo ########".ToConsoleInfo();
		}
	}

	public static class ConsoleExtension
	{
		public static void ToConsoleInfo(this string message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(message);
		}

		public static void ToConsoleResult(this string message)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(message);
		}
	}
}
