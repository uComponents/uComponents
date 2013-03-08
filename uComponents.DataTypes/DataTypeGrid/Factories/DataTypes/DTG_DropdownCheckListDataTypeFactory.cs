namespace uComponents.DataTypes.DataTypeGrid.Factories.DataTypes
{
    using uComponents.DataTypes.DataTypeGrid.Model;
    using uComponents.DataTypes.DropdownCheckList;

    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="DDCList_DataType"/>
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class DropdownCheckListDataTypeFactory : BaseDataTypeFactory<DDCList_DataType>
    {
        /// <summary>
        /// Method for performing special actions <b>before</b> creating the <see cref="IDataType" /> editor.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called <b>before</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        public override void Initialize(DDCList_DataType dataType, DataTypeLoadEventArgs eventArgs)
        {
            if (dataType.Data.Value == null)
            {
                dataType.Data.Value = string.Empty;
            }
        }
    }
}