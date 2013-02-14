namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Linq;
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco;
    using umbraco.editorControls.dropdownlist;

    /// <summary>
    /// Factory for the <see cref="DropdownListDataType"/>
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class DropdownListDataTypeFactory : BaseDataTypeFactory<DropdownListDataType>
    {
        /// <summary>
        /// Method for customizing the way the <typeparamref name="DropdownDataType">datatype</typeparamref> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <typeparamref name="DropdownDataType">datatype</typeparamref>.</remarks>
        /// <param name="dataType">The <typeparamref name="DropdownDataType">datatype</typeparamref> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(DropdownListDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            var p = uQuery.GetPreValues(dataType.DataTypeDefinitionId);

            var v = p.FirstOrDefault(x => x.Id.ToString().Equals(value));

            if (v != null)
            {
                return v.Value;
            }

            return value;
        }
    }
}