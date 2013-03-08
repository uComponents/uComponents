namespace uComponents.DataTypes.DataTypeGrid.Factories.DataTypes
{
    using System.Linq;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco;
    using umbraco.editorControls.listbox;

    /// <summary>
    /// Factory for the <see cref="ListBoxKeysDataType"/>
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class ListBoxKeysDataTypeFactory : BaseDataTypeFactory<ListBoxKeysDataType>
    {
        /// <summary>
        /// Method for customizing the way the <paramref name="dataType" /> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <paramref name="dataType" />.</remarks>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(ListBoxKeysDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            var p = uQuery.GetPreValues(dataType.DataTypeDefinitionId);

            var v = p.Where(x => value.Split(',').Contains(x.Id.ToString())).Select(x => x.Value);

            return string.Join(", ", v.ToArray());
        }
    }
}