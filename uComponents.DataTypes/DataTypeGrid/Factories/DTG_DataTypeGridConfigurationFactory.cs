using System;
using System.Collections.Generic;
using System.Xml;
using uComponents.DataTypes.DataTypeGrid.Interfaces;

namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using Umbraco.Core.IO;

    /// <summary>
    /// The DTG configuration factory
    /// </summary>
    public class ConfigurationFactory : IConfigurationFactory
    {
        /// <summary>
        /// Gets the compatible data types.
        /// </summary>
        /// <returns>A list of compatible datatypes.</returns>
        public IEnumerable<Guid> GetCompatibleDataTypes()
        {
            var list = new List<Guid>();
            var document = this.OpenXmlFile("~/config/DataTypeGrid.config");
            var compatibleDataTypes = document.GetElementsByTagName("CompatibleDataTypes")[0] as XmlElement;

            if (compatibleDataTypes != null && compatibleDataTypes.ChildNodes.Count > 0)
            {
                foreach (XmlElement item in compatibleDataTypes.ChildNodes)
                {
                    var guid = item.Attributes["guid"].Value;

                    if (!string.IsNullOrEmpty(guid))
                    {
                        list.Add(new Guid(guid));
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Opens the xml document.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The xml document.</returns>
        public XmlDocument OpenXmlFile(string filePath)
        {
            var path = IOHelper.MapPath(filePath);
            var doc = new XmlDocument();

            var readerSettings = new XmlReaderSettings
            {
                IgnoreComments = true
            };

            using (var reader = XmlReader.Create(path, readerSettings))
            {
                doc.Load(reader);
            }

            return doc;
        }
    }
}
