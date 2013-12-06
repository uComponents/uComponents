using System;
using System.IO;
using System.Web.Configuration;
using uComponents.Core;
using Umbraco.Core.IO;

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

		/// <summary>
		/// Private static backing field for <c>RazorModelBindingEnabled</c>.
		/// </summary>
		private static bool? razorModelBindingEnabled = null;

		/// <summary>
		/// Gets a value indicating whether [razor model binding enabled].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [razor model binding enabled]; otherwise, <c>false</c>.
		/// </value>
		public static bool RazorModelBindingEnabled
		{
			get
			{
				if (!razorModelBindingEnabled.HasValue)
				{
					var enableRazorModelBinding = true;
					var appSettingValue = WebConfigurationManager.AppSettings[Constants.AppKey_RazorModelBinding] ?? bool.TrueString;

					bool.TryParse(appSettingValue, out enableRazorModelBinding);

					razorModelBindingEnabled = enableRazorModelBinding;
				}

				return razorModelBindingEnabled.Value;
			}
		}
	}
}