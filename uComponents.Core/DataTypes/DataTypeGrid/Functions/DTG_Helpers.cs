// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Changed to internal
// 11.08.2010 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.Core.DataTypes.DataTypeGrid.Functions
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Text.RegularExpressions;
	using System.Web;
	using System.Web.Script.Serialization;
	using System.Web.UI.WebControls;
	using System.Xml;

	using uComponents.Core.DataTypes.DataTypeGrid.Model;

	using umbraco.cms.businesslogic.datatype;
	using umbraco.BusinessLogic;

	/// <summary>
	/// The dtg helpers.
	/// </summary>
	internal class DtgHelpers
	{
		/// <summary>
		/// Gets the compatible data types.
		/// </summary>
		/// <returns></returns>
		public static IList<Guid> GetCompatibleDataTypes()
		{
			var list = new List<Guid>();

			foreach (XmlElement item in GetDtgSettingsSection(OpenDtgSettings(), "CompatibleDataTypes"))
			{
				var guid = item.Attributes["guid"].Value;

				if (!string.IsNullOrEmpty(guid)) 
				{ 
					list.Add(new Guid(guid));
				}
			}

			return list;
		}

		/// <summary>
		/// Reads the DTG settings.
		/// </summary>
		/// <returns></returns>
		public static XmlDocument OpenDtgSettings()
		{
			var filePath = HttpContext.Current.Server.MapPath("~/config/DataTypeGrid.config");
			var doc = new XmlDocument();

			var readerSettings = new XmlReaderSettings
												   {
													   IgnoreComments = true
												   };

			using (var reader = XmlReader.Create(filePath, readerSettings))
			{
				doc.Load(reader);
			}

			return doc;
		}

		/// <summary>
		/// Gets the DTG settings section.
		/// </summary>
		/// <param name="doc">The document.</param>
		/// <param name="sectionName">Name of the section.</param>
		/// <returns></returns>
		public static XmlNodeList GetDtgSettingsSection(XmlDocument doc, string sectionName)
		{
			var section = doc.GetElementsByTagName(sectionName)[0] as XmlElement;

			if (section != null && section.ChildNodes.Count > 0)
			{
				return section.ChildNodes;
			}

			return null;
		}

		/// <summary>
		/// Ensures the folder exists.
		/// </summary>
		/// <param name="foldername">The foldername.</param>
		/// <param name="locker">The locker.</param>
		/// <returns></returns>
		public static DirectoryInfo EnsureFolderExists(string foldername, object locker)
		{
			var path = Path.Combine(uComponents.Core.Shared.Settings.BaseDir.FullName, foldername);

			if (!Directory.Exists(path))
			{
				lock (locker)
				{
					if (!Directory.Exists(path))
					{
						var dir = new DirectoryInfo(path);
						dir.Create();
					}
				}
			}

			return new DirectoryInfo(path);
		}

		/// <summary>
		/// Ensures the file exists.
		/// </summary>
		/// <param name="filepath">The filepath.</param>
		/// <param name="resource">The resource.</param>
		/// <param name="locker">The locker.</param>
		/// <returns></returns>
		public static FileInfo EnsureFileExists(string filepath, string resource, object locker)
		{
			if (!File.Exists(filepath))
			{
				lock (locker)
				{
					if (!File.Exists(filepath))
					{
						using (var writer = new StreamWriter(File.Create(filepath)))
						{
							writer.Write(resource);
						}
					}
				}
			}

			return new FileInfo(filepath);
		}

		/// <summary>
		/// Gets the data type dropdown.
		/// </summary>
		/// <returns></returns>
		public static DropDownList GetDataTypeDropDown()
		{
			var d = new DropDownList() { CssClass = "propertyItemContent" };

			foreach (var item in uQuery.GetAllDataTypes())
			{
				var guid = uQuery.GetBaseDataTypeGuid(item.Key);
				if (DtgHelpers.GetCompatibleDataTypes().Contains(guid)) 
				{ 
					d.Items.Add(new ListItem(item.Value, item.Key.ToString()));
				}
			}

			return d;
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		/// <param name="dataTypeDefinitionId">
		/// The data type definition id.
		/// </param>
		/// <returns>
		/// </returns>
		public static PreValueEditorSettings GetSettings(int dataTypeDefinitionId)
		{
			var prevalues = PreValues.GetPreValues(dataTypeDefinitionId);
			var settings = new PreValueEditorSettings();

			if (prevalues.Count > 0)
			{
				var prevalue = (PreValue) prevalues[0];

				if (!string.IsNullOrEmpty(prevalue.Value)) 
				{ 
					var serializer = new JavaScriptSerializer();
					
					try
					{
						settings = serializer.Deserialize<PreValueEditorSettings>(prevalue.Value);
					}
					catch (Exception ex)
					{
						// Cannot understand stored prevalues
						Log.Add(LogTypes.Error, User.GetUser(0), dataTypeDefinitionId, "uComponents [DataTypeGrid]: Error when parsing stored prevalues: " + ex.Message);
					}
				}
			}

			return settings;
		}

		/// <summary>
		/// Gets the DTG config.
		/// </summary>
		/// <param name="dataTypeDefinitionId">
		/// The data type definition id.
		/// </param>
		/// <returns>
		/// </returns>
		public static List<PreValueRow> GetConfig(int dataTypeDefinitionId)
		{
			var prevalues = PreValues.GetPreValues(dataTypeDefinitionId);
			var sl = new List<PreValueRow>();

			if (prevalues.Count > 1)
			{
				for (var i = 1; i < prevalues.Count; i++)
				{
				    var prevalue = (PreValue)prevalues[i];

				    if (!string.IsNullOrEmpty(prevalue.Value))
				    {
						// Get the config
						var s = GetSingleConfig(prevalue.Value);
						s.Id = prevalue.Id;
						s.SortOrder = prevalue.SortOrder;

						sl.Add(s);
					}
				}
			}

			return sl;
		}

		/// <summary>
		/// Gets the single DTG config.
		/// </summary>
		/// <param name="value">
		/// The value.
		/// </param>
		/// <returns>
		/// </returns>
		public static PreValueRow GetSingleConfig(string value)
		{
			var serializer = new JavaScriptSerializer();

			// Return the config
			return serializer.Deserialize<PreValueRow>(value);
		}

		/// <summary>
		/// Gets an available id.
		/// </summary>
		/// <param name="dataTypeDefinitionId">
		/// The data Type Definition Id.
		/// </param>
		/// <returns>
		/// A new available id
		/// </returns>
		public static int GetAvailableId(int dataTypeDefinitionId)
		{
			int newId = 1;

			foreach (var config in GetConfig(dataTypeDefinitionId))
			{
				if (config.Id >= newId)
				{
					newId = config.Id + 1;
				}
			}

			return newId;
		}

		/// <summary>
		/// Validates the regex.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static bool ValidateRegex(string value)
		{
			var isValid = true;
			if ((value != null) && (value.Trim().Length > 0))
			{
				try
				{
					Regex.Match("", value);
				}
				catch (ArgumentException)
				{
					// BAD PATTERN: Syntax error
					isValid = false;
				}
			}
			else
			{
				//BAD PATTERN: Pattern is null or blank
				isValid = false;
			}

			return isValid;
		}

		/// <summary>
		/// Adds the log entry.
		/// </summary>
		/// <param name="message">The message.</param>
		internal static void AddLogEntry(string message)
		{
			int pageId;
			int.TryParse(HttpContext.Current.Request.QueryString["id"], out pageId);

			Log.Add(LogTypes.Custom, User.GetCurrent(), pageId, message);
		}
	}
}