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
		static async Task Main(string[] args)
		{
			Console.WriteLine("##### Lancement de l'application ######");
			Console.WriteLine("# Appuyer sur une touche pour commencer l'injection.");
			Console.ReadKey();

			string pathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

			//string pathSolarSystem = Path.Combine(pathBase, "systemSolar.json");
			//string pathJumps = Path.Combine(pathBase, "Jumps.json");

			string pathSystemTheForge = Path.Combine(pathBase, "AllSystemsTheForge.json");
			string pathJumpsTheForge = Path.Combine(pathBase, "AllJumpsTheForge.json");

			Console.WriteLine("Chargement du fichier des systèmes solaires...");
			//string jsonContentSystems = File.ReadAllText(pathSolarSystem);
			string jsonContentSystems = File.ReadAllText(pathSystemTheForge);

			List<SolarSystem> allSolarSystems = JsonSerializer.Deserialize<List<SolarSystem>>(jsonContentSystems);
			Console.WriteLine($"# Il y a {allSolarSystems.Count} systèmes solaires.");

			Console.WriteLine("Chargement du fichier des jumps...");
			//string jsonContentJumps = File.ReadAllText(pathJumps);
			string jsonContentJumps = File.ReadAllText(pathJumpsTheForge);

			List<Jumps> allJumps = JsonSerializer.Deserialize<List<Jumps>>(jsonContentJumps);
			Console.WriteLine($"# Il y a {allJumps.Count} sauts interstellaires possibles.");

			Console.WriteLine("Fin du chargement des fichiers...");


			Console.WriteLine("# Début de la création du Graph !");

			using (Loader loadData = new Loader())
			{
				Console.WriteLine("#--> Drop de la base");
				loadData.DropDatabase();

				Console.WriteLine("#--> Création des systèmes (un Vertex / des Vertices !)");
				loadData.PopulateGraphAsync(allSolarSystems, allJumps).Wait();
				Console.WriteLine("# --> Done");

				Console.WriteLine("#:> Système présent dans la région The Forge.");
				Console.WriteLine("Appuyer sur une touche pour continuer...");
				Console.ReadKey();
				var allSystemInTheForge = await loadData.GetRegion("The Forge");
				foreach (var item in allSystemInTheForge)
				{
					Console.WriteLine($"-> {item.SolarSystemName} - sécurité {item.Securite} - Région {item.RegionName}");
				}

				Console.WriteLine("#:> Système présent dans la région The Forge avec une sécurité supérieur à 0.5");
				Console.WriteLine("Appuyer sur une touche pour continuer...");
				Console.ReadKey();
				var allSystemUpTo = await loadData.GetRegion("The Forge", 0.5);
				foreach (var item in allSystemUpTo)
				{
					Console.WriteLine($"-> {item.SolarSystemName} - sécurité {item.Securite} - Région {item.RegionName}");
				}

				string systemDepart = "Airaken";
				string systemArrive = "Reisen";

				Console.WriteLine($"#:> Systèmes voisin du système {systemDepart}");
				Console.WriteLine("Appuyer sur une touche pour continuer...");
				Console.ReadKey();
				var systemJita = await loadData.GetSystemVoisin(systemDepart);
				foreach (var item in systemJita)
				{
					Console.WriteLine($"-> {item.SolarSystemName} - sécurité {item.Securite} - Région {item.RegionName}");
				}

				Console.WriteLine();
				Console.WriteLine("###################################################");
				Console.WriteLine();

				Console.WriteLine($"#:> Systèmes voisin du système {systemArrive}");
				Console.WriteLine("Appuyer sur une touche pour continuer...");
				Console.ReadKey();
				var systemItamo = await loadData.GetSystemVoisin(systemArrive);
				foreach (var item in systemItamo)
				{
					Console.WriteLine($"-> {item.SolarSystemName} - sécurité {item.Securite} - Région {item.RegionName}");
				}

				Console.WriteLine();
				Console.WriteLine("###################################################");
				Console.WriteLine();

				Console.WriteLine($"#:> Itinéraire le plus court pour aller de {systemDepart} à {systemArrive}");
				Console.WriteLine("Appuyer sur une touche pour continuer...");
				Console.ReadKey();
				var itineraire = await loadData.GetItineraire(systemDepart, systemArrive);
				foreach (var item in itineraire)
				{
					Console.WriteLine($"-> {item.SolarSystemName} - sécurité {item.Securite}");
				}

				Console.WriteLine();
				Console.WriteLine("###################################################");
				Console.WriteLine();

				Console.WriteLine($"#:> Itinéraire sans passer par du LowSec pour aller de {systemDepart} à {systemArrive}");
				Console.WriteLine("Appuyer sur une touche pour continuer...");
				Console.ReadKey();
				var itineraireNoLowSec = await loadData.GetItineraire(systemDepart, systemArrive, 0.5);
				foreach (var item in itineraireNoLowSec)
				{
					Console.WriteLine($"-> {item.SolarSystemName} - sécurité {item.Securite}");
				}
			}

			Console.WriteLine("##### FIN de l'application ######");
			Console.ReadKey();
		}
	}
}
