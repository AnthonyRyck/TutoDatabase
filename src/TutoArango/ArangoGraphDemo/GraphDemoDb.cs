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
								Key = solar.SolarSystemId.ToString(),
								Name = solar.SolarSystemName,
								Securite = solar.Securite,
								RegionName = solar.RegionName
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
				Console.WriteLine("#################");
				Console.WriteLine("Error :");
				Console.WriteLine(ex.Message);
				Console.WriteLine("#################");
				Console.WriteLine(ex.StackTrace);
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

	}
}
