using System;

namespace ArangoGraphDemo
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("######## Début de l'application Démo ########");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();

			string urlArango = "localhost";
			int port = 0;
			string databaseName = "ctrlaltsupprDb";
			
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("######################################################");
			Console.WriteLine("####### 1ere étape : Connexion à ArangoDb. #######");
			Console.WriteLine($"----> Connexion sur : {urlArango}:{port}");
			

			Console.WriteLine();
			Console.WriteLine("######## Fin de l'application Démo ########");
		}

		private static ToDelete()
		{
			Console.WriteLine("##### Début de l'application #####");
			Console.WriteLine("Appuyer sur une touche pour continuer...");
			Console.ReadKey();


			string pathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

			string pathSystemTheForge = Path.Combine(pathBase, "AllSystemsTheForge.json");
			string pathJumpsTheForge = Path.Combine(pathBase, "AllJumpsTheForge.json");

			Console.WriteLine("Chargement du fichier des systèmes solaires...");
			string jsonContentSystems = File.ReadAllText(pathSystemTheForge);

			List<SolarSystem> allSolarSystems = JsonSerializer.Deserialize<List<SolarSystem>>(jsonContentSystems);
			Console.WriteLine($"# Il y a {allSolarSystems.Count} systèmes solaires.");

			Console.WriteLine("Chargement du fichier des jumps...");
			string jsonContentJumps = File.ReadAllText(pathJumpsTheForge);

			List<Jumps> allJumps = JsonSerializer.Deserialize<List<Jumps>>(jsonContentJumps);
			Console.WriteLine($"# Il y a {allJumps.Count} sauts interstellaires possibles.");

			Console.WriteLine("Fin du chargement des fichiers...");

			Console.WriteLine(":> Création de la base de donnée");
			Console.WriteLine("Appuyer sur une touche pour continuer...");
			Console.ReadKey();

			ArangoLoader tuto = new ArangoLoader("localhost", 8529, "tutoctrlaltsuppr", "root", "CtrlAltSuppr!");
			string nomDatabase = "CtrlAltSupprBD";

			//await tuto.DropDatabase(nomDatabase);
			//await tuto.CreateDatabase(nomDatabase);

			//Console.WriteLine(":> Création d'un graph");
			//Console.WriteLine("Appuyer sur une touche pour continuer...");
			//Console.ReadKey();

			string graphName = "EveGraph";
			//await tuto.CreateGraph(nomDatabase, graphName);
			//await tuto.PopulateAsync(nomDatabase, graphName, allSolarSystems, allJumps);

			//Console.WriteLine("Graph créé !");
			//Console.WriteLine(":> Faisons quelques requêtes.");
			//Console.WriteLine("Appuyer sur une touche pour continuer...");
			//Console.ReadKey();

			await tuto.GetSystems(nomDatabase, graphName, 0.5);

			Console.WriteLine("##### Fin de l'application #####");
		}
	}
}
