using System;
using System.Web.Script.Serialization;
using uComponents.DataTypes;
using uComponents.DataTypes.MultiUrlPicker.Dto;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace uComponents.PropertyEditors.ValueConverters.MultiUrlPicker
{
	public class MultiUrlPickerPropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.MultiUrlPickerId).Equals(propertyEditorId);
		}

		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (value != null && value.ToString().Length > 0)
			{
				return new Attempt<object>(true, new JavaScriptSerializer().Deserialize<MultiUrlPickerState>(value.ToString()));
			}

			return Attempt<object>.False;
		}
	}
}