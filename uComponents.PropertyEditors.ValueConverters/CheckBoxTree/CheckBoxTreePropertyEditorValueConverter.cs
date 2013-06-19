using System;
using uComponents.DataTypes;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace uComponents.PropertyEditors.ValueConverters.CheckBoxTree
{
	public class CheckBoxTreePropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.CheckBoxTreeId).Equals(propertyEditorId);
		}

		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (UmbracoContext.Current != null && value != null && value.ToString().Length > 0)
			{
				var data = value.ToString();
				var nodeIds = XmlHelper.CouldItBeXml(data) ? uQuery.GetXmlIds(data) : uQuery.ConvertToIntArray(uQuery.GetCsvIds(data));

				var helper = new UmbracoHelper(UmbracoContext.Current);
				return new Attempt<object>(true, helper.TypedContent(nodeIds));
			}

			return Attempt<object>.False;
		}
	}
}