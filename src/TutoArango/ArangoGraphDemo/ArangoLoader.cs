using ConsoleArango.Models;
using Core.Arango;
using Core.Arango.Protocol;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleArango
{
	class ArangoLoader
	{
		private const string SOLAR_VERTICES = "solarSystem";
		private const string EDGES = "jumpEdge";
		private ArangoContext Arango;


		public ArangoLoader(string url, int port, string projectName, string login, string password)
		{
			Arango = new ArangoContext($"Server=http://{url}:{port};Realm={projectName};User={login};Password={password};");
		}

		/// <summary>
		/// Permet de créer la base de donnée.
		/// </summary>
		/// <param name="name">Nom de la base de donnée.</param>
		/// <returns></returns>
		internal async Task CreateDatabase(string name)
		{
			await Arango.Database.CreateAsync(name);
		}

		/// <summary>
		/// Supprime une base de donnée.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal async Task DropDatabase(string name)
		{
			await Arango.Database.DropAsync(name);
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

				// Création des Edges
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

		internal async Task GetSystems(string nomDatabase, string graphName, double minSecurite)
		{
			// new ArangoLoader("localhost", 8529, "tutoctrlaltsuppr", "root", "CtrlAltSuppr!");
			var ClientGremlin = new GremlinClient(new GremlinServer("localhost", 8529, false, "root", "CtrlAltSuppr!"));
			var GremlinRequest = AnonymousTraversalSource.Traversal().WithRemote(new DriverRemoteConnection(ClientGremlin));


			ArangoDBConfigurationBuilder builder = new ArangoDBConfigurationBuilder();
			builder.graph("modern")
				.withVertexCollection("software")
				.withVertexCollection("person")
				.withEdgeCollection("knows")
				.withEdgeCollection("created")
				.configureEdge("knows", "person", "person")
				.configureEdge("created", "person", "software");

			// use the default database (and user:password) or configure a different database
			// builder.arangoHosts("172.168.1.10:4456")
			//     .arangoUser("stripe")
			//     .arangoPassword("gizmo")


			BaseConfiguration conf = builder.build();
			Graph graph = GraphFactory.open(conf);
			GraphTraversalSource gts = new GraphTraversalSource(graph);


			//var allSystems = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has(REGION_NAME, regionName)
			//							.Project<Object>(SOLAR_SYSTEM_ID, SOLAR_SYSTEM_NAME, SECURITE, REGION_NAME)
			//							.By(SOLAR_SYSTEM_ID)
			//							.By(SOLAR_SYSTEM_NAME)
			//							.By(SECURITE)
			//							.By(REGION_NAME)
			//							.ToList();
		}

	}
}
