using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;
using uComponents.Core;
using umbraco;

[assembly: WebResource("uComponents.UI.Resources.Scripts.jquery.ui.position.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.UI.Resources.Scripts.jquery.simulate.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.UI.Resources.Scripts.jquery.shortcuts.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.UI.Resources.Scripts.jquery.tree.keyboard.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.UI.Resources.Scripts.shortcuts.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.UI.Resources.Scripts.tree-extensions.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.UI.Resources.Scripts.content-drag-drop.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.UI.Resources.Scripts.item-info-service.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.UI.Resources.Scripts.ie-z-index-fix.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.UI.Resources.Styles.tree-extras.css", Constants.MediaTypeNames.Text.Css)]

namespace uComponents.UI
{
	/// <summary>    
	/// This control contains the scripts required for the shared ui extensions. They are automatically added to all (relevant) pages by the uComponentsModule
	/// </summary>
	public sealed class UIExtensionResources : IResourceSet
	{
		/// <summary>
		/// We only expect trees in the top frame (umbraco.aspx) and edit pages
		/// </summary>
		static Regex editPages = new Regex(@"edit.*\.aspx", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// 
		/// </summary>
		public static Regex topFrame = new Regex(string.Concat("^", GlobalSettings.Path, @"/(default|umbraco)\.aspx"), RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// 
		/// </summary>
		static Regex[] treeTargets = new[] { topFrame, editPages };

		/// <summary>
		/// 
		/// </summary>
		static Regex[] shortcutTargets = new[] { topFrame, editPages };

		/// <summary>
		/// To allow us to only target the edit pages.
		/// </summary>
		static Regex[] editTargets = new[] { editPages };

		/// <summary>
		/// 
		/// </summary>
		public static readonly IEnumerable<ResourceRegistration> Resources = new[]
		{
			new ResourceRegistration
			{
				ResourceName = "uComponents.UI.Resources.Scripts.jquery.simulate.js",
				IsScript = true,
				Filters = treeTargets,
				Purposes = new string[] { "*" }
			},
			new ResourceRegistration
			{
				ResourceName = "uComponents.UI.Resources.Scripts.jquery.ui.position.js",
				IsScript = true,
				Filters = treeTargets,
				Purposes = new string[] { "*" }
			},
			new ResourceRegistration
			{
				ResourceName = "uComponents.UI.Resources.Scripts.jquery.shortcuts.js",
				IsScript = true,
				Filters = shortcutTargets,
				Purposes = new string[] { "*" }
			},
			new ResourceRegistration
			{
				ResourceName = "uComponents.UI.Resources.Scripts.jquery.tree.keyboard.js",
				IsScript = true,
				Filters = treeTargets,
				Purposes = new string[] { "*" }
			},
			new ResourceRegistration
			{
				ResourceName = "uComponents.UI.Resources.Scripts.shortcuts.js",
				IsScript = true,
				Filters = shortcutTargets,
				Purposes = new string[] { "*" }
			},
			new ResourceRegistration
			{
				ResourceName = "uComponents.UI.Resources.Scripts.tree-extensions.js",
				IsScript = true,
				Filters = treeTargets,
				Purposes = new string[] { "*" }
			},
			new ResourceRegistration
			{
				ResourceName = "uComponents.UI.Resources.Scripts.content-drag-drop.js",
				IsScript = true,
				Filters = treeTargets,
				Purposes = new string[] { "*" }
			},
			new ResourceRegistration
			{
				ResourceName = "uComponents.UI.Resources.Scripts.item-info-service.js",
				IsScript = true,
				Filters = treeTargets,
				Purposes = new string[] { "*" }
			},
			new ResourceRegistration
			{
				ResourceName = "uComponents.UI.Resources.Scripts.ie-z-index-fix.js",
				IsScript = true,
				Filters = editTargets,
				Purposes = new string[] { "*" }
			},
			new ResourceRegistration
			{
				ResourceName = "uComponents.UI.Resources.Styles.tree-extras.css",
				IsScript = false,
				Filters = treeTargets,
				Purposes = new string[] { "*" }
			}
		};

		/// <summary>
		/// Gets the resources to embed.
		/// </summary>
		/// <value></value>
		IEnumerable<ResourceRegistration> IResourceSet.Resources
		{
			get { return Resources; }
		}
	}
}