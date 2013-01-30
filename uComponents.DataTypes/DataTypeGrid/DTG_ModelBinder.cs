using System.Collections.Generic;
using System.Xml;
using uComponents.DataTypes.DataTypeGrid.Model;
using umbraco.MacroEngines;

namespace uComponents.DataTypes.DataTypeGrid
{
	/// <summary>
	/// Model binder for the DataTypeGrid data-type.
	/// </summary>
	[RazorDataTypeModel(DataTypeConstants.DataTypeGridId)]
	public class ModelBinder : IRazorDataTypeModel
	{
		/// <summary>
		/// Initializes the the model for specified node id.
		/// </summary>
		/// <param name="CurrentNodeId">The current node id.</param>
		/// <param name="PropertyData">The property data.</param>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public bool Init(int CurrentNodeId, string PropertyData, out object instance)
		{
			if (!Settings.RazorModelBindingEnabled)
			{
				instance = new DynamicXml(PropertyData);

				return true;
			}

			var values = new List<StoredValueRow>();

			if (!string.IsNullOrEmpty(PropertyData))
			{
			    var doc = new XmlDocument();
			    doc.LoadXml(PropertyData);

				var items = doc.DocumentElement;

				if (items.HasChildNodes)
			    {
			        foreach (XmlNode item in items.ChildNodes)
			        {
			
			            var valueRow = new StoredValueRow();

						if (item.Attributes != null)
			            {
							valueRow.Id = int.Parse(item.Attributes["id"].Value);
			            }

						foreach (XmlNode node in item.ChildNodes)
			            {
							var value = new StoredValueForModel
							                {
							                    Alias = node.Name,
							                    Name = node.Attributes["nodeName"].Value,
							                    NodeType = int.Parse(node.Attributes["nodeType"].Value),
							                    Value = node.InnerText
							                };

			                valueRow.Cells.Add(value);
			            }

			            values.Add(valueRow);
			        }
			    }

			}

			instance = values;

			return true;
		}		
	}
}