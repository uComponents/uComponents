using System;
using System.Collections.Generic;
using System.Xml;

namespace uComponents.DataTypes.DataTypeGrid.Interfaces
{
    /// <summary>
    /// Interface for the DTG configuration factory
    /// </summary>
    interface IConfigurationHandler
    {
        /// <summary>
        /// Gets the compatible data types.
        /// </summary>
        /// <returns>A list of compatible datatypes.</returns>
        IEnumerable<Guid> GetCompatibleDataTypes();

        /// <summary>
        /// Opens the specified XML document.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The xml document.</returns>
        XmlDocument OpenXmlFile(string filePath);
    }
}
