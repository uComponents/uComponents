
namespace uComponents.Core
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
		/// AppSettings key to disable Razor model binding for data-types.
		/// </summary>
		public const string AppKey_RazorModelBinding = "ucomponents:RazorModelBinding";

		/// <summary>
		/// AppSettings key for UI Modules' tray peek.
		/// </summary>
		public const string AppKey_TrayPeek = "ucomponents:TrayPeek";

		/// <summary>
		/// The resource path for the favicon.
		/// </summary>
		public const string FaviconResourcePath = "uComponents.Core.Resources.Images.favicon.ico";

		/// <summary>
		/// The resource path for the favicon.
		/// </summary>
		public const string IconResourcePath = "uComponents.Core.Resources.Images.icon.png";

		/// <summary>
		/// The resource path for the Prevalue Editor stylesheet.
		/// </summary>
		public const string PrevalueEditorCssResourcePath = "uComponents.DataTypes.Shared.Resources.Styles.PrevalueEditor.css";

		/// <summary>
		/// Message for legacy usage of the uQuery extension methods.
		/// </summary>
		public const string uQueryLegacyObsoleteMessage = "Please reference the uQuery library from the 'umbraco' assembly.";
	}
}
