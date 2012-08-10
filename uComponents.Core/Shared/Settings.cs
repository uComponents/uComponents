using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using umbraco.IO;
using System.Configuration;

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
		[Obsolete("Please use uComponents.Core.Shared.Constants.ApplicationName")]
		public const string APPNAME = Constants.ApplicationName;

		/// <summary>
		/// A comma (char).
		/// </summary>
		[Obsolete("Please use uComponents.Core.Shared.Constants.Common.COMMA")]
		public const char COMMA = Constants.Common.COMMA;

		/// <summary>
		/// File extension for ASP.NET webpages.
		/// </summary>
		[Obsolete("Please use uComponents.Core.Shared.Constants.Common.DOTASPX")]
		public const string DOTASPX = Constants.Common.DOTASPX;

		/// <summary>
		/// A hash (string).
		/// </summary>
		[Obsolete("Please use uComponents.Core.Shared.Constants.Common.HASH")]
		public const string HASH = Constants.Common.HASH;

		/// <summary>
		/// HTTP protocol/prefix.
		/// </summary>
		[Obsolete("Please use uComponents.Core.Shared.Constants.Common.HTTP")]
		public const string HTTP = Constants.Common.HTTP;

		/// <summary>
		/// HTTPS protocol/prefix.
		/// </summary>
		[Obsolete("Please use uComponents.Core.Shared.Constants.Common.HTTPS")]
		public const string HTTPS = Constants.Common.HTTPS;

		/// <summary>
		/// A forward-slash (char).
		/// </summary>
		[Obsolete("Please use uComponents.Core.Shared.Constants.Common.SLASH")]
		public const char SLASH = Constants.Common.SLASH;

		/// <summary>
		/// AppSettings key for UI Modules' drag-n-drop.
		/// </summary>
		[Obsolete("Please use uComponents.Core.Shared.Constants.AppKey_DragAndDrop")]
		public const string AppKey_DragAndDrop = Constants.AppKey_DragAndDrop;

		/// <summary>
		/// AppSettings key for UI Modules' keyboard shortcuts.
		/// </summary>
		[Obsolete("Please use uComponents.Core.Shared.Constants.AppKey_KeyboardShortcuts")]
		public const string AppKey_KeyboardShortcuts = Constants.AppKey_KeyboardShortcuts;

		/// <summary>
		/// AppSettings key for UI Modules' tray peek.
		/// </summary>
		[Obsolete("Please use uComponents.Core.Shared.Constants.AppKey_TrayPeek")]
		public const string AppKey_TrayPeek = Constants.AppKey_TrayPeek;

		/// <summary>
		/// Dictionary for the UI Modules' appSettings keys.
		/// </summary>
		public static readonly Dictionary<string, string> AppKeys_UiModules = new Dictionary<string, string>()
		{
			{ Constants.AppKey_DragAndDrop, "Drag-n-drop" },
			{ Constants.AppKey_TrayPeek, "Tray Peek" }
		};

		/// <summary>
		/// The base directory for additional uComponents files.
		/// </summary>
		public static readonly string BaseDirName = string.Concat(SystemDirectories.Umbraco, "/plugins/", Constants.ApplicationName);

		/// <summary>
		/// The resource path for the favicon.
		/// </summary>
		public const string FaviconResourcePath = Constants.FaviconResourcePath;

		/// <summary>
		/// The resource path for the Prevalue Editor stylesheet.
		/// </summary>
		public const string PrevalueEditorCssResourcePath = Constants.PrevalueEditorCssResourcePath;

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
					var appSettingValue = ConfigurationManager.AppSettings[Constants.AppKey_RazorModelBinding] ?? bool.TrueString;

					bool.TryParse(appSettingValue, out enableRazorModelBinding);

					razorModelBindingEnabled = enableRazorModelBinding;
				}

				return razorModelBindingEnabled.Value;
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