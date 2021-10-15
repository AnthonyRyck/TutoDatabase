using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;
using GremlinDriver.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gremlin.Net.Structure.IO.GraphSON;

namespace GremlinDriver
{
	public class Loader : IDisposable
	{
		private GraphTraversalSource GremlinRequest;
		private GremlinClient ClientGremlin;


		private const string LABEL_VERTEX = "SystemSolar";
		private const string EDGE_NAME = "jumpTo";



		public Loader()
		{
			ClientGremlin = new GremlinClient(new GremlinServer("localhost", 8183));
			GremlinRequest = AnonymousTraversalSource.Traversal().WithRemote(new DriverRemoteConnection(ClientGremlin));
		}

		/// <summary>
		/// Supprime tous les Vextices
		/// </summary>
		public void DropDatabase()
		{
			try
			{
				GremlinRequest.V().Drop().Iterate();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ERROR : {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Ajoute les Vertices et les Edges.
		/// </summary>
		/// <param name="allSolarSystems"></param>
		/// <param name="allJumps"></param>
		/// <returns></returns>
		internal Task PopulateGraphAsync(List<SolarSystem> allSolarSystems, List<Jumps> allJumps)
		{
			try
			{
				return Task.Factory.StartNew(() => 
				{
					Console.WriteLine("--> Création des systèmes solaires.");
					int iSytem = 0;
					int totalSystem = allSolarSystems.Count;
					foreach (var system in allSolarSystems)
					{
						GremlinRequest.AddV(LABEL_VERTEX)
									.Property("SolarSystemId", system.SolarSystemId)
									.Property("SolarSystemName", system.SolarSystemName)
									.Property("Securite", system.Securite)
									.Property("RegionName", system.RegionName)
									.As(system.SolarSystemId.ToString())
									.Iterate();

						Console.WriteLine($"--> Système {iSytem++} sur {totalSystem} créé...");
					}

					Console.WriteLine("Création des Jumps entre les systèmes.");
					int totalJump = allJumps.Count;
					int counterJump = 0;

					foreach (var jump in allJumps)
					{
						var debut = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has("SolarSystemId", jump.FromSystemID);
						var arrive = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has("SolarSystemId", jump.ToSystemID);
						if (debut != null && arrive != null)
						{

						
						GremlinRequest.V().HasLabel(LABEL_VERTEX).Has("SolarSystemId", jump.FromSystemID)
									.AddE("jumpTo")
									//.To(GremlinRequest.V().Has("SolarSystemId", jump.ToSystemID))
									.To(__.V().Has("SolarSystemId", jump.ToSystemID))
									.Iterate();

						Console.WriteLine($"--> Jump {counterJump++} sur {totalJump} créé...");
						}
					}
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ERROR : {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Récupère tous les systèmes de la region donnée.
		/// </summary>
		/// <param name="regionName"></param>
		/// <returns></returns>
		internal Task<List<SolarSystem>> GetRegion(string regionName)
		{
			return Task.Factory.StartNew(() =>
			{
				// Retourne List<Dictionary<string, Object>
				// Liste de system avec le Dictionnaire : nom de la propriété et sa valeur.
				var allSystems = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has("RegionName", regionName)
										.Project<Object>("SolarSystemId", "SolarSystemName", "Securite", "RegionName")
										.By("SolarSystemId")
										.By("SolarSystemName")
										.By("Securite")
										.By("RegionName")
										.ToList();

				List<SolarSystem> systemsRegion = new List<SolarSystem>();
				foreach (var item in allSystems)
				{
					SolarSystem system = new SolarSystem();

					foreach (var prop in item)
					{
						// Key : correspond au nom de la propriété
						// Value : la valeur de la propriété
						var property = typeof(SolarSystem).GetProperty(prop.Key);
						property.SetValue(system, prop.Value);
					}

					systemsRegion.Add(system);
				}

				return systemsRegion;
			});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="regionName"></param>
		/// <param name="securiteMin"></param>
		/// <returns></returns>
		internal Task<List<SolarSystem>> GetRegion(string regionName, double securiteMin)
		{
			return Task.Factory.StartNew(() =>
			{
				// Retourne List<Dictionary<string, Object>
				// Liste de system avec le Dictionnaire : nom de la propriété et sa valeur.
				var allSystems = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has("RegionName", regionName)
										.Has("Securite", P.Gte(securiteMin))
										.Project<Object>("SolarSystemId", "SolarSystemName", "Securite", "RegionName")
										.By("SolarSystemId")
										.By("SolarSystemName")
										.By("Securite")
										.By("RegionName")
										.ToList();

				List<SolarSystem> systemsRegion = new List<SolarSystem>();
				foreach (var item in allSystems)
				{
					SolarSystem system = new SolarSystem();

					foreach (var prop in item)
					{
						// Key : correspond au nom de la propriété
						// Value : la valeur de la propriété
						var property = typeof(SolarSystem).GetProperty(prop.Key);
						property.SetValue(system, prop.Value);
					}

					systemsRegion.Add(system);
				}

				return systemsRegion;
			});
		}


		internal Task<List<SolarSystem>> GetSystemVoisin(string systemName)
		{
			return Task.Factory.StartNew(() =>
			{
				// Retourne List<Dictionary<string, Object>
				// Liste de system avec le Dictionnaire : nom de la propriété et sa valeur.
				var allSystems = GremlinRequest.V().HasLabel("SystemSolar").Has("SolarSystemName", systemName)
										.OutE()
										.OtherV()
										.Project<Object>("SolarSystemId", "SolarSystemName", "Securite", "RegionName")
										.By("SolarSystemId")
										.By("SolarSystemName")
										.By("Securite")
										.By("RegionName")
										.ToList();

				List<SolarSystem> systemsRegion = new List<SolarSystem>();
				foreach (var item in allSystems)
				{
					SolarSystem system = new SolarSystem();

					foreach (var prop in item)
					{
						// Key : correspond au nom de la propriété
						// Value : la valeur de la propriété
						var property = typeof(SolarSystem).GetProperty(prop.Key);
						property.SetValue(system, prop.Value);
					}

					systemsRegion.Add(system);
				}

				return systemsRegion;
			});
		}



		internal Task<List<SolarSystem>> GetItineraire(string systemDepart, string systemArrive)
		{
			// Requête Gremlin !
			//g.V().has('name', 'Airaken')
			//.repeat(out ().simplePath())
			//.until(has('name', 'Reisen'))
			//.path().limit(1)

			return Task.Factory.StartNew(() =>
			{
				try
				{
					// Retourne List<Dictionary<string, Object>
					// Liste de system avec le Dictionnaire : nom de la propriété et sa valeur.
					//var allSystems = GremlinRequest.V().HasLabel("SystemSolar").Has("SolarSystemName", systemDepart)
					//						.Repeat("itineraire", __.Out().SimplePath())
					//						.Until(__.HasLabel("SystemSolar").Has("SolarSystemName", systemArrive))
					//						.Path().Limit<Vertex>(1);
					//.Project<Object>("SolarSystemId", "SolarSystemName", "Securite", "RegionName")
					//.By("SolarSystemId")
					//.By("SolarSystemName")
					//.By("Securite")
					//.By("RegionName")
					//.ToList();

					var allSystems = ((GremlinRequest.V().HasLabel("SystemSolar").Has("SolarSystemName", systemDepart))
											.Repeat("itineraire", __.Out().SimplePath())
											.Until(__.HasLabel("SystemSolar").Has("SolarSystemName", systemArrive)))
											.Dedup().Path()
											.Limit<Vertex>(1)
											.Fold()
					.Project<Object>("SolarSystemId", "SolarSystemName", "Securite", "RegionName")
					.By("SolarSystemId")
					.By("SolarSystemName")
					.By("Securite")
					.By("RegionName")
					.ToList();

					List<SolarSystem> systemsRegion = new List<SolarSystem>();
					foreach (var item in allSystems)
					{
						SolarSystem system = new SolarSystem();

						foreach (var prop in item)
						{
							// Key : correspond au nom de la propriété
							// Value : la valeur de la propriété
							var property = typeof(SolarSystem).GetProperty(prop.Key);
							property.SetValue(system, prop.Value);
						}

						systemsRegion.Add(system);
					}

					return systemsRegion;

				}
				catch (Exception ex)
				{
					bool stop = true;
					throw;
				}
			});
		}


		internal Task<List<SolarSystem>> Test(string depart, string arrive)
		{
			return Task.Factory.StartNew(() =>
			{
				List<SolarSystem> systemsRegion = new List<SolarSystem>();

				try
				{
					// ServerError: The by("SolarSystemId") modulator can only be applied to a traverser that is an Element or a Map
					// - it is being applied to [path[v[41112], v[110776], v[118968], v[53296], v[41136]]] a ImmutablePath class instead
					var allSystems = GremlinRequest.V().HasLabel("SystemSolar").Has("SolarSystemName", depart)
											.Repeat(__.Out().SimplePath())
											.Until(__.HasLabel("SystemSolar").Has("SolarSystemName", arrive))
											.Path()
											//.By("SolarSystemId")
											//.By("SolarSystemName")
											//.By("Securite")
											//.By("RegionName")
											.Limit<Path>(1)

					//.Project<Object>("SolarSystemId", "SolarSystemName", "Securite", "RegionName")
					//.By("SolarSystemId")
					//.By("SolarSystemName")
					//.By("Securite")
					//.By("RegionName")

					.Next();
					//.ToList();

					bool stopIci = true;

					foreach (var systemRoute in allSystems.Objects)
					{
						SolarSystem system = new SolarSystem();

						var etapeItineraire = GremlinRequest.V(((Vertex)systemRoute).Id)
															.Project<Object>("SolarSystemId", "SolarSystemName", "Securite", "RegionName")
															.By("SolarSystemId")
															.By("SolarSystemName")
															.By("Securite")
															.By("RegionName")
															.Next();



						// Key : correspond au nom de la propriété
						// Value : la valeur de la propriété
						foreach (var etape in etapeItineraire)
						{
							//SolarSystem system = new SolarSystem();

							// Key : correspond au nom de la propriété
							// Value : la valeur de la propriété
							var property = typeof(SolarSystem).GetProperty(etape.Key);
							property.SetValue(system, etape.Value);

							systemsRegion.Add(system);
						}
						//foreach (var item in etapeItineraire)
						//{
						//	var property = typeof(SolarSystem).GetProperty(item);
						//	property.SetValue(system, prop.Value);
						//	systemsRegion.Add(system);
						//}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					Console.WriteLine("#############");
					Console.WriteLine(ex.StackTrace);
				}

				return systemsRegion;
			});
		}

		public void Dispose()
		{
			ClientGremlin.Dispose();
		}
	}
}
