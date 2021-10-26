using System.Collections.Generic;

namespace ArangoConnect.Models
{
	public class Commande
	{
		public string ClientId { get; set; }

		public int IdCommand { get; set; }

		public List<Panier> Panier { get; set; }

	}
}
