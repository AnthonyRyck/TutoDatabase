

namespace ConsoleGremlin.Models.Graph
{
	
	public class SystemSolar : Vertex
	{
		/// <summary>
		/// ID du système solaire
		/// </summary>
		public int SolarSystemID { get; set; }

		/// <summary>
		/// Nom du système solaire.
		/// </summary>
		public string SolarSystemName { get; set; }

		/// <summary>
		/// Niveau de sécurité
		/// </summary>
		public double Securite { get; set; }

		/// <summary>
		/// Classe de sécurité
		/// </summary>
		public string SecuriteClass { get; set; }

		/// <summary>
		/// Nom de la région d'appartenance.
		/// </summary>
		public string RegionName { get; set; }
	}
}
