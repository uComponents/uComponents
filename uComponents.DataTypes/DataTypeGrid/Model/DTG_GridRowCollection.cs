// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 30.01.2013 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.Xml;

    using umbraco.MacroEngines;

    /// <summary>
    /// Represents rows in a DataTypeGrid
    /// </summary>
    public class GridRowCollection : Collection<GridRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridRowCollection" /> class.
        /// </summary>
        public GridRowCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridRowCollection" /> class.
        /// </summary>
        /// <param name="xml">The XML.</param>
        public GridRowCollection(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var items = doc.DocumentElement;

            if (items != null && items.HasChildNodes)
            {
                foreach (XmlNode item in items.ChildNodes)
                {
                    var row = new GridRow();

                    if (item.Attributes != null)
                    {
                        row.Id = int.Parse(item.Attributes["id"].Value);
                        row.SortOrder = int.Parse(item.Attributes["sortOrder"].Value);
                    }

                    foreach (XmlNode node in item.ChildNodes)
                    {
                        if (node.Attributes != null)
                        {
                            var cell = new GridCell()
                                           {
                                               Alias = node.Name,
                                               Name = node.Attributes["nodeName"].Value,
                                               DataType = int.Parse(node.Attributes["nodeType"].Value),
                                               Value = node.InnerText
                                           };

                            row.Add(cell);
                        }
                    }

                    this.Add(row);
                }
            }
        }

        /// <summary>
        /// Converts the collection to a <see cref="DynamicXml"/> object.
        /// </summary>
        /// <returns>The dynamic xml.</returns>
        public DynamicXml AsDynamicXml()
        {
            throw new NotImplementedException();
        }
    }
}