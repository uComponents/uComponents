using System;
using uComponents.DataTypes;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace uComponents.PropertyEditors.ValueConverters.CountryPicker
{
	public class CountryPickerPropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.CountryPickerId).Equals(propertyEditorId);
		}

		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (UmbracoContext.Current != null && value != null && value.ToString() != string.Empty)
			{
				return new Attempt<object>(true, value.ToString().Split(uComponents.Core.Constants.Common.COMMA));
			}

			return Attempt<object>.False;
		}
	}
}