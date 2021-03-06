using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;
using GremlinDriver.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GremlinDriver
{
	public class Loader : IDisposable
	{
		private GraphTraversalSource GremlinRequest;
		private GremlinClient ClientGremlin;


		private const string LABEL_VERTEX = "SystemSolar";
		private const string EDGE_NAME = "jumpTo";
		private const string SOLAR_SYSTEM_ID = "SolarSystemId";
		private const string SOLAR_SYSTEM_NAME = "SolarSystemName";
		private const string REGION_NAME = "RegionName";
		private const string SECURITE = "Securite";

		public Loader()
		{
			ClientGremlin = new GremlinClient(new GremlinServer("localhost", 8182));
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
									.Property(SOLAR_SYSTEM_ID, system.SolarSystemId)
									.Property(SOLAR_SYSTEM_NAME, system.SolarSystemName)
									.Property(SECURITE, system.Securite)
									.Property(REGION_NAME, system.RegionName)
									.Iterate();

						Console.WriteLine($"--> Système {iSytem++} sur {totalSystem} créé...");
					}

					Console.WriteLine("Création des Jumps entre les systèmes.");
					int totalJump = allJumps.Count;
					int counterJump = 0;

					foreach (var jump in allJumps)
					{
						var debut = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has(SOLAR_SYSTEM_ID, jump.FromSystemID);
						var arrive = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has(SOLAR_SYSTEM_ID, jump.ToSystemID);
						if (debut != null && arrive != null)
						{

						
						GremlinRequest.V().HasLabel(LABEL_VERTEX).Has(SOLAR_SYSTEM_ID, jump.FromSystemID)
									.AddE(EDGE_NAME)
									.To(__.V().Has(SOLAR_SYSTEM_ID, jump.ToSystemID))
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
				var allSystems = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has(REGION_NAME, regionName)
										.Project<Object>(SOLAR_SYSTEM_ID, SOLAR_SYSTEM_NAME, SECURITE, REGION_NAME)
										.By(SOLAR_SYSTEM_ID)
										.By(SOLAR_SYSTEM_NAME)
										.By(SECURITE)
										.By(REGION_NAME)
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
		/// Récupère tous les systèmes de la région donnée, mais qui
		/// ont une sécurité minimal.
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
				var allSystems = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has(REGION_NAME, regionName)
										.Has(SECURITE, P.Gte(securiteMin))
										.Project<Object>(SOLAR_SYSTEM_ID, SOLAR_SYSTEM_NAME, SECURITE, REGION_NAME)
										.By(SOLAR_SYSTEM_ID)
										.By(SOLAR_SYSTEM_NAME)
										.By(SECURITE)
										.By(REGION_NAME)
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
		/// Récupère les systèmes voisins (connectés) du système donnée.
		/// </summary>
		/// <param name="systemName"></param>
		/// <returns></returns>
		internal Task<List<SolarSystem>> GetSystemVoisin(string systemName)
		{
			return Task.Factory.StartNew(() =>
			{
				// Retourne List<Dictionary<string, Object>
				// Liste de system avec le Dictionnaire : nom de la propriété et sa valeur.
				var allSystems = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has(SOLAR_SYSTEM_NAME, systemName)
										.OutE()
										.OtherV()
										.Project<Object>(SOLAR_SYSTEM_ID, SOLAR_SYSTEM_NAME, SECURITE, REGION_NAME)
										.By(SOLAR_SYSTEM_ID)
										.By(SOLAR_SYSTEM_NAME)
										.By(SECURITE)
										.By(REGION_NAME)
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
		/// Donne l'itinéraire le plus rapide entre les 2 systèmes donnés.
		/// </summary>
		/// <param name="depart"></param>
		/// <param name="arrive"></param>
		/// <returns></returns>
		internal Task<List<SolarSystem>> GetItineraire(string depart, string arrive)
		{
			return Task.Factory.StartNew(() =>
			{
				List<SolarSystem> systemsRegion = new List<SolarSystem>();

				try
				{
					Path allSystems = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has(SOLAR_SYSTEM_NAME, depart)
											.Repeat(__.Out().SimplePath())
											.Until(__.HasLabel(LABEL_VERTEX).Has(SOLAR_SYSTEM_NAME, arrive))
											.Path()
											.Limit<Path>(1)
											.Next();

					foreach (var systemRoute in allSystems.Objects)
					{
						SolarSystem system = new SolarSystem();

						var etapeItineraire = GremlinRequest.V(((Vertex)systemRoute).Id)
															.Project<Object>(SOLAR_SYSTEM_ID, SOLAR_SYSTEM_NAME, SECURITE, REGION_NAME)
															.By(SOLAR_SYSTEM_ID)
															.By(SOLAR_SYSTEM_NAME)
															.By(SECURITE)
															.By(REGION_NAME)
															.Next();

						// Key : correspond au nom de la propriété
						// Value : la valeur de la propriété
						foreach (var etape in etapeItineraire)
						{
							// Key : correspond au nom de la propriété
							// Value : la valeur de la propriété
							var property = typeof(SolarSystem).GetProperty(etape.Key);
							property.SetValue(system, etape.Value);
						}

						systemsRegion.Add(system);
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

		/// <summary>
		/// Donne l'itinéraire le plus rapide entre les 2 systèmes donnés,
		/// et en passant QUE par des systèmes ayant au minimum la sécurité donnée.
		/// </summary>
		/// <param name="depart"></param>
		/// <param name="arrive"></param>
		/// <param name="minSecurite"></param>
		/// <returns></returns>
		internal Task<List<SolarSystem>> GetItineraire(string depart, string arrive, double minSecurite)
		{
			return Task.Factory.StartNew(() =>
			{
				List<SolarSystem> systemsRegion = new List<SolarSystem>();

				try
				{
					Path allSystems = GremlinRequest.V().HasLabel(LABEL_VERTEX).Has(SOLAR_SYSTEM_NAME, depart)
											.Repeat(__.Out()
														.Has(SECURITE, P.Gte(minSecurite))
														.SimplePath())
											.Until(__.HasLabel(LABEL_VERTEX).Has(SOLAR_SYSTEM_NAME, arrive))
											.Path()
											.Limit<Path>(1)
											.Next();

					foreach (var systemRoute in allSystems.Objects)
					{
						SolarSystem system = new SolarSystem();

						var etapeItineraire = GremlinRequest.V(((Vertex)systemRoute).Id)
															.Project<Object>(SOLAR_SYSTEM_ID, SOLAR_SYSTEM_NAME, SECURITE, REGION_NAME)
															.By(SOLAR_SYSTEM_ID)
															.By(SOLAR_SYSTEM_NAME)
															.By(SECURITE)
															.By(REGION_NAME)
															.Next();

						// Key : correspond au nom de la propriété
						// Value : la valeur de la propriété
						foreach (var etape in etapeItineraire)
						{
							// Key : correspond au nom de la propriété
							// Value : la valeur de la propriété
							var property = typeof(SolarSystem).GetProperty(etape.Key);
							property.SetValue(system, etape.Value);
						}

						systemsRegion.Add(system);
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
