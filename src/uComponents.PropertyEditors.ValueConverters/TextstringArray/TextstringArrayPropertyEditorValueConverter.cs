using System;
using System.Collections.Generic;
using System.Xml;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace uComponents.DataTypes.RazorDataTypeModels.TextstringArray
{
	/// <summary>
	/// Property-editor value converter for the TextstringArray data-type.
	/// </summary>
	public class TextstringArrayPropertyEditorValueConverter : IPropertyEditorValueConverter
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
			return Guid.Parse(DataTypeConstants.CheckBoxTreeId).Equals(propertyEditorId);
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