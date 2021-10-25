using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoConnect
{
	internal class DemoDb : ArangoLoader
	{

		public DemoDb(string url, int port, string projectName, string login, string password) 
			: base(url, port, projectName, login, password)
		{
		}
	}
}
