using System.Collections.Generic;

namespace ArangoDemo.Models
{
    public class Client //: ArangoId ---> pas obligatoire.
    {
		public int IdClient { get; set; }

        public string Prenom { get; set; }

        public string Nom { get; set; }

        public string Genre { get; set; }

        public int Age { get; set; }

        public Adresse Adresse { get; set; }

        public List<Telephone> Telephone { get; set; }

        public Client(int idClient, string prenom, string nom, string genre, int age, Adresse adresse, List<Telephone> telephone)
        {
            IdClient = idClient;
            Prenom = prenom;
            Nom = nom;
            Genre = genre;
            Age = age;
            Adresse = adresse;
            Telephone = telephone;
        }

		public Client() {}


		public override string ToString()
		{
			return IdClient + " - " 
                + Nom + " - " 
                + Prenom + " - "
                + Age + " ans - " 
                + "Sexe : " + Genre;
		}
	}

    public class Adresse
    {
        public string Numero { get; set; }

        public string Rue { get; set; }

        public string Ville { get; set; }
    }

    public class Telephone
    {
        public string Type { get; set; }

        public string Number { get; set; }
    }
}
