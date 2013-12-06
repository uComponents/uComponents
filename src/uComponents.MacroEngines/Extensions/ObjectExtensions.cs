using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace uComponents.MacroEngines.Extensions
{
	/// <summary>
	/// Extension methods for <c>System.Object</c>.
	/// </summary>
	internal static class ObjectExtensions
	{
		/// <summary>
		/// Converts an <c>System.Object</c> to a <c>System.Collections.Generic.Dictionary</c>.
		/// </summary>
		/// <typeparam name="TVal">The type of the value.</typeparam>
		/// <param name="o">The <c>System.Object</c>.</param>
		/// <param name="ignoreProperties">The properties to ignore.</param>
		/// <returns>
		/// Returns a <c>System.Collections.Generic.Dictionary</c> representaiton of the <c>System.Object</c>.
		/// </returns>
		public static IDictionary<string, TVal> ToDictionary<TVal>(this object o, params string[] ignoreProperties)
		{
			if (o != null)
			{
				var props = TypeDescriptor.GetProperties(o);
				var d = new Dictionary<string, TVal>();
				foreach (var prop in props.Cast<PropertyDescriptor>().Where(x => !ignoreProperties.Contains(x.Name)))
				{
					var val = prop.GetValue(o);
					if (val != null)
					{
						d.Add(prop.Name, (TVal)val);
					}
				}
				return d;
			}
			return new Dictionary<string, TVal>();
		}
	}
}