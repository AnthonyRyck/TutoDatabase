using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleGremlin.Models;
using ConsoleGremlin.Models.Graph;
using ExRam.Gremlinq.Core;

namespace ConsoleGremlin
{
	public class LoaderToDocker
	{
		private readonly IGremlinQuerySource _g;

		public LoaderToDocker(IGremlinQuerySource g)
		{
			_g = g;
		}

		internal async Task DropBase()
		{
			await _g
				.V()
				.Drop();
		}

		internal async Task CreateAllSystems(List<SolarSystem> solarSystems)
		{
			try
			{
				foreach (var system in solarSystems)
				{
					await _g.AddV(new SystemSolar
					{
						SolarSystemID = system.solarSystemID,
						SolarSystemName = system.solarSystemName,
						Securite = system.securite,
						SecuriteClass = system.securiteClass,
						RegionName = system.regionName
					}).FirstAsync();

					Console.WriteLine("Ajout du système : " + system.solarSystemName);
				}				
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERREUR sur le création des VERTICES : " + ex.Message);
			}
		}

		/// <summary>
		/// Création des relations entre les systèmes
		/// </summary>
		/// <param name="allJumps"></param>
		/// <returns></returns>
		internal async Task CreateEdges(List<Jumps> allJumps)
		{
			try
			{
				foreach (var jump in allJumps)
				{
					var systemDepart = await _g.V<SystemSolar>().Where(x => x.SolarSystemID == jump.FromSystemID);
					var systemArrive = await _g.V<SystemSolar>().Where(x => x.SolarSystemID == jump.ToSystemID);

					if(systemDepart.Length == 0 || systemArrive.Length == 0)
					{
						continue;
					}

					await _g.V(systemDepart.First().Id!)
							.AddE<JumpEdge>()
							.To(__ => __.V(systemArrive.First().Id!))
							.FirstAsync();

					Console.WriteLine("Ajout de la relation : " + systemDepart.First().SolarSystemName + " vers " + systemArrive.First().SolarSystemName);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERREUR sur le création des EDGES : " + ex.Message);
			}
		}

		/// <summary>
		/// Retourne tous les systèmes qui sont dans la région donnée.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal async Task GetRegion(string name)
		{

			var test = await _g.V<SystemSolar>().Where(x => x.RegionName == name).ToArrayAsync();

			Console.WriteLine("Il y a " + test.Length + " systèmes dans la région : " + name);
			await Task.Delay(2000);
			foreach (var item in test)
			{
				Console.WriteLine("Nom système : " + item.SolarSystemName);
			}
		}

		/// <summary>
		/// Retourne tous les systèmes qui sont dans la région donnée et avec
		/// une sécurité minimum.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal async Task GetRegion(string name, double securiteMin)
		{

			var test = await _g.V<SystemSolar>().Where(x => x.RegionName == name && x.Securite >= securiteMin).ToArrayAsync();

			Console.WriteLine("Il y a " + test.Length + " systèmes dans la région : " + name);
			await Task.Delay(2000);
			foreach (var item in test)
			{
				Console.WriteLine($"Nom système : {item.SolarSystemName} - sécurité : {item.Securite}");
			}
		}

		internal async Task GetItineraire(string departSystem, string arriveSystem)
		{
			try
			{
				// Requête Gremlin !
				//g.V().has('name', 'Airaken')
				//.repeat(out ().simplePath())
				//.until(has('name', 'Reisen'))
				//.path().limit(1)

				//var tt = await _g.V<SystemSolar>(57456).FirstAsync();
				//Console.WriteLine("--> " + tt.SolarSystemName);
				//var tt2 = await _g.V<SystemSolar>(45280).FirstAsync();
				//Console.WriteLine("--> " + tt2.SolarSystemName);
				//var tt3 = await _g.V<SystemSolar>(69640).FirstAsync();
				//Console.WriteLine("--> " + tt3.SolarSystemName);

				var depart = await _g.V<SystemSolar>().Where(x => x.SolarSystemName == departSystem)
									.FirstAsync();
				var arrive = await _g.V<SystemSolar>().Where(x => x.SolarSystemName == arriveSystem)
									.FirstAsync();

				//var requete = _g.V<SystemSolar>(depart.Id)
				//					.RepeatUntil(rep => rep.Out().SimplePath().Cast<SystemSolar>(),
				//								until => until.Where(x => x.SolarSystemName == arriveSystem))
				//					.Path().Limit(1)//;
				//.Debug();

				var temtt = await _g.V<SystemSolar>().Where(x => x.SolarSystemName == departSystem)
									.RepeatUntil(repeat => repeat.Out().SimplePath().Cast<SystemSolar>(),
												until => until.Where(x => x.SolarSystemName == arriveSystem))
									.Path().Limit(1);

				foreach (var item in temtt)
				{
					//item.Objects
					//foreach (var pp in item.Objects)
					//{
					//	var tt = (SystemSolar)pp;
					//	Console.WriteLine("TROUVE : " + tt.SolarSystemName);
					//}
				}

			}
			catch (Exception ex)
			{
				bool excep = true;
			}
		}

		/// <summary>
		/// Permet de récupérer les systèmes qui sont connectés au système demandé.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal async Task GetSystemVoisin(string name)
		{
			Console.WriteLine($"Système connecté à {name}");

			var test = await _g.V<SystemSolar>().Where(x => x.SolarSystemName == name)
								.Out<JumpEdge>()
								.OfType<SystemSolar>()
								.ToArrayAsync();

			foreach (var item in test)
			{
				Console.WriteLine("Nom système : " + item.SolarSystemName);
			}
		}


		internal async Task Get()
		{

		}
	}
}
