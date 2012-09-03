using System;
using System.Xml;
using uComponents.Core;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace uComponents.Installer.PackageActions
{
	public class LegacyCleanUp : IPackageAction
	{
		/// <summary>
		/// The alias of the action - for internal use only.
		/// </summary>
		internal static readonly string ActionAlias = string.Concat(Constants.ApplicationName, "_LegacyCleanUp");

		/// <summary>
		/// This Alias must be unique and is used as an identifier that must match the alias in the package action XML
		/// </summary>
		/// <returns>The Alias in string format</returns>
		public string Alias()
		{
			return ActionAlias;
		}

		/// <summary>
		/// Executes the specified package name.
		/// </summary>
		/// <param name="packageName">Name of the package.</param>
		/// <param name="xmlData">The XML data.</param>
		/// <returns>Returns <c>true</c> when successful, otherwise <c>false</c>.</returns>
		public bool Execute(string packageName, XmlNode xmlData)
		{
			// TODO: [LK] Check if upgrade from 3.x/4.x, if so, do a clean-up (HttpModules, any references to old "uComponents.Core" namespace, etc)
			/*
			 * Remove the HttpModules from Web.config
			 * Delete /plugins/uComponents/CustomTreeService.asmx
			 * Delete /plugins/uComponents/ AjaxUploadHandler.ashx
			 * Delete /plugins/uComponents/ UrlPickerService.asmx
			 * Delete /plugins/uComponents/ PreValueWebService.asmx
			 */
			return true;
		}

		/// <summary>
		/// Returns a Sample XML Node
		/// </summary>
		/// <returns>The sample xml as node</returns>
		public XmlNode SampleXml()
		{
			var sample = string.Concat("<Action runat=\"install\" undo=\"false\" alias=\"", ActionAlias, "\" />");
			return helper.parseStringToXmlNode(sample);
		}

		/// <summary>
		/// Undoes the specified package name.
		/// </summary>
		/// <param name="packageName">Name of the package.</param>
		/// <param name="xmlData">The XML data.</param>
		/// <returns>Returns <c>true</c> when successful, otherwise <c>false</c>.</returns>
		public bool Undo(string packageName, XmlNode xmlData)
		{
			return true;
		}
	}
}
