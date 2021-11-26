using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Arango.Protocol;
using Core.Arango.Linq;
using System.Linq;
using Core.Arango;
using ArangoConnect;
using ArangoGraphDemo.Models;
using ArangoConnect.Extensions;

namespace ArangoGraphDemo
{
	internal class GraphDemoDb : ArangoLoader
	{
		private const string SOLAR_VERTICES = "SolarSystem";
		private const string EDGES = "Jumps";


		public GraphDemoDb(string url, int port, string projectName, string login, string password)
			: base(url, port, projectName, login, password)
		{
		}

		/// <summary>
		/// Créer un graph sur la base de donnée fournit.
		/// </summary>
		/// <param name="nameDatabase">Nom de la base de donnée</param>
		/// <returns></returns>
		internal async Task CreateGraph(string nameDatabase, string graphName)
		{
			await Arango.Collection.CreateAsync(nameDatabase, SOLAR_VERTICES, ArangoCollectionType.Document);
			await Arango.Collection.CreateAsync(nameDatabase, EDGES, ArangoCollectionType.Edge);

			await Arango.Graph.CreateAsync(nameDatabase, new ArangoGraph
			{
				Name = graphName,
				EdgeDefinitions = new List<ArangoEdgeDefinition>
				{
					new()
					{
					  Collection = EDGES,
					  From = new List<string> {SOLAR_VERTICES},
					  To = new List<string> { SOLAR_VERTICES }
					}
				}
			});
		}

		/// <summary>
		/// Injecte les systèmes solaire et les liens entre eux.
		/// </summary>
		/// <param name="nomDatabase"></param>
		/// <param name="graphName"></param>
		/// <param name="allSolarSystems"></param>
		/// <param name="allJumps"></param>
		/// <returns></returns>
		internal async Task PopulateAsync(string nomDatabase, string graphName, List<SolarSystem> allSolarSystems, List<Jumps> allJumps)
		{
			try
			{
				int countSystem = allSolarSystems.Count;
				int countInsertSystem = 1;

				// Création des Vertices
				foreach (var solar in allSolarSystems)
				{
					await Arango.Graph.Vertex.CreateAsync(nomDatabase, graphName, SOLAR_VERTICES,
					new
					{
						// Bien mettre en ToString, sinon Exception
						// voir :
						// https://www.arangodb.com/docs/stable/data-modeling-naming-conventions-document-keys.html
						Key = solar.SolarSystemId.ToString(),
						SolarSystemName = solar.SolarSystemName,
						Securite = solar.Securite,
						RegionName = solar.RegionName,
						SolarSystemId = solar.SolarSystemId
					});

					Console.WriteLine($"Insertion de {solar.SolarSystemName} - {countInsertSystem++} sur {countSystem}");
				}


				int countEdge = allJumps.Count;
				int countInsertEdge = 1;

				// Création des relations
				foreach (var jump in allJumps)
				{
					await Arango.Graph.Edge.CreateAsync(nomDatabase, graphName, EDGES, new
					{
						Key = jump.FromSystemID.ToString() + "-" + jump.ToSystemID.ToString(),
						From = SOLAR_VERTICES + "/" + jump.FromSystemID,
						To = SOLAR_VERTICES + "/" + jump.ToSystemID,
						Label = "JumpTo"
					});

					Console.WriteLine($"Jump {countInsertEdge++} sur {countEdge} créé...");
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("#################");
				Console.WriteLine("Error :");
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine(ex.Message);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("#################");
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine(ex.StackTrace);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("#################");
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		/// <summary>
		/// Récupère les systèmes solaires par rapport à un niveau
		/// de sécurité.
		/// </summary>
		/// <param name="nomDatabase"></param>
		/// <param name="minSecurite"></param>
		/// <returns></returns>
		internal async Task<IEnumerable<SolarSystem>> GetSystems(string nomDatabase, double minSecurite)
		{
			return await Arango.Query<SolarSystem>(nomDatabase)
								.Where(x => x.Securite >= minSecurite)
								.ToListAsync();
		}

		/// <summary>
		/// Trouve le chemin le plus rapide entre 2 systèmes.
		/// </summary>
		/// <param name="systemDepart"></param>
		/// <param name="systemArrive"></param>
		/// <returns></returns>
		internal async Task<List<SolarSystem>> GetItineraireAsync(string databaseName, string graphName, string systemDepart, string systemArrive)
		{
			try
			{
				// ### Requête AQL pour avoir le chemin le plus court.
				// FOR sysDepart IN SolarSystem
				// FILTER sysDepart.SolarSystemName == 'systemDepart'
				// FOR sysArrive IN SolarSystem
				// FILTER sysArrive.SolarSystemName == 'systemArrive'
				// FOR path IN OUTBOUND SHORTEST_PATH
				// sysDepart._id TO sysArrive._id
				// GRAPH 'graphName'
				// RETURN path

				FormattableString query = "FOR sysDepart IN SolarSystem".StartQuery()
							.AddLineToQuery($"FILTER sysDepart.SolarSystemName == '{systemDepart}'")
							.AddLineToQuery("FOR sysArrive IN SolarSystem")
							.AddLineToQuery($"FILTER sysArrive.SolarSystemName == '{systemArrive}'")
							.AddLineToQuery("FOR path IN OUTBOUND SHORTEST_PATH")
							.AddLineToQuery("sysDepart._id TO sysArrive._id")
							.AddLineToQuery($"GRAPH '{graphName}'")
							.AddLineToQuery("RETURN path")
							.ToQueryAql();

				ArangoList<SolarSystem> resultJointure = await Arango.Query.ExecuteAsync<SolarSystem>(databaseName, query);
				return resultJointure.ToList();
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("#################");
				Console.WriteLine("Error :");
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine(ex.Message);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("#################");
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine(ex.StackTrace);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("#################");
				Console.ForegroundColor = ConsoleColor.White;

				return new List<SolarSystem>();
			}
		}

		/// <summary>
		/// Non fonctionnel, juste pour montrer l'Exception.
		/// </summary>
		/// <param name="systemDepart"></param>
		/// <param name="systemArrive"></param>
		/// <returns></returns>
		internal async Task<List<SolarSystem>> GetItineraireWithInterpolationAsync(string databaseName, string graphName, string systemDepart, string systemArrive)
		{
			try
			{
				// ### Juste pour montrer l'exception en utilisant cette méthode.
				FormattableString For1 = $"FOR sysDepart IN SolarSystem ";
				FormattableString Filter1 = $"FILTER sysDepart.SolarSystemName == '{systemDepart}' ";
				FormattableString For2 = $"FOR sysArrive IN SolarSystem ";
				FormattableString Filter2 = $"FILTER sysArrive.SolarSystemName == '{systemArrive}' ";
				FormattableString Path1 = $"FOR path IN OUTBOUND SHORTEST_PATH ";
				FormattableString Path2 = $"sysDepart._id TO sysArrive._id ";
				FormattableString Graph = $"GRAPH '{graphName}' ";
				FormattableString Return = $"RETURN path";

				ArangoList<SolarSystem> resultJointure = await Arango.Query.ExecuteAsync<SolarSystem>(databaseName, $"{For1} {Filter1} {For2} {Filter2} {Path1} {Path2} {Graph} {Return}");
				return resultJointure.ToList();
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("#################");
				Console.WriteLine("Error :");
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine(ex.Message);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("#################");
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine(ex.StackTrace);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("#################");
				Console.ForegroundColor = ConsoleColor.White;

				return new List<SolarSystem>();
			}
		}
		
	}
}
