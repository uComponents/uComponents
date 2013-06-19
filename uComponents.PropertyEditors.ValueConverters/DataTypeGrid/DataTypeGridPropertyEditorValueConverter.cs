using System;
using uComponents.DataTypes;
using uComponents.DataTypes.DataTypeGrid.Model;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace uComponents.PropertyEditors.ValueConverters.DataTypeGrid
{
	public class DataTypeGridPropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.DataTypeGridId).Equals(propertyEditorId);
		}

		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (UmbracoContext.Current != null && value != null && value.ToString() != string.Empty)
			{
				try
				{
					return new Attempt<object>(true, new GridRowCollection(value.ToString()));
				}
				catch (Exception)
				{
					return Attempt<object>.False;
				}
			}

			return Attempt<object>.False;
		}
	}
}