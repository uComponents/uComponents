namespace uComponents.DataTypes.DataTypeGrid
{
    using System;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using Umbraco.Core;
    using Umbraco.Core.PropertyEditors;

    /// <summary>
    /// Model binder for the DataTypeGrid data-type.
    /// </summary>
    public class DtgPropertyEditorValueConverter : IPropertyEditorValueConverter
    {
        /// <summary>
        /// Returns true if this converter can perform the value conversion for the specified property editor id
        /// </summary>
        /// <param name="propertyEditorId">The property editor id.</param>
        /// <param name="docTypeAlias">The doc type alias.</param>
        /// <param name="propertyTypeAlias">The property type alias.</param>
        /// <returns><c>true</c> if [is converter for] [the specified property editor id]; otherwise, <c>false</c>.</returns>
        public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
        {
            var guid = new Guid(DataTypeConstants.DataTypeGridId);

            return guid.Equals(propertyEditorId);
        }

        /// <summary>
        /// Attempts to convert the value specified into a useable value on the front-end
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The operation attempt.</returns>
        /// <remarks>This is used to convert the value stored in the repository into a usable value on the front-end.
        /// For example, if a 0 or 1 is stored for a boolean, we'd want to convert this to a real boolean.
        /// Also note that the value might not come in as a 0 or 1 but as a "0" or "1"</remarks>
        public Attempt<object> ConvertPropertyValue(object value)
        {
            var attempt = new Attempt<object>(true, new GridRowCollection(value.ToString()));

            return attempt;
        }
    }
}