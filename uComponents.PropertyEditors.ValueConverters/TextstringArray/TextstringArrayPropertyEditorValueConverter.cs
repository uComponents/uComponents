using System;
using System.Collections.Generic;
using System.Xml;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace uComponents.DataTypes.RazorDataTypeModels.TextstringArray
{
	public class TextstringArrayPropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.CheckBoxTreeId).Equals(propertyEditorId);
		}

		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (value != null && value.ToString().Length > 0)
			{
				var items = new List<string[]>();

				var xml = new XmlDocument();
				xml.LoadXml(value.ToString());

				foreach (XmlNode node in xml.SelectNodes("/TextstringArray/values"))
				{
					var item = new List<string>();
					foreach (XmlNode child in node.SelectNodes("value"))
					{
						item.Add(child.InnerText);
					}

					items.Add(item.ToArray());
				}

				return new Attempt<object>(true, items);
			}

			return Attempt<object>.False;
		}
	}
}