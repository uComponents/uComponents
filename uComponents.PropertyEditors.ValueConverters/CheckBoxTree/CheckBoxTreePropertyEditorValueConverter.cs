using System;
using uComponents.DataTypes;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

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
			throw new NotImplementedException();
		}
	}
}