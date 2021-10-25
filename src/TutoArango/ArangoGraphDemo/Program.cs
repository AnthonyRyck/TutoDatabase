using System;

namespace ArangoGraphDemo
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("######## Début de l'application Démo ########");
			Console.WriteLine("#:> Appuyer sur une touche pour commencer.");
			Console.ReadKey();

			string urlArango = "localhost";
			int port = 0;
			string databaseName = "ctrlaltsupprDb";
			
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("######################################################");
			Console.WriteLine("####### 1ere étape : Connexion à ArangoDb. #######");
			Console.WriteLine($"----> Connexion sur : {urlArango}:{port}");


			Console.WriteLine();
			Console.WriteLine("######## Fin de l'application Démo ########");
		}
	}
}
