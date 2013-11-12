namespace uComponents.DataTypes.DataTypeGrid.Interfaces
{
    using System.Collections.Generic;

    using uComponents.DataTypes.DataTypeGrid.Model;

    /// <summary>
    /// Interface for the prevalue editor settings factory
    /// </summary>
    public interface IPrevalueEditorSettingsHandler
    {
        /// <summary>
        /// Gets the prevalue editor settings.
        /// </summary>
        /// <param name="dataTypeDefinitionId">The data type definition id.</param>
        /// <returns>The PreValue Editor Settings.</returns>
        PreValueEditorSettings GetPrevalueEditorSettings(int dataTypeDefinitionId);

        /// <summary>
        /// Gets the stored datatype column configurations.
        /// </summary>
        /// <param name="dataTypeDefinitionId">The data type definition id.</param>
        /// <returns>A list of datatype column configurations.</returns>
        IEnumerable<PreValueRow> GetColumnConfigurations(int dataTypeDefinitionId);

        /// <summary>
        /// Gets an available column id.
        /// </summary>
        /// <param name="dataTypeDefinitionId">The data type definition id.</param>
        /// <returns>An id.</returns>
        int GetAvailableId(int dataTypeDefinitionId);
    }
}