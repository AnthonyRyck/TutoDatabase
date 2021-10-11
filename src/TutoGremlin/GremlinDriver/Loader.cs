using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;
using GremlinDriver.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GremlinDriver
{
	public class Loader
	{
		private GraphTraversalSource GremlinRequest;

		public Loader()
		{
			var client = new GremlinClient(new GremlinServer("localhost", 8183));
			GremlinRequest = AnonymousTraversalSource.Traversal().WithRemote(new DriverRemoteConnection(client));
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
					foreach (var system in allSolarSystems)
					{
						GremlinRequest.AddV("SystemSolar")
									.Property("SolarSystemID", system.SolarSystemID)
									.Property("SolarSystemName", system.SolarSystemName)
									.Property("Securite", system.Securite)
									.Property("SecuriteClass", system.SecuriteClass)
									.Property("RegionName", system.RegionName)
									.As(system.SolarSystemID.ToString())
									.Iterate();
					}

					Console.WriteLine("Création des Jumps entre les systèmes.");
					foreach (var jump in allJumps)
					{
						if(allSolarSystems.Any(x => x.SolarSystemID == jump.ToSystemID)
							&& allSolarSystems.Any(x => x.SolarSystemID == jump.FromSystemID))
						{
							GremlinRequest.V().HasLabel("SystemSolar").Has("SolarSystemID", jump.FromSystemID)
										.AddE("jumpTo")
										.To(GremlinRequest.V().Has("SolarSystemID", jump.ToSystemID))
										.Iterate();
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
	}
}
