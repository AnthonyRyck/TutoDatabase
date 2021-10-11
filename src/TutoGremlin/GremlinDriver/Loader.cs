using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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



		public void Populate()
		{
			GremlinRequest.AddV("post")
				.Property("name", "value")
				.Property("autreName", "autreValue")
				.As("p1") // A voir à quoi ça correspond ?

			.AddV("comment")
				.Property("name", "value")
				.Property("autreName", "autreValue")
				.As("c1")
			.AddE("nameEdge").From("p1").To("c1")

			.Iterate();
		}
	}
}
