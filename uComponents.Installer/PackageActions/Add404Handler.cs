using System.Web;
using System.Xml;
using uComponents.Core;
using umbraco.interfaces;
using Umbraco.Core.IO;

namespace uComponents.Installer.PackageActions
{
	/// <summary>
	/// This package action will add a new entry to the 404handlers.config file
	/// </summary>
	/// <remarks>
	/// This package action has been adapted from the PackageActionsContrib Project.
	/// http://packageactioncontrib.codeplex.com
	/// </remarks>
	public class Add404Handler : IPackageAction
	{
		/// <summary>
		/// The alias of the action - for internal use only.
		/// </summary>
		internal static readonly string ActionAlias = string.Concat(Constants.ApplicationName, "_Add404Handler");

		/// <summary>
		/// This Alias must be unique and is used as an identifier that must match the alias in the package action XML
		/// </summary>
		/// <returns>The Alias in string format</returns>
		public string Alias()
		{
			return ActionAlias;
		}

		/// <summary>
		/// Appends the xmlData Node to the 404handlers.config file
		/// </summary>
		/// <param name="packageName">Name of the package that we install</param>
		/// <param name="xmlData">The data that must be appended to the 404handlers.config file</param>
		/// <returns>True when succeeded</returns>
		public bool Execute(string packageName, XmlNode xmlData)
		{
			// set result default to false
			var result = false;

			// open the 404 handlers config file
			var handlersFile = Umbraco.Core.XmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("/config/404handlers.config"));

			// select notfound node in the config file
			var handlersRootNode = handlersFile.SelectSingleNode("//NotFoundHandlers");

			// get the properties of the handler
			var assembly = xmlData.Attributes["assembly"].Value;
			var type = xmlData.Attributes["type"].Value;

			// check if the handler node does NOT already exist in the config file
			var handlerEntry = handlersRootNode.SelectSingleNode("//notFound[@assembly = '" + assembly + "' and @type = '" + type + "']");
			if (handlerEntry == null)
			{
				// create a new handler node
				var newHandlerNode = (XmlNode)handlersFile.CreateElement("notFound");

				// add the attributes
				newHandlerNode.Attributes.Append(Umbraco.Core.XmlHelper.AddAttribute(handlersFile, "assembly", assembly));
				newHandlerNode.Attributes.Append(Umbraco.Core.XmlHelper.AddAttribute(handlersFile, "type", type));

				// append the new handler node to the 404handlers config file before the 'handle404' entry
				handlersRootNode.InsertBefore(newHandlerNode, handlersRootNode.SelectSingleNode("//notFound[@type = 'handle404']"));

				// save the config file
				handlersFile.Save(IOHelper.MapPath(VirtualPathUtility.ToAbsolute("/config/404handlers.config")));

				// no errors so the result is true
				result = true;
			}

			return result;
		}

		/// <summary>
		/// Removes the xmlData Node from the 404handlers.config file based on the rulename 
		/// </summary>
		/// <param name="packageName">Name of the package that we install</param>
		/// <param name="xmlData">The data</param>
		/// <returns>True when succeeded</returns>
		public bool Undo(string packageName, XmlNode xmlData)
		{
			var result = false;

			// Get the properties of the handler
			var assembly = xmlData.Attributes["assembly"].Value;
			var type = xmlData.Attributes["type"].Value;

			// Open the 404handlers config file
			var rewriteFile = Umbraco.Core.XmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("/config/404handlers.config"));

			// Select the rootnode where we want to delete from
			var handlersRootNode = rewriteFile.SelectSingleNode("//NotFoundHandlers");

			// Select the handler node by attributes from the config file
			var handlerEntry = handlersRootNode.SelectSingleNode("//notFound[@assembly = '" + assembly + "' and @type = '" + type + "']");
			if (handlerEntry != null)
			{
				// Node is found, remove it from the xml document
				handlersRootNode.RemoveChild(handlerEntry);

				// Save the modified configuration file
				rewriteFile.Save(IOHelper.MapPath(VirtualPathUtility.ToAbsolute("/config/404handlers.config")));
			}

			result = true;

			return result;
		}

		/// <summary>
		/// Returns a Sample XML Node
		/// </summary>
		/// <returns>The sample xml as node</returns>
		public XmlNode SampleXml()
		{
			var sample = string.Concat("<Action runat=\"install\" undo=\"true/false\" alias=\"", ActionAlias, "\" assembly=\"assembly\" type=\"type\" />");
			return umbraco.cms.businesslogic.packager.standardPackageActions.helper.parseStringToXmlNode(sample);
		}
	}
}