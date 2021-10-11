using System;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;

namespace GremlinDriver
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("##### Lancement de l'application ######");
			Console.WriteLine("# Appuyer sur une touche pour commencer l'injection.");
			Console.ReadKey();






			//string pathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
			//string pathSolarSystem = Path.Combine(pathBase, "systemSolar.json");
			//string pathJumps = Path.Combine(pathBase, "Jumps.json");

			//Console.WriteLine("Chargement du fichier des systèmes solaires...");
			//string jsonContentSystems = File.ReadAllText(pathSolarSystem);
			//List<SolarSystem> allSolarSystems = JsonSerializer.Deserialize<List<SolarSystem>>(jsonContentSystems);
			//Console.WriteLine($"# Il y a {allSolarSystems.Count} systèmes solaires.");

			//Console.WriteLine("Chargement du fichier des jumps...");
			//string jsonContentJumps = File.ReadAllText(pathJumps);
			//List<Jumps> allJumps = JsonSerializer.Deserialize<List<Jumps>>(jsonContentJumps);
			//Console.WriteLine($"# Il y a {allJumps.Count} sauts interstellaires possibles.");

			//Console.WriteLine("Fin du chargement des fichiers...");


			//Console.WriteLine("# Début de la création du Graph !");

			//var gremlinQuerySource = g.ConfigureEnvironment(env => env.UseModel(GraphModel
			//												.FromBaseTypes<Models.Graph.Vertex, Models.Graph.Edge>(lookup => lookup.IncludeAssembliesOfBaseTypes()))
			//	.UseJanusGraph(builder => builder
			//			.AtLocalhost()));

			//LoaderToDocker loadData = new LoaderToDocker(gremlinQuerySource);

			//Console.WriteLine("#--> Drop e la base");
			//loadData.DropBase().Wait();

			//Console.WriteLine("#--> Création des systèmes (un Vertex / des Vertices !)");
			//Console.WriteLine("Petite pause.....");
			//Task.Delay(1000).Wait();
			//loadData.CreateAllSystems(allSolarSystems).Wait();

			//Console.WriteLine("#--> Création des liens (Edge)");
			//Console.WriteLine("Petite pause.....");
			//Task.Delay(1000).Wait();
			//loadData.CreateEdges(allJumps).Wait();

			//loadData.GetRegion("The Forge").Wait();
			//loadData.GetRegion("The Forge", 0.5).Wait();
			//loadData.GetSystemVoisin("Jita").Wait();
			//loadData.GetSystemVoisin("Itamo").Wait();
			//loadData.GetItineraire("Jita", "Itamo").Wait();

			Console.WriteLine("##### FIN de l'application ######");
			Console.ReadKey();
		}
	}
}
