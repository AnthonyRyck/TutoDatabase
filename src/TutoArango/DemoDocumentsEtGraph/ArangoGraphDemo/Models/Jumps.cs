

namespace ArangoGraphDemo.Models
{
	public class Jumps
	{
		/// <summary>
		/// Nom du système solaire départ
		/// </summary>
		public string FromSystem { get; set; }

		/// <summary>
		/// ID du système solaire départ
		/// </summary>
		public int FromSystemID { get; set; }

		/// <summary>
		/// Nom du système solaire arrivé
		/// </summary>
		public string ToSystem { get; set; }

		/// <summary>
		/// ID du système solaire arrivé.
		/// </summary>
		public int ToSystemID { get; set; }
	}


	public static class JumpsExtension
	{
		public static string ToQueryAddEdge(this Jumps jump)
		{
			return $"g.V().has('solar',{jump.FromSystemID})"
				+ $".addE('jumpTo')" 
				+ $".to(g.V().has('solar',{jump.ToSystemID}))";
		}
	}
}
