using System;
using System.Web.UI;
using umbraco;

namespace umbraco.cms.businesslogic.datatype
{
	/// <summary>
	/// Extension methods for embedded resources
	/// </summary>
	public static class ResourceExtensions
	{
		/// <summary>
		/// Adds the resource to client dependency.
		/// </summary>
		/// <param name="ctl">The control.</param>
		/// <param name="resourceName">Name of the resource.</param>
		/// <param name="type">The type.</param>
		public static void AddResourceToClientDependency(this Control ctl, string resourceName, ClientDependency.Core.ClientDependencyType type)
		{
			ctl.AddResourceToClientDependency(resourceName, (ClientDependencyType)type);
		}

		/// <summary>
		/// Adds an embedded resource to the ClientDependency output by name
		/// </summary>
		/// <param name="ctl">The control.</param>
		/// <param name="resourceName">Name of the resource.</param>
		/// <param name="type">The type.</param>
		public static void AddResourceToClientDependency(this Control ctl, string resourceName, ClientDependencyType type)
		{
			ctl.AddResourceToClientDependency(ctl.GetType(), resourceName, type);
		}

		/// <summary>
		/// Adds the resource to client dependency.
		/// </summary>
		/// <param name="ctl">The CTL.</param>
		/// <param name="resourceContainer">The resource container.</param>
		/// <param name="resourceName">Name of the resource.</param>
		/// <param name="type">The type.</param>
		public static void AddResourceToClientDependency(this Control ctl, Type resourceContainer, string resourceName, ClientDependencyType type)
		{
			ctl.Page.AddResourceToClientDependency(resourceContainer, resourceName, type, 100);
		}

		/// <summary>
		/// Adds an embedded resource to the ClientDependency output by name
		/// </summary>
		/// <param name="page">The Page to add the resource to</param>
		/// <param name="resourceContainer">The type containing the embedded resourcre</param>
		/// <param name="resourceName">Name of the resource.</param>
		/// <param name="type">The type.</param>
		/// <param name="priority">The priority.</param>
		public static void AddResourceToClientDependency(this Page page, Type resourceContainer, string resourceName, ClientDependencyType type, int priority)
		{
			// get the urls for the embedded resources           
			var resourceUrl = page.ClientScript.GetWebResourceUrl(resourceContainer, resourceName);
			var target = page.Header;

			// if there's no <head runat="server" /> don't throw an exception.
			if (target != null)
			{
				// Umbraco v4.5.x shipped with earlier version of ClientDependency - which had an issue with querystrings in virtual paths.
				switch (type)
				{
					case ClientDependencyType.Css:
						target.Controls.Add(
							new LiteralControl(string.Concat("<link type='text/css' rel='stylesheet' href='", page.Server.HtmlEncode(resourceUrl), "' />")));
						break;

					case ClientDependencyType.Javascript:
						target.Controls.Add(
							new LiteralControl(string.Concat("<script type='text/javascript' src='", page.Server.HtmlEncode(resourceUrl), "'></script>")));
						break;

					default:
						break;
				}
			}
		}
	}
}