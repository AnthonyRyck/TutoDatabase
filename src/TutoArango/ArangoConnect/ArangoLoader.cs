﻿using Core.Arango;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoConnect
{
	internal class ArangoLoader
	{
		private ArangoContext Arango;



		public ArangoLoader(string url, int port, string projectName, string login, string password)
		{
			Arango = new ArangoContext($"Server=http://{url}:{port};Realm={projectName};User={login};Password={password}");
		}


		/// <summary>
		/// Permet de créer une base de donnée
		/// </summary>
		/// <param name="name">Nom de la base de donnée</param>
		/// <returns></returns>
		protected async Task CreateDatabase(string name)
		{
			await Arango.Database.CreateAsync(name);
		}

		/// <summary>
		/// Permet de supprimer une base de donnée
		/// </summary>
		/// <param name="name">Nom de la base</param>
		/// <returns></returns>
		protected async Task DeleteDatabase(string name)
		{
			await Arango.Database.DropAsync(name);
		}
	}
}
