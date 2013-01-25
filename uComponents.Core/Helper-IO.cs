using System.Collections.Generic;
using System.IO;
using System.Linq;
using umbraco.IO;

namespace uComponents.Core
{
	/// <summary>
	/// Generic helper methods
	/// </summary>
	internal static partial class Helper
	{
		/// <summary>
		/// IO helpers
		/// </summary>
		public static class IO
		{
			/// <summary>
			/// Gets the assemblies.
			/// </summary>
			/// <returns>Returns a list of assembly names.</returns>
			public static string[] GetAssemblies()
			{
				var assemblies = new List<string>();

				// check if the App_Code directory exists and has any files
				var appCode = new DirectoryInfo(IOHelper.MapPath("~/App_Code"));
				if (appCode != null && appCode.Exists && appCode.GetFiles().Length > 0)
				{
					assemblies.Add(appCode.Name);
				}

				// add assemblies from the /bin directory
				assemblies.AddRange(Directory.GetFiles(IOHelper.MapPath("~/bin"), "*.dll").Select(fileName => fileName.Substring(fileName.LastIndexOf('\\') + 1)));

				return assemblies.ToArray();
			}
		}
	}
}