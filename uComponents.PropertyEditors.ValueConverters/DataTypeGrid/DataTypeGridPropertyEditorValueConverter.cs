using System;
using System.Linq;
using uComponents.DataTypes;
using uComponents.DataTypes.DataTypeGrid.Model;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace uComponents.PropertyEditors.ValueConverters.DataTypeGrid
{
	/// <summary>
	/// Property-editor value converter for the DataTypeGrid data-type.
	/// </summary>
	public class DataTypeGridPropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		/// <summary>
		/// Returns true if this converter can perform the value conversion for the specified property editor id
		/// </summary>
		/// <param name="propertyEditorId">The property editor unique identifier.</param>
		/// <param name="docTypeAlias">The document type alias.</param>
		/// <param name="propertyTypeAlias">The property type alias.</param>
		/// <returns>True if this class can convert the specified property editor.</returns>
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.DataTypeGridId).Equals(propertyEditorId);
		}

		/// <summary>
		/// Attempts to convert the value specified into a useable value on the front-end
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The conversion attempt.</returns>
		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (value != null && value.ToString().Length > 0)
			{
				try
				{
					var c = new GridRowCollection(value.ToString()).OrderBy(x => x.SortOrder).ToList();

					return new Attempt<object>(true, (GridRowCollection)c);
				}
				catch
				{
					return Attempt<object>.False;
				}
			}

			return Attempt<object>.False;
		}
	}
}