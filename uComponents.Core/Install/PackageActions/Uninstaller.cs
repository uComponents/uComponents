using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using uComponents.Core.Shared;
using umbraco.cms.businesslogic.packager;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using uc = uComponents.Core.Shared;

namespace uComponents.Core.Install.PackageActions
{
	/// <summary>
	/// This package action is a stub for uninstalling uComponents modules.
	/// </summary>
	public class Uninstaller : IPackageAction
	{
		/// <summary>
		/// The alias of the action - for internal use only.
		/// </summary>
		internal static readonly string ActionAlias = string.Concat(uc.Constants.ApplicationName, "_Uninstaller");

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

			// list of AppSettings keys to uninstall
			var appSettingsKeys = new List<string>()
			{
				Constants.AppKey_DragAndDrop,
				Constants.AppKey_KeyboardShortcuts,
				Constants.AppKey_RazorModelBinding,
				Constants.AppKey_TrayPeek
			};

			// loop through each of the AppSettings keys (for the UI Modules and Razor Model Binding)
			foreach (var appSettingsKey in appSettingsKeys)
			{
				sb.AppendFormat("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" key=\"{1}\" value=\"false\" />", AddAppConfigKey.ActionAlias, appSettingsKey);
			}

			// get the UI Modules
			sb.AppendFormat("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" />", AddHttpModule.ActionAlias);

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
				if (ns == "uComponents.Core.NotFoundHandlers")
				{
					sb.AppendFormat("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" assembly=\"uComponents.Core\" type=\"{1}\" />", Add404Handler.ActionAlias, type.FullName.Replace("uComponents.Core.", string.Empty));
					continue;
				}

				// get the XSLT Extensions
				if (ns == "uComponents.Core.XsltExtensions" && type.IsPublic && !type.IsSerializable)
				{
					sb.AppendFormat("<Action runat=\"install\" undo=\"true\" alias=\"addXsltExtension\" assembly=\"uComponents.Core\" type=\"{0}\" extensionAlias=\"ucomponents.{1}\" />", type.FullName, type.Name.ToLower());
					continue;
				}
			}

			// remove the dashboard control (if exists)
			sb.Append("<Action runat=\"install\" undo=\"true\" alias=\"addDashboardSection\" dashboardAlias=\"uComponentsInstaller\" />");

			// append the closing tag
			sb.Append("</Actions>");

			// load the XML string into an XML document
			var actionsXml = new XmlDocument();
			actionsXml.LoadXml(sb.ToString());

			// loop through each of the installable components
			foreach (XmlNode node in actionsXml.DocumentElement.SelectNodes("//Action"))
			{
				// uninstall the components
				PackageAction.UndoPackageAction(uc.Constants.ApplicationName, node.Attributes["alias"].Value, node);
			}

			return result;
		}
	}
}