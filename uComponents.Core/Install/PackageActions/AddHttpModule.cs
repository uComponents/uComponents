using System;
using System.Web;
using System.Xml;
using umbraco;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.interfaces;

namespace uComponents.Core.Install.PackageActions
{
	/// <summary>
	/// This package action will Add a new HTTP Module to the web.config file.
	/// </summary>
	/// <remarks>
	/// This package action has been adapted from the PackageActionsContrib Project.
	/// http://packageactioncontrib.codeplex.com
	/// </remarks>
	public class AddHttpModule : IPackageAction
	{
		/// <summary>
		/// The alias of the action - for internal use only.
		/// </summary>
		internal static readonly string ActionAlias = string.Concat(Constants.ApplicationName, "_AddHttpModule");

		/// <summary>
		/// The name of the HttpModule.
		/// </summary>
		public const string Name = "uComponentsModule";

		/// <summary>
		/// The type of the HttpModule.
		/// </summary>
		public const string Type = "uComponents.Core.Modules.uComponentsModule, uComponents.Core";

		// Set the web.config full path
		const string FULL_PATH = "~/Web.config";

		// Add the module to the different locations for IIS6 and 7
		private static string[] targets = new[] { "/configuration/system.web/httpModules", "/configuration/system.webServer/modules" };

		/// <summary>
		/// Tests if the uComponents HTTP module is added to web.config
		/// </summary>
		/// <returns></returns>
		public static bool Test()
		{
			try
			{
				var document = new XmlDocument();
				document.PreserveWhitespace = true;
				document.Load(HttpContext.Current.Server.MapPath(FULL_PATH));
				var foundOne = false;

				// See if any of the target contains the module
				foreach (var target in targets)
				{
					var rootNode = document.SelectSingleNode(target);

					if (rootNode == null)
					{
						continue;
					}

					var node = rootNode.SelectSingleNode(string.Format("add[@name = '{0}']", Name));

					if (node != null)
					{
						// This one did. Return true if all did.
						foundOne = true;
					}
					else
					{
						// This one didn't. Return false
						return false;
					}
				}

				return foundOne;
			}
			catch (Exception e)
			{
				Log.Add(LogTypes.Error, getUser(), -1, string.Concat("Error at testing AddHttpModule package action: ", e.Message));
				return false;
			}
		}

		/// <summary>
		/// Installs the uComponents HTTP module.
		/// </summary>
		/// <returns></returns>
		public static bool Install()
		{
			try
			{
				// Set result default to false
				bool result = false;

				// Create a new xml document
				var document = new XmlDocument();

				// Keep current indentions format
				document.PreserveWhitespace = true;

				// Load the web.config file into the xml document
				document.Load(HttpContext.Current.Server.MapPath(FULL_PATH));

				// Set modified document default to false
				bool modified = false;

				// Loop through each of the targets
				foreach (var target in targets)
				{
					// Select root node in the web.config file for insert new nodes
					var rootNode = document.SelectSingleNode(target);

					// Check for rootNode exists
					if (rootNode == null)
					{
						continue;
					}

					// Set insert node default true
					bool insertNode = true;

					// Look for existing nodes with same name like the new node
					if (rootNode.HasChildNodes)
					{
						// Look for existing nodeType nodes
						var node = rootNode.SelectSingleNode(string.Format("add[@name = '{0}']", Name));

						// If name already exists 
						if (node != null)
						{
							// Cancel insert node operation
							insertNode = false;
						}
					}

					// Check for insert flag
					if (insertNode)
					{
						// Create new node with attributes
						var newNode = document.CreateElement("add");
						newNode.Attributes.Append(xmlHelper.addAttribute(document, "name", Name));
						newNode.Attributes.Append(xmlHelper.addAttribute(document, "type", Type));

						// Append new node at the end of root node
						rootNode.AppendChild(newNode);

						// Mark document modified
						modified = true;
					}
				}

				// Check for modified document
				if (modified)
				{
					// Save the Rewrite config file with the new rewerite rule
					document.Save(HttpContext.Current.Server.MapPath(FULL_PATH));

					// No errors so the result is true
					result = true;
				}

				return result;
			}
			catch (Exception e)
			{
				Log.Add(LogTypes.Error, getUser(), -1, string.Concat("Error at execute ", ActionAlias, " package action: ", e.Message));
				return false;
			}
		}

		/// <summary>
		/// This Alias must be unique and is used as an identifier that must match the alias in the package action XML.
		/// </summary>
		/// <returns>The Alias of the package action.</returns>
		public string Alias()
		{
			return ActionAlias;
		}

