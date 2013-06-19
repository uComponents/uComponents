using System;
using System.Web.Script.Serialization;
using uComponents.DataTypes;
using uComponents.DataTypes.UrlPicker.Dto;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace uComponents.PropertyEditors.ValueConverters.UrlPicker
{
	public class UrlPickerModelBinder : IPropertyEditorValueConverter
	{
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.UrlPickerId).Equals(propertyEditorId);
		}

		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (UmbracoContext.Current != null && value != null && value.ToString() != string.Empty)
			{
				return new Attempt<object>(true, new JavaScriptSerializer().Deserialize<UrlPickerState>(value.ToString()));
			}

			return Attempt<object>.False;
		}
	}
}