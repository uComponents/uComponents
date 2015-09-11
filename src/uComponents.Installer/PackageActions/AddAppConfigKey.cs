using System.Configuration;
using System.Web.Configuration;
using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using uComponents.Core;

namespace uComponents.Installer.PackageActions
{
	/// <summary>
	/// Adds a key to the web.config app settings.
	/// </summary>
	/// <remarks>
	/// This package action has been adapted from the PackageActionsContrib Project.
	/// http://packageactioncontrib.codeplex.com
	/// </remarks>
	public class AddAppConfigKey : IPackageAction
	{
		/// <summary>
		/// The alias of the action - for internal use only.
		/// </summary>
		internal static readonly string ActionAlias = string.Concat(Constants.ApplicationName, "_AddAppConfigKey");

		/// <summary>
		/// This Alias must be unique and is used as an identifier that must match the alias in the package action XML.
		/// </summary>
		/// <returns>The Alias of the package action.</returns>
		public string Alias()
		{
			return ActionAlias;
		}

		/// <summary>
		/// Adds an appSettings key to the web.config file.
		/// </summary>
		/// <param name="packageName">Name of the package that we install.</param>
		/// <param name="xmlData">The XML data for the appSettings key.</param>
		/// <returns>True when succeeded.</returns>
		public bool Execute(string packageName, XmlNode xmlData)
		{
			try
			{
				var addKey = xmlData.Attributes["key"].Value;
				var addValue = xmlData.Attributes["value"].Value;

				// as long as addKey has a value, create the key entry in web.config
				if (addKey != string.Empty)
				{
					this.CreateAppSettingsKey(addKey, addValue);
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Provides a sample of the XML.
		/// </summary>
		/// <returns></returns>
		public XmlNode SampleXml()
		{
			var sample = string.Concat("<Action runat=\"install\" undo=\"true/false\" alias=\"", ActionAlias, "\" key=\"your key\" value=\"your value\" />");
			return helper.parseStringToXmlNode(sample);
		}

		/// <summary>
		/// Removes an appSettings key to the web.config file.
		/// </summary>
		/// <param name="packageName">Name of the package that we install.</param>
		/// <param name="xmlData">The XML data for the appSettings key.</param>
		/// <returns>True when succeeded.</returns>
		public bool Undo(string packageName, XmlNode xmlData)
		{
			try
			{
				var addKey = xmlData.Attributes["key"].Value;

				// as long as addKey has a value, remove it from the key entry in web.config
				if (addKey != string.Empty)
				{
					this.RemoveAppSettingsKey(addKey);
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Creates the appSettings key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		private void CreateAppSettingsKey(string key, string value)
		{
			var config = WebConfigurationManager.OpenWebConfiguration("~");
			var appSettings = (AppSettingsSection)config.GetSection("appSettings");

			if (appSettings.Settings[key] != null)
			{
				appSettings.Settings.Remove(key);
			}

			appSettings.Settings.Add(key, value);

			config.Save(ConfigurationSaveMode.Modified);
		}

		/// <summary>
		/// Removes the appSettings key.
		/// </summary>
		/// <param name="key">The key.</param>
		private void RemoveAppSettingsKey(string key)
		{
			var config = WebConfigurationManager.OpenWebConfiguration("~");
			var appSettings = (AppSettingsSection)config.GetSection("appSettings");

			appSettings.Settings.Remove(key);

			config.Save(ConfigurationSaveMode.Modified);
		}
	}
}