		/// <summary>
		/// Append the xmlData node to the web.config file
		/// </summary>
		/// <param name="packageName">Name of the package that we install</param>
		/// <param name="xmlData">The data that must be appended to the web.config file</param>
		/// <returns>True when succeeded</returns>
		public bool Execute(string packageName, XmlNode xmlData)
		{
			return Install();
		}

		/// <summary>
		/// Removes the xmlData node from the web.config file
		/// </summary>
		/// <param name="packageName">Name of the package that we install</param>
		/// <param name="xmlData">The data that must be appended to the web.config file</param>
		/// <returns>True when succeeded</returns>
		public bool Undo(string packageName, XmlNode xmlData)
		{
			// Set result default to false
			var result = false;

			// Create a new xml document
			var document = new XmlDocument();

			// Keep current indentions format
			document.PreserveWhitespace = true;

			// Load the web.config file into the xml document
			document.Load(HttpContext.Current.Server.MapPath(FULL_PATH));

			// Set modified document default to false
			var modified = false;

			foreach (var target in targets)
			{
				// Select root node in the web.config file for insert new nodes
				var rootNode = document.SelectSingleNode(target);

				// Check for rootNode exists
				if (rootNode == null)
				{
					continue;
				}

				// Look for existing nodes with same name of undo attribute
				if (rootNode.HasChildNodes)
				{
					// Look for existing add nodes with attribute name
					foreach (XmlNode existingNode in rootNode.SelectNodes(string.Format("add[@name = '{0}']", Name)))
					{
						// Remove existing node from root node
						rootNode.RemoveChild(existingNode);
						modified = true;
					}
				}
			}

			if (modified)
			{
				try
				{
					// Save the Rewrite config file with the new rewerite rule
					document.Save(HttpContext.Current.Server.MapPath(FULL_PATH));

					// No errors so the result is true
					result = true;
				}
				catch (Exception ex)
				{
					Log.Add(LogTypes.Error, getUser(), -1, string.Concat("Error at undo ", ActionAlias, " package action: ", ex.Message));
				}
			}

			return result;
		}

		/// <summary>
		/// Get the current user, or when unavailable admin user
		/// </summary>
		/// <returns>The current user</returns>
		private static User getUser()
		{
			var id = BasePage.GetUserId(BasePage.umbracoUserContextID);

			if (id < 0)
			{
				id = 0;
			}

			return User.GetUser(id);
		}

		/// <summary>
		/// Get a named attribute from xmlData root node
		/// </summary>
		/// <param name="xmlData">The data that must be appended to the web.config file</param>
		/// <param name="attribute">The name of the attribute</param>
		/// <param name="value">returns the attribute value from xmlData</param>
		/// <returns>True, when attribute value available</returns>
		private bool getAttribute(XmlNode xmlData, string attribute, out string value)
		{
			// Set result default to false
			var result = false;

			// Out params must be assigned
			value = string.Empty;

			// Search xml attribute
			var xmlAttribute = xmlData.Attributes[attribute];

			// When xml attribute exists
			if (xmlAttribute != null)
			{
				// Get xml attribute value
				value = xmlAttribute.Value;

				// Set result successful to true
				result = true;
			}
			else
			{
				Log.Add(LogTypes.Error, getUser(), -1, string.Concat("Error at ", ActionAlias, " package action: Attribute \"", attribute, "\" not found."));
			}

			return result;
		}

		/// <summary>
		/// Get an optional named attribute from xmlData root node
		/// when attribute is unavailable, return the default value
		/// </summary>
		/// <param name="xmlData">The data that must be appended to the web.config file</param>
		/// <param name="attribute">The name of the attribute</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns>The attribute value or the default value</returns>
		private string getAttributeDefault(XmlNode xmlData, string attribute, string defaultValue)
		{
			// Set result default value
			var result = defaultValue;

			// Search xml attribute
			var xmlAttribute = xmlData.Attributes[attribute];

			// When xml attribute exists
			if (xmlAttribute != null)
			{
				// Get available xml attribute value
				result = xmlAttribute.Value;
			}

			return result;
		}

		/// <summary>
		/// Returns a Sample XML Node 
		/// In this case the Sample HTTP Module TimingModule 
		/// </summary>
		/// <returns>The sample xml as node</returns>
		public XmlNode SampleXml()
		{
			var sample = string.Concat("<Action runat=\"install\" undo=\"true\" alias=\"", ActionAlias, "\" />");
			return umbraco.cms.businesslogic.packager.standardPackageActions.helper.parseStringToXmlNode(sample);
		}
	}
}