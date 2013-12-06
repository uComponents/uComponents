using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using uComponents.Core;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.DataTypes.DataTypeGrid.Model
{
    public class StoredValueRowCollection : List<StoredValueRow>
    {
        private IEnumerable<PreValueRow> columnConfigurations;

        public StoredValueRowCollection(IEnumerable<PreValueRow> columnConfigurations)
        {
            this.columnConfigurations = columnConfigurations;
        }

        public StoredValueRowCollection(IEnumerable<PreValueRow> columnConfigurations, string xml)
            : this(columnConfigurations)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                // Create and add XML declaration. 
                var xmldecl = doc.CreateXmlDeclaration("1.0", null, null);
                var root = doc.DocumentElement;
                doc.InsertBefore(xmldecl, root);

                // Get stored values from database
                if (root.ChildNodes.Count > 0)
                {
                    foreach (XmlNode container in root.ChildNodes)
                    {
                        // <DataTypeGrid>
                        var valueRow = new StoredValueRow();

                        if (container.Attributes["id"] != null)
                        {
                            valueRow.Id = int.Parse(container.Attributes["id"].Value);
                        }

                        if (container.Attributes["sortOrder"] != null)
                        {
                            valueRow.SortOrder = int.Parse(container.Attributes["sortOrder"].Value);
                        }

                        foreach (PreValueRow config in this.columnConfigurations)
                        {
                            var value = new StoredValue { Name = config.Name, Alias = config.Alias };

                            var datatypeid = config.DataTypeId;

                            if (datatypeid != 0)
                            {
                                var dtd = DataTypeDefinition.GetDataTypeDefinition(datatypeid);
                                var dt = dtd.DataType;
                                dt.Data.Value = string.Empty;
                                value.Value = dt;

                                foreach (XmlNode node in container.ChildNodes)
                                {
                                    if (config.Alias.Equals(node.Name))
                                    {
                                        try
                                        {
                                            value.Value.Data.Value = node.InnerText;
                                        }
                                        catch (Exception ex)
                                        {
                                            Helper.Log.Warn<DataType>(string.Format("DataTypeGrid", "Stored data ({0}) for '{1}' is incompatible with the datatype '{2}'", node.InnerText, value.Alias, value.Value.DataTypeName));
                                        }
                                    }
                                }

                                valueRow.Cells.Add(value);
                            }
                        }

                        this.Add(valueRow);
                    }
                }
            }
        }

        public override string ToString()
        {
            var str = string.Empty;

            // Only add elements if there a rows
            if (this.Any())
            {
                // Start data
                str = "<items>";

                foreach (var container in this.OrderBy(x => x.SortOrder))
                {
                    // Start
                    str += string.Concat("<item id='", container.Id.ToString(), "' sortOrder='", container.SortOrder, "'>");

                    foreach (var v in container.Cells)
                    {
                        if (v.Value.Data.Value == null)
                        {
                            v.Value.Data.Value = string.Empty;
                        }

                        str += string.Concat(
                            "<",
                            v.Alias,
                            " nodeName='",
                            v.Name,
                            "' nodeType='",
                            v.Value.DataTypeDefinitionId,
                            "'>",
                            HttpUtility.HtmlEncode(v.Value.Data.Value.ToString()),
                            "</",
                            v.Alias,
                            ">");
                    }

                    // End row
                    str += "</item>";
                }

                // End data
                str += "</items>";
            }

            return str;
        }
    }
}
