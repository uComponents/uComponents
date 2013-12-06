namespace uComponents.DataTypes.DataTypeGrid.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;

    using uComponents.Core;
    using uComponents.DataTypes.DataTypeGrid.Interfaces;
    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.cms.businesslogic.datatype;

    /// <summary>
    /// Handler for prevalue editor settings
    /// </summary>
    public class PrevalueEditorSettingsHandler : IPrevalueEditorSettingsHandler
    {
        /// <summary>
        /// Gets the prevalue editor settings.
        /// </summary>
        /// <param name="dataTypeDefinitionId">The data type definition id.</param>
        /// <returns>The PreValue Editor Settings.</returns>
        public PreValueEditorSettings GetPrevalueEditorSettings(int dataTypeDefinitionId)
        {
            var prevalues = PreValues.GetPreValues(dataTypeDefinitionId);
            var settings = new PreValueEditorSettings();

            if (prevalues.Count > 0)
            {
                var prevalue = (PreValue)prevalues[0];

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
                        Helper.Log.Error<DataType>("Error when parsing stored prevalues", ex);
                    }
                }
            }

            return settings;
        }

        /// <summary>
        /// Gets the stored datatype column configurations.
        /// </summary>
        /// <param name="dataTypeDefinitionId">The data type definition id.</param>
        /// <returns>A list of datatype column configurations.</returns>
        public IEnumerable<PreValueRow> GetColumnConfigurations(int dataTypeDefinitionId)
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
                        var serializer = new JavaScriptSerializer();

                        // Return the config
                        var s = serializer.Deserialize<PreValueRow>(prevalue.Value);
                        s.Id = prevalue.Id;
                        s.SortOrder = prevalue.SortOrder;

                        sl.Add(s);
                    }
                }
            }

            return sl;
        }

        /// <summary>
        /// Gets an available column id.
        /// </summary>
        /// <param name="dataTypeDefinitionId">The data type definition id.</param>
        /// <returns>An id.</returns>
        public int GetAvailableId(int dataTypeDefinitionId)
        {
            var newId = 1;

            foreach (var config in this.GetColumnConfigurations(dataTypeDefinitionId))
            {
                if (config.Id >= newId)
                {
                    newId = config.Id + 1;
                }
            }

            return newId;
        }
    }
}