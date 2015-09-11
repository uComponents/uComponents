using System;
using uComponents.DataTypes;
using uComponents.DataTypes.MultiUrlPicker.Dto;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace uComponents.PropertyEditors.ValueConverters.MultiUrlPicker
{
	/// <summary>
	/// Property-editor value converter for the MultiUrlPicker data-type.
	/// </summary>
	public class MultiUrlPickerPropertyEditorValueConverter : IPropertyEditorValueConverter
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
			return Guid.Parse(DataTypeConstants.MultiUrlPickerId).Equals(propertyEditorId);
		}

		/// <summary>
		/// Attempts to convert the value specified into a useable value on the front-end
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (value != null && value.ToString().Length > 0)
				return new Attempt<object>(true, MultiUrlPickerState.Deserialize(value.ToString()));

			return Attempt<object>.False;
		}
	}
}