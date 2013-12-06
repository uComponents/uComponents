using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using uComponents.DataTypes;
using uComponents.DataTypes.Shared.Models;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace uComponents.PropertyEditors.ValueConverters.SqlAutoComplete
{
	/// <summary>
	/// Property-editor value converter for the SqlAutoComplete data-type.
	/// </summary>
	public class SqlAutoCompletePropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		/// <summary>
		/// Returns true if this converter can perform the value conversion for the specified property editor id
		/// </summary>
		/// <param name="propertyEditorId"></param>
		/// <param name="docTypeAlias"></param>
		/// <param name="propertyTypeAlias"></param>
		/// <returns></returns>
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.SqlAutoCompleteId).Equals(propertyEditorId);
		}

		/// <summary>
		/// Attempts to convert the value specified into a useable value on the front-end
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (value != null && value.ToString().Length > 0)
			{
				var data = value.ToString();

				// check if the data is XML
				if (XmlHelper.CouldItBeXml(data))
				{
					// parse the XML
					var xdoc = XDocument.Parse(data);
					if (xdoc != null)
					{
						// select the Item nodes
						var nodes = xdoc.XPathSelectElements("//Item");
						if (nodes != null && nodes.Count() > 0)
						{
							// iterate over the nodes; building collection of AutoCompleteItem(s)
							var items = new List<AutoCompleteItem>();
							foreach (var node in nodes)
							{
								// check if the node has any attributes
								if (!node.HasAttributes)
									continue;

								var item = new AutoCompleteItem();

								// loop through the attributes, setting the properties accordingly
								foreach (var attribute in node.Attributes())
								{
									switch (attribute.Name.LocalName)
									{
										case "Text":
											item.Text = attribute.Value;
											break;

										case "Value":
											item.Value = attribute.Value;
											break;

										default:
											break;
									}
								}

								items.Add(item);
							}

							return new Attempt<object>(true, items);
						}
					}
				}
			}

			return Attempt<object>.False;
		}
	}
}