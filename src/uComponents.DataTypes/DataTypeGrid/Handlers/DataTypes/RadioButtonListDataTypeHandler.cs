namespace uComponents.DataTypes.DataTypeGrid.Handlers.DataTypes
{
    using System.Linq;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco;
    using umbraco.editorControls.radiobuttonlist;
    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="RadioButtonListDataType"/>.
    /// </summary>
    [DataTypeHandler(Priority = -1)]
    public class RadioButtonListDataTypeHandler : BaseDataTypeHandler<RadioButtonListDataType>
    {
        /// <summary>
        /// Method for customizing the way the <see cref="IDataType">datatype</see> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(RadioButtonListDataType dataType)
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