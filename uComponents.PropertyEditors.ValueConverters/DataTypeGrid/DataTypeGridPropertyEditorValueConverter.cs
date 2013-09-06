using System;
using uComponents.DataTypes;
using uComponents.DataTypes.DataTypeGrid.Model;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace uComponents.PropertyEditors.ValueConverters.DataTypeGrid
{
    using System.Linq;

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
        /// <remarks>This is used to convert the value stored in the repository into a usable value on the front-end.
        /// For example, if a 0 or 1 is stored for a boolean, we'd want to convert this to a real boolean.
        /// Also note that the value might not come in as a 0 or 1 but as a "0" or "1"</remarks>
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