using Core.Arango;
using System.Threading.Tasks;

namespace ArangoConnect
{
	public abstract class ArangoLoader
	{
		protected ArangoContext Arango;

		public ArangoLoader(string url, int port, string projectName, string login, string password)
		{
			Arango = new ArangoContext($"Server=http://{url}:{port};Realm={projectName};User={login};Password={password}");
		}


		/// <summary>
		/// Permet de créer une base de donnée
		/// </summary>
		/// <param name="name">Nom de la base de donnée</param>
		/// <returns></returns>
		public async Task CreateDatabase(string name)
		{
			await Arango.Database.CreateAsync(name);
		}

		/// <summary>
		/// Permet de supprimer une base de donnée
		/// </summary>
		/// <param name="name">Nom de la base</param>
		/// <returns></returns>
		public async Task DeleteDatabase(string name)
		{
			await Arango.Database.DropAsync(name);
		}
	}
}
