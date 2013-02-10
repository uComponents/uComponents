namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Linq;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco;
    using umbraco.editorControls.listbox;

    /// <summary>
    /// Factory for the <see cref="ListBoxDataType"/>
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class ListBoxDataTypeFactory : BaseDataTypeFactory<ListBoxDataType>
    {
        /// <summary>
        /// Method for customizing the way the <typeparamref name="TDataType">datatype</typeparamref> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <typeparamref name="TDataType">datatype</typeparamref>.</remarks>
        /// <param name="dataType">The <typeparamref name="TDataType">datatype</typeparamref> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(ListBoxDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            var p = uQuery.GetPreValues(dataType.DataTypeDefinitionId);

            var v = p.Where(x => value.Split(',').Contains(x.Id.ToString())).Select(x => x.Value);

            return string.Join(", ", v.ToArray());
        }
    }
}