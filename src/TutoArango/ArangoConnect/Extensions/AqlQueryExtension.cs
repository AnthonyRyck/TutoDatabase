using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ArangoConnect.Extensions
{
	public static class AqlQueryExtension
	{
		public static StringBuilder StartQuery(this string value)
		{
			var source = new StringBuilder();
			source.AppendLine(value);
			return source;
		}

		public static StringBuilder AddLineToQuery(this StringBuilder source, string value)
		{
			if (source == null)
				source = new StringBuilder();

			source.AppendLine(value);
			return source;
		}

		public static FormattableString ToQueryAql(this StringBuilder source)
		{
			return FormattableStringFactory.Create(source.ToString());
		}
	}
}
