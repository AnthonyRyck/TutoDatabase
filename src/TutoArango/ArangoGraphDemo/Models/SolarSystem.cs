

namespace ArangoGraphDemo.Models
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
}
