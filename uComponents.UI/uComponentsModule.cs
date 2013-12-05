using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using umbraco;
using umbraco.cms.businesslogic.datatype;
using umbraco.presentation;
using umbraco.editorControls;

namespace uComponents.UI
{
	/// <summary>
	/// This module adds the scripts required for the shared ui extensions
	/// </summary>
	public class uComponentsModule : IHttpModule
	{
		/// <summary>
		/// Include resources from these IResourceSets
		/// </summary>
		private static List<IResourceSet> sets = new List<IResourceSet> { new UIExtensionResources() };

		private static bool _trayPeek;
		private static bool _dragAndDrop;

		/// <summary>
		/// Gets the sets.
		/// </summary>
		/// <value>The sets.</value>
		public static IList<IResourceSet> Sets { get { return sets; } }

		#region IHttpModule Members

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
		public void Init(HttpApplication context)
		{
			//Item info service
			context.BeginRequest += (sender, e) =>
			{
				ItemInfoService.ProcessRequest((HttpApplication)sender);
			};

			#region Script injection (for drag & drop etc.)

			string umbracoPath = GlobalSettings.Path + "/";
			string enableDragAndDrop = ConfigurationManager.AppSettings[Constants.AppKey_DragAndDrop] ?? "true";
			string enableTrayPeek = ConfigurationManager.AppSettings[Constants.AppKey_TrayPeek] ?? "true";

			bool.TryParse(enableDragAndDrop, out _dragAndDrop);
			bool.TryParse(enableTrayPeek, out _trayPeek);

			context.PostMapRequestHandler += (sender, e) =>
			{
				if (context.Request.CurrentExecutionFilePath.StartsWith(umbracoPath, StringComparison.CurrentCultureIgnoreCase)) // Fix for #12951
				{

					if (context.Request.CurrentExecutionFilePath.Equals(umbracoPath + "default.aspx", StringComparison.InvariantCultureIgnoreCase))
					{
						// /umbraco/default.aspx forwards to umbraco.aspx with Server.Transfer. That's evil, because then the code below doesn't work because it doesn't have the right page, i.e. umbraco.aspx
						// Until a better solution is found this redirect works well.
						context.Response.Redirect("umbraco.aspx");
						return;
					}


					var page = HttpContext.Current.CurrentHandler as Page;
					if (page != null && !UmbracoContext.Current.InPreviewMode)
					{
						page.Load += (s2, e2) =>
							IncludeScripts(page, (reg) => reg.Filters == null || reg.Filters.Any(x => x.IsMatch(context.Request.CurrentExecutionFilePath)));
					}
				}
			};
			#endregion
		}

		#endregion

		/// <summary>
		/// Includes the scripts.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="filter">The filter.</param>
		protected static void IncludeScripts(Page page, Func<ResourceRegistration, bool> filter)
		{
			//BUG Fix: Unfortunately, CD doesn't work with all IIS's in all versions

			//var loader = ClientDependencyLoader.Instance;
			//if (loader == null)
			//{
			//    if (page.Header != null)
			//    {
			//        //If the page doesn't have a ClientDependecyLoader already, and doesn't have a head with runat="server" we don't bother
			//        page.Header.Controls.Add(loader = new ClientDependencyLoader());
			//    }
			//}

			//Item info service
			page.ClientScript.RegisterClientScriptBlock(page.GetType(), "uComponentsItemInfoService",

				"var UC_ITEM_INFO_SERVICE = \"" + ItemInfoService.ServicePath + "\";" +

				//Used when inserting images in content editors
				"var UC_IMAGE_SERVICE = \"" +
					umbraco.IO.IOHelper.ResolveUrl(umbraco.IO.SystemDirectories.Umbraco) +
					"/controls/Images/ImageViewerUpdater.asmx/UpdateImage\";", true);


			var ucSettings = new StringBuilder();
			ucSettings.Append("var UC_SETTINGS = { ENABLE_DRAG_AND_DROP: ").Append(_dragAndDrop ? "true" : "false")
				.Append(", ENABLE_TRAY_PEEK: ").Append(_trayPeek ? "true" : "false").Append("};");
			page.ClientScript.RegisterClientScriptBlock(page.GetType(), "uComponentsSettings",
				ucSettings.ToString(), true);


			//// int userPriority = 100; // Core umbraco stuff must be added first

			var resourcesHtml = new StringBuilder();

			resourcesHtml.Append("<!-- uComponents -->");
			foreach (var set in sets)
			{
				foreach (var reg in set.Resources)
				{
					if (filter(reg))
					{
						page.RegisterEmbeddedClientResource(
							set.GetType(),
							reg.ResourceName,
							reg.IsScript ? ClientDependencyType.Javascript : ClientDependencyType.Css);

						//loader.RegisterDependency(userPriority++, resourceUrl,
						//    reg.IsScript ? ClientDependencyType.Javascript : ClientDependencyType.Css);
					}
				}
			}
		}

		/// <summary>
		/// Use this method to include drag and drop and shortcut scripts on the page specified
		/// </summary>
		public void IncludeScripts(Page page)
		{
			string[] purposes = { "*", "trees", "shortucts" };
			IncludeScripts(page, (reg) => reg.Purposes == null || reg.Purposes.Any(x => purposes.Any(p => p == x)));
		}
	}
}