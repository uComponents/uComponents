// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Changed to internal
// 11.08.2010 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Functions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.UI.WebControls;
    using System.Xml;

    using Umbraco.Core.IO;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco;
    using umbraco.BusinessLogic;
    using umbraco.cms.businesslogic.datatype;

    using Umbraco.Core.Logging;

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
            var filePath = IOHelper.MapPath("~/config/DataTypeGrid.config");
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
                        LogError("Error when parsing stored prevalues", ex);
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
            SortedList prevalues = PreValues.GetPreValues(dataTypeDefinitionId);
            var sl = new List<PreValueRow>();

            if (prevalues.Count > 1)
            {
                for (int i = 1; i < prevalues.Count; i++)
                {
                    var prevalue = (PreValue) prevalues[i];
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
        /// Adds the error log entry.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception.</param>
        internal static void LogError(string message, Exception ex)
        {
            int pageId;
            int.TryParse(HttpContext.Current.Request.QueryString["id"], out pageId);

            LogHelper.Error<DataType>(string.Format("[User {0}] [Page {1}] {2}", User.GetCurrent().Id, pageId, message), ex);
        }

        /// <summary>
        /// Adds the warning log entry.
        /// </summary>
        /// <param name="message">The message.</param>
        internal static void LogWarn(string message)
        {
            int pageId;
            int.TryParse(HttpContext.Current.Request.QueryString["id"], out pageId);

            LogHelper.Warn<DataType>(string.Format("[User {0}] [Page {1}] {2}", User.GetCurrent().Id, pageId, message));
        }

        /// <summary>
        /// Adds the debug entry.
        /// </summary>
        /// <param name="message">The message.</param>
        internal static void LogDebug(string message)
        {
            int pageId;
            int.TryParse(HttpContext.Current.Request.QueryString["id"], out pageId);

            LogHelper.Debug<DataType>(string.Format("[User {0}] [Page {1}] {2}", User.GetCurrent().Id, pageId, message));
        }
    }
}