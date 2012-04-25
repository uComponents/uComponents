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
	public class DTG_ModelBinder : IRazorDataTypeModel
	{
		/// <summary>
		/// Inits the specified current node id.
		/// </summary>
		/// <param name="CurrentNodeId">The current node id.</param>
		/// <param name="PropertyData">The property data.</param>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public bool Init(int CurrentNodeId, string PropertyData, out object instance)
		{
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
							var value = new StoredValueForModel();
							value.Alias = node.Name;
							value.Name = node.Attributes["nodeName"].Value;
							value.NodeType = int.Parse(node.Attributes["nodeType"].Value);
							value.Value = node.InnerText;

							valueRow.Cells.Add(value);
			            }

			            values.Add(valueRow);
			        }
			    }

			}

			instance = values;

			return true;
		}

		/// <remarks>
		/// We use the <c>uComponents.RazorModels.DataTypeGrid.StoredValueForModel</c> object 
		/// instead of the <c>uComponents.DataTypes.DataTypeGrid.Model.StoredValue</c> object 
		/// to override the <c>uComponents.DataTypes.DataTypeGrid.Model.StoredValue.Value</c> 
		/// property, as the type is <c>umbraco.interfaces.IDataType</c>, which we do not need to 
		/// access at the front-end.
		/// </remarks>
		public class StoredValueForModel : StoredValue
		{
			/// <summary>
			/// Gets or sets the type of the node.
			/// </summary>
			/// <value>The type of the node.</value>
			public int NodeType { get; set; }

			/// <summary>
			/// Gets or sets the value.
			/// </summary>
			/// <value>The value.</value>
			public new string Value { get; set; }
		}
	}
}