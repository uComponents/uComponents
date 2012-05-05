using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using umbraco.IO;
using uComponents.Core;

namespace uComponents.DataTypes
{
	/// <summary>
	/// Global settings for uComponents.
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// The base directory for additional uComponents files.
		/// </summary>
		public static readonly string BaseDirName = string.Concat(SystemDirectories.Umbraco, "/plugins/", Constants.ApplicationName);

		/// <summary>
		/// The output format for a data-type.
		/// </summary>
		public enum OutputFormat
		{
			/// <summary>
			/// Outputs as CSV.
			/// </summary>
			CSV = 0,

			/// <summary>
			/// Outputs as XML.
			/// </summary>
			XML = 1
		}

		/// <summary>
		/// Gets the folder that stores additional uComponents files.
		/// </summary>
		/// <value>The base  directory for uComponents files.</value>
		public static DirectoryInfo BaseDir
		{
			get
			{
				var dir = new DirectoryInfo(IOHelper.MapPath(BaseDirName));

				if (!dir.Exists)
				{
					dir.Create();
				}

				return dir;
			}
		}
	}
}