

namespace ConsoleGremlin.Models
{
	public class SolarSystem
	{
		/// <summary>
		/// ID du système solaire
		/// </summary>
		public int solarSystemID { get; set; }

		/// <summary>
		/// Nom du système solaire.
		/// </summary>
		public string solarSystemName { get; set; }

		/// <summary>
		/// Niveau de sécurité
		/// </summary>
		public double securite { get; set; }

		/// <summary>
		/// Classe de sécurité
		/// </summary>
		public string securiteClass { get; set; }

		/// <summary>
		/// Nom de la région d'appartenance.
		/// </summary>
		public string regionName { get; set; }
	}




	//public static class SolarSystemExtension
	//{
	//	public static string ToQueryAddVertex(this SolarSystem system)
	//	{
	//		return $"g.addV('SystemSolar')"
	//			+ $".property('solar', {system.solarSystemID})"
	//			+ $".property('name', '{system.solarSystemName}')"
	//			+ $".property('security', {system.securite.ToString(CultureInfo.InvariantCulture)})"
	//			+ $".property('region', '{system.regionName}')";
	//	}
	//}
}
