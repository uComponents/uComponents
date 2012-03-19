using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uComponents.Core.Shared
{
	/// <summary>
	/// Constants class for uComponent specific values.
	/// </summary>
	public partial class Constants
	{
		/// <summary>
		/// Name of the application.
		/// </summary>
		public const string ApplicationName = "uComponents";

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
		/// The resource path for the favicon.
		/// </summary>
		public const string FaviconResourcePath = "uComponents.Core.Shared.Resources.Images.favicon.ico";

		/// <summary>
		/// The resource path for the Prevalue Editor stylesheet.
		/// </summary>
		public const string PrevalueEditorCssResourcePath = "uComponents.Core.Shared.Resources.Styles.PrevalueEditor.css";
	}
}
