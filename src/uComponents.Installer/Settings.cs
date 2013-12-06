using System.Collections.Generic;
using uComponents.Core;

namespace uComponents.Installer
{
	/// <summary>
	/// Global settings for uComponents installer.
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// Dictionary for the UI Modules' appSettings keys.
		/// </summary>
		public static readonly Dictionary<string, string> AppKeys_UiModules = new Dictionary<string, string>()
		{
			{ Constants.AppKey_DragAndDrop, "Drag-n-drop" },
			{ Constants.AppKey_TrayPeek, "Tray Peek" }
		};
	}
}