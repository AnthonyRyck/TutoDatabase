using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoConnect.Models
{
	public class ClientCommandes //: Client
	{
		//public List<Commande> Commandes { get; set; }


		//public ClientCommandes(int idClient, string prenom, string nom, string genre, 
		//			int age, Adresse adresse, List<Telephone> telephone, List<Commande> commandes) 
		//	: base(idClient, prenom, nom, genre, age, adresse, telephone)
		//{
		//	Commandes = commandes;
		//}

		public Client Client { get; set; }

		public Commande Commande { get; set; }
	} 
}
