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
	public class Loader
	{
		private GraphTraversalSource GremlinRequest;

		public Loader()
		{
			using (var client = new GremlinClient(new GremlinServer("localhost", 8182)))
			{
				GremlinRequest = AnonymousTraversalSource.Traversal().WithRemote(new DriverRemoteConnection(client));
			}
		}


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

		internal Task PopulateSolarSystemsAsync(List<SolarSystem> allSolarSystems)
		{
			try
			{
				return Task.Factory.StartNew(() => 
				{
					GraphTraversal<Vertex, Vertex> next = null;

					foreach (var system in allSolarSystems)
					{
						if (next == null)
						{
							next = GremlinRequest.AddV("SystemSolar")
									.Property("SolarSystemID", system.SolarSystemID)
									.Property("SolarSystemName", system.SolarSystemName)
									.Property("Securite", system.Securite)
									.Property("SecuriteClass", system.SecuriteClass)
									.Property("RegionName", system.RegionName)
									.As(system.SolarSystemName); //----> mettre As ?
						}
						else
						{
							next.AddV("SystemSolar")
									.Property("SolarSystemID", system.SolarSystemID)
									.Property("SolarSystemName", system.SolarSystemName)
									.Property("Securite", system.Securite)
									.Property("SecuriteClass", system.SecuriteClass)
									.Property("RegionName", system.RegionName)
									.As(system.SolarSystemName); //----> mettre As ?
						}
					}

					next.Iterate();
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ERROR : {ex.Message}");
				throw;
			}
		}



		internal Task PopulateJumpsAsync(List<Jumps> allJumps)
		{
			try
			{
				return Task.Factory.StartNew(() =>
				{
					GraphTraversal<Edge, Edge> next = null;

					foreach (var jump in allJumps)
					{
						if (next == null)
						{
							next = GremlinRequest.AddE("jumpTo").From(jump.FromSystem).To(jump.ToSystem);
						}
						else
						{
							next.AddE("jumpTo").From(jump.FromSystem).To(jump.ToSystem);
						}
					}

					next.Iterate();
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
