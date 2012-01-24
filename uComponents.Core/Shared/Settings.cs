using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using umbraco.IO;

namespace uComponents.Core.Shared
{
	/// <summary>
	/// Global settings for uComponents.
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// Name of the application.
		/// </summary>
		public const string APPNAME = "uComponents";

		/// <summary>
		/// A comma (char).
		/// </summary>
		public const char COMMA = ',';

		/// <summary>
		/// File extension for ASP.NET webpages.
		/// </summary>
		public const string DOTASPX = ".aspx";

		/// <summary>
		/// A hash (string).
		/// </summary>
		public const string HASH = "#";

		/// <summary>
		/// HTTP protocol/prefix.
		/// </summary>
		public const string HTTP = "http://";

		/// <summary>
		/// HTTPS protocol/prefix.
		/// </summary>
		public const string HTTPS = "https://";

		/// <summary>
		/// A forward-slash (char).
		/// </summary>
		public const char SLASH = '/';

		/// <summary>
		/// AppSettings key for UI Modules' drag-n-drop.
		/// </summary>
		public const string AppKey_DragAndDrop = "ucomponents:DragAndDrop";

		/// <summary>
		/// AppSettings key for UI Modules' keyboard shortcuts.
		/// </summary>
		public const string AppKey_KeyboardShortcuts = "ucomponents:KeyboardShortcuts";

		/// <summary>
		/// AppSettings key for UI Modules' tray peek.
		/// </summary>
		public const string AppKey_TrayPeek = "ucomponents:TrayPeek";

		/// <summary>
		/// Dictionary for the UI Modules' appSettings keys.
		/// </summary>
		public static readonly Dictionary<string, string> AppKeys_UiModules = new Dictionary<string, string>()
			{
				{ AppKey_DragAndDrop, "Drag-n-drop" },
				{ AppKey_TrayPeek, "Tray Peek" }
			};

		/// <summary>
		/// The base directory for additional uComponents files.
		/// </summary>
		public static readonly string BaseDirName = string.Concat(SystemDirectories.Umbraco, "/plugins/", APPNAME);

		/// <summary>
		/// The resource path for the favicon.
		/// </summary>
		public const string FaviconResourcePath = "uComponents.Core.Shared.Resources.Images.favicon.ico";

		/// <summary>
		/// The resource path for the Prevalue Editor stylesheet.
		/// </summary>
		public const string PrevalueEditorCssResourcePath = "uComponents.Core.Shared.Resources.Styles.PrevalueEditor.css";

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
		/// Gets the uComponents version number.
		/// </summary>
		/// <value>The uComponents version number.</value>
		public static Version Version
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}
	}
}