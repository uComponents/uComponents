using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using uComponents.Core;
using umbraco.cms.businesslogic.macro;
using umbraco.cms.businesslogic.packager;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using Umbraco.Core.IO;

namespace uComponents.Installer.PackageActions
{
	/// <summary>
	/// This package action is a stub for uninstalling uComponents modules.
	/// </summary>
	public class Uninstaller : IPackageAction
	{
		/// <summary>
		/// An object to temporarily lock writing to disk.
		/// </summary>
		private static readonly object m_Locker = new object();

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
			var result = this.CleanUpLegacyComponents();
			return result;
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

			// loop through each of the appSettings keys (e.g. UI Modules and Razor Model Binding)
			foreach (var appSettingsKey in appSettingsKeys)
			{
				sb.AppendFormat("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" key=\"{1}\" value=\"false\" />", AddAppConfigKey.ActionAlias, appSettingsKey);
			}

			// get the UI Modules
			sb.AppendFormat("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" />", AddHttpModule.ActionAlias);

			// find the NotFoundHandlers
			var notFoundHandlersNamespace = "uComponents.NotFoundHandlers";
			var notFoundHandlersAssembly = Assembly.Load(notFoundHandlersNamespace);
			if (notFoundHandlersAssembly != null)
			{
				var notFoundHandlersTypes = notFoundHandlersAssembly.GetTypes();
				if (notFoundHandlersTypes != null)
				{
					var notFoundHandlersAction = "<Action runat=\"install\" undo=\"true\" alias=\"{0}\" assembly=\"{1}\" type=\"{2}\" />";
					foreach (var type in notFoundHandlersTypes)
					{
						if (string.Equals(type.Namespace, notFoundHandlersNamespace) && type.FullName.StartsWith(notFoundHandlersNamespace))
						{
							sb.AppendFormat(notFoundHandlersAction, Add404Handler.ActionAlias, notFoundHandlersNamespace, type.FullName.Substring(notFoundHandlersNamespace.Length + 1));
							continue;
						}
					}
				}
			}

			// find the XSLT extensions
			var xsltExtensionsNamespace = "uComponents.XsltExtensions";
			var xsltExtensionsAssembly = Assembly.Load(xsltExtensionsNamespace);
			if (xsltExtensionsAssembly != null)
			{
				var xsltExtensionsTypes = xsltExtensionsAssembly.GetTypes();
				if (xsltExtensionsTypes != null)
				{
					var xsltExtensionsAction = "<Action runat=\"install\" undo=\"true\" alias=\"addXsltExtension\" assembly=\"{0}\" type=\"{1}\" extensionAlias=\"ucomponents.{2}\" />";
					foreach (var type in xsltExtensionsTypes)
					{
						if (string.Equals(type.Namespace, xsltExtensionsNamespace) && type.IsPublic && !type.IsSerializable)
						{
							sb.AppendFormat(xsltExtensionsAction, xsltExtensionsNamespace, type.FullName, type.Name.ToLower());
							continue;
						}
					}
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
				PackageAction.UndoPackageAction(Constants.ApplicationName, node.Attributes["alias"].Value, node);
			}

			return result;
		}

		/// <summary>
		/// Cleans up legacy components.
		/// </summary>
		/// <returns></returns>
		private bool CleanUpLegacyComponents()
		{
			// remove the legacy HttpModules from Web.config
			var result = new AddHttpModule().Undo(string.Empty, null);

			// delete legacy files, as they contain references to legacy namespaces, these will be later recreated.
			var files = new[]
			{
				"DataTypeGrid/PreValueWebService.asmx",
				"MultiNodePicker/CustomTreeService.asmx",
				"Shared/AjaxUpload/AjaxUploadHandler.ashx",
				"UrlPicker/UrlPickerService.asmx"
			};
			foreach (var file in files)
			{
				result = this.DeletePluginFile(file);
			}

			// NotFoundHandlers - update the assembly reference
			result = this.UpdateNotFoundHandlersConfig();

			// XsltExtensions - update the assembly and type references
			result = this.UpdateXsltExtensionsConfig();

			// Macros - update references for RenderTemplate
			result = this.UpdateMacroReferences();

			return result;
		}

		/// <summary>
		/// Deletes the plugin file.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		private bool DeletePluginFile(string file)
		{
			var path = IOHelper.MapPath(Path.Combine(uComponents.DataTypes.Settings.BaseDirName, file));

			lock (m_Locker)
			{
				var info = new FileInfo(path);
				if (info.Exists && !info.IsReadOnly)
				{
					info.Delete();
				}
			}

			return true;
		}

		/// <summary>
		/// Updates the macro references.
		/// </summary>
		/// <returns></returns>
		private bool UpdateMacroReferences()
		{
			var macros = Macro.GetAll();

			foreach (var macro in macros)
			{
				if (string.Equals(macro.Assembly, "uComponents.Core"))
				{
					macro.Assembly = macro.Assembly.Replace(".Core", ".Controls");
					macro.Type = macro.Type.Replace(".Core", string.Empty);
					macro.Save();
				}
			}

			return true;
		}

		/// <summary>
		/// Updates the NotFoundHandlers config.
		/// </summary>
		/// <returns></returns>
		private bool UpdateNotFoundHandlersConfig()
		{
			var path = IOHelper.MapPath(SystemFiles.NotFoundhandlersConfig);
			var config = new XmlDocument() { PreserveWhitespace = true };
			config.Load(path);

			if (config != null)
			{
				var legacyNamespace = "uComponents.Core";
				var nodes = config.SelectNodes(string.Format("//notFound[@assembly = '{0}']", legacyNamespace));
				if (nodes != null && nodes.Count > 0)
				{
					var newNamespace = "uComponents.NotFoundHandlers";
					foreach (XmlNode node in nodes)
					{
						var assembly = node.Attributes.GetNamedItem("assembly");
						if (assembly != null)
						{
							assembly.Value = newNamespace;
						}

						var type = node.Attributes.GetNamedItem("type");
						if (type != null)
						{
							type.Value = type.Value.Replace("NotFoundHandlers.", string.Empty);
						}
					}

					config.Save(path);
				}
			}

			return true;
		}

		/// <summary>
		/// Updates the XSLT extensions config.
		/// </summary>
		/// <returns></returns>
		private bool UpdateXsltExtensionsConfig()
		{
			var path = IOHelper.MapPath(SystemFiles.XsltextensionsConfig);
			var config = new XmlDocument() { PreserveWhitespace = true };
			config.Load(path);

			if (config != null)
			{
				var legacyNamespace = "uComponents.Core";
				var nodes = config.SelectNodes(string.Format("//ext[@assembly = '{0}']", legacyNamespace));
				if (nodes != null && nodes.Count > 0)
				{
					var newNamespace = "uComponents.XsltExtensions";
					foreach (XmlNode node in nodes)
					{
						var assembly = node.Attributes.GetNamedItem("assembly");
						if (assembly != null)
						{
							assembly.Value = newNamespace;
						}

						var type = node.Attributes.GetNamedItem("type");
						if (type != null)
						{
							type.Value = type.Value.Replace("uComponents.Core.XsltExtensions", newNamespace);
						}
					}

					config.Save(path);
				}
			}

			return true;
		}
	}
}