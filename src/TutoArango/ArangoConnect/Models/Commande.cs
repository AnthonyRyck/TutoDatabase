using System.Collections.Generic;

namespace ArangoConnect.Models
{
	public class Commande
	{
		public int IdCommand { get; set; }
		
		public int ClientId { get; set; }

		public List<Panier> Panier { get; set; }

	}
}
