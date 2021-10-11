

namespace GremlinDriver.Models
{
	public class SolarSystem
	{
		/// <summary>
		/// ID du système solaire
		/// </summary>
		public int SolarSystemId { get; set; }

		/// <summary>
		/// Nom du système solaire.
		/// </summary>
		public string SolarSystemName { get; set; }

		/// <summary>
		/// Niveau de sécurité
		/// </summary>
		public double Securite { get; set; }

		/// <summary>
		/// Nom de la région d'appartenance.
		/// </summary>
		public string RegionName { get; set; }
	}




	//public static class SolarSystemExtension
	//{
	//	public static string ToQueryAddVertex(this SolarSystem system)
	//	{
	//		return $"g.addV('SystemSolar')"
	//			+ $".property('solar', {system.SolarSystemID})"
	//			+ $".property('name', '{system.solarSystemName}')"
	//			+ $".property('security', {system.securite.ToString(CultureInfo.InvariantCulture)})"
	//			+ $".property('region', '{system.regionName}')";
	//	}
	//}
}
