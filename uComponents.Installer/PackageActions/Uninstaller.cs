using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using umbraco.cms.businesslogic.packager;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using uComponents.Core;

namespace uComponents.Installer.PackageActions
{
	/// <summary>
	/// This package action is a stub for uninstalling uComponents modules.
	/// </summary>
	public class Uninstaller : IPackageAction
	{
		/// <summary>
		/// The alias of the action - for internal use only.
		/// </summary>
		internal static readonly string ActionAlias = string.Concat(Constants.ApplicationName, "_Uninstaller");

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
			return true;
		}

		/// <summary>
		/// Returns a Sample XML Node
		/// </summary>
		/// <returns>The sample xml as node</returns>
		public XmlNode SampleXml()
		{
			var sample = string.Concat("<Action runat=\"install\" undo=\"true\" alias=\"", ActionAlias, "\" />");
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
			var result = true;

			// build XML string for all the installable components
			var sb = new StringBuilder("<Actions>");
			
			// get the UI Modules
			sb.AppendFormat("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" />", AddHttpModule.ActionAlias);

			// loop through each of the appSettings keys for the UI Modules
			foreach (var appKey in Settings.AppKeys_UiModules)
			{
				sb.AppendFormat("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" key=\"{1}\" value=\"false\" />", AddAppConfigKey.ActionAlias, appKey.Key);
			}

			// TODO: [LK] Refactor the uninstaller to load the correct assemblies (e.g. NotFoundHandlers and XsltExtensions)

			// loop through the assembly's types
			var types = Assembly.GetExecutingAssembly().GetTypes().ToList();
			types.Sort(delegate(Type t1, Type t2) { return t1.Name.CompareTo(t2.Name); });

			foreach (var type in types)
			{
				string ns = type.Namespace;
				if (string.IsNullOrEmpty(ns))
				{
					continue;
				}

				// get the NotFoundHandlers
				if (ns == "uComponents.NotFoundHandlers")
				{
					sb.AppendFormat("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" assembly=\"uComponents.NotFoundHandlers\" type=\"{1}\" />", Add404Handler.ActionAlias, type.FullName.Replace("uComponents.NotFoundHandlers.", string.Empty));
					continue;
				}

				// get the XSLT Extensions
				if (ns == "uComponents.XsltExtensions" && type.IsPublic && !type.IsSerializable)
				{
					sb.AppendFormat("<Action runat=\"install\" undo=\"true\" alias=\"addXsltExtension\" assembly=\"uComponents.XsltExtensions\" type=\"{0}\" extensionAlias=\"ucomponents.{1}\" />", type.FullName, type.Name.ToLower());
					continue;
				}
			}

			// append the closing tag
			sb.Append("</Actions>");

			// load the XML string into an XML document
			var actionsXml = new XmlDocument();
			actionsXml.LoadXml(sb.ToString());

			// loop through each of the installable components
			foreach (XmlNode node in actionsXml.DocumentElement.SelectNodes("//Action"))
			{
				// uninstall the components
				PackageAction.UndoPackageAction(Constants.ApplicationName, node.Attributes["alias"].Value, node);
			}

			return result;
		}
	}
}