using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using GremlinDriver.Models;

namespace GremlinDriver
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("##### Lancement de l'application ######");
			Console.WriteLine("# Appuyer sur une touche pour commencer l'injection.");
			Console.ReadKey();

			string pathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
			string pathSolarSystem = Path.Combine(pathBase, "systemSolar.json");
			string pathJumps = Path.Combine(pathBase, "Jumps.json");

			Console.WriteLine("Chargement du fichier des systèmes solaires...");
			string jsonContentSystems = File.ReadAllText(pathSolarSystem);
			List<SolarSystem> allSolarSystems = JsonSerializer.Deserialize<List<SolarSystem>>(jsonContentSystems);
			Console.WriteLine($"# Il y a {allSolarSystems.Count} systèmes solaires.");

			Console.WriteLine("Chargement du fichier des jumps...");
			string jsonContentJumps = File.ReadAllText(pathJumps);
			List<Jumps> allJumps = JsonSerializer.Deserialize<List<Jumps>>(jsonContentJumps);
			Console.WriteLine($"# Il y a {allJumps.Count} sauts interstellaires possibles.");
						
			Console.WriteLine("Fin du chargement des fichiers...");


			Console.WriteLine("# Début de la création du Graph !");

			Loader loadData = new Loader();

			Console.WriteLine("#--> Drop de la base");
			loadData.DropDatabase();

			Console.WriteLine("#--> Création des systèmes (un Vertex / des Vertices !)");
			Console.WriteLine("Petite pause.....");
			Task.Delay(1000).Wait();
			loadData.PopulateSolarSystemsAsync(allSolarSystems).Wait();

			Console.WriteLine("#--> Création des liens (Edge)");
			Console.WriteLine("Petite pause.....");
			Task.Delay(1000).Wait();
			loadData.PopulateJumpsAsync(allJumps).Wait();

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
