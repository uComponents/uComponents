namespace uComponents.DataTypes.DataTypeGrid.Handlers.DataTypes
{
    using System.Linq;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco;
    using umbraco.editorControls.dropdownlist;

    /// <summary>
    /// Factory for the <see cref="DropdownListDataType"/>
    /// </summary>
    [DataTypeHandler(Priority = -1)]
    public class DropdownListDataTypeHandler : BaseDataTypeHandler<DropdownListDataType>
    {
        /// <summary>
        /// Method for customizing the way the <paramref name="dataType" /> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <paramref name="dataType" />.</remarks>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(DropdownListDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            var p = uQuery.GetPreValues(dataType.DataTypeDefinitionId);

            var v = p.FirstOrDefault(x => x.Id.ToString().Equals(value));

            if (v != null)
            {
                // Use dictionary item if prefixed with '#'
                if (v.Value.StartsWith("#"))
                {
                    return uQuery.GetDictionaryItem(v.Value.Substring(1), v.Value);
                }

                return v.Value;
            }

            return value;
        }
    }
}