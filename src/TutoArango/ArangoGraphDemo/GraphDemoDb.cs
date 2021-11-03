using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Arango.Protocol;
using ArangoConnect.Models;
using Core.Arango.Linq;
using System.Linq;
using Core.Arango;
using ArangoConnect;
using ArangoGraphDemo.Models;
using System.Text;
using System.Runtime.CompilerServices;

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
				}

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
		internal async Task<IEnumerable<SolarSystem>> GetItineraireAsync(string databaseName, string graphName, string systemDepart, string systemArrive)
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

				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("FOR sysDepart IN SolarSystem");
				stringBuilder.AppendLine($"FILTER sysDepart.SolarSystemName == '{systemDepart}'");
				stringBuilder.AppendLine("FOR sysArrive IN SolarSystem");
				stringBuilder.AppendLine($"FILTER sysArrive.SolarSystemName == '{systemArrive}'");
				stringBuilder.AppendLine("FOR path IN OUTBOUND SHORTEST_PATH");
				stringBuilder.AppendLine("sysDepart._id TO sysArrive._id");
				stringBuilder.AppendLine($"GRAPH '{graphName}'");
				stringBuilder.AppendLine("RETURN path");

				StringBuilder autreFacon = "FOR sysDepart IN SolarSystem".StartQuery()
							.AddLineToQuery($"FILTER sysDepart.SolarSystemName == '{systemDepart}'")
							.AddLineToQuery("FOR sysArrive IN SolarSystem")
							.AddLineToQuery($"FILTER sysArrive.SolarSystemName == '{systemArrive}'")
							.AddLineToQuery("FOR path IN OUTBOUND SHORTEST_PATH")
							.AddLineToQuery("sysDepart._id TO sysArrive._id")
							.AddLineToQuery($"GRAPH '{graphName}'")
							.AddLineToQuery("RETURN path");

				FormattableString query = autreFacon.ToQueryAql();
				
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

				return Enumerable.Empty<SolarSystem>();
			}
		}
	}


	public static class AqlQueryExtension
	{
		public static StringBuilder StartQuery(this string value)
		{
			var source = new StringBuilder();
			source.AppendLine(value);
			return source;
		}

		public static StringBuilder AddLineToQuery(this StringBuilder source, string value)
		{
			if(source == null)
				source = new StringBuilder();
			
			source.AppendLine(value);
			return source;
		}

		public static FormattableString ToQueryAql(this StringBuilder source)
		{
			return FormattableStringFactory.Create(source.ToString());
		}
	}
}
