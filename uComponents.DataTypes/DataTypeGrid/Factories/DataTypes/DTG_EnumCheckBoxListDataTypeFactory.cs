namespace uComponents.DataTypes.DataTypeGrid.Factories.DataTypes
{
    using uComponents.DataTypes.DataTypeGrid.Model;
    using uComponents.DataTypes.EnumCheckBoxList;

    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="EnumCheckBoxListDataType"/> datatype.
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class EnumCheckBoxListDataTypeFactory : BaseDataTypeFactory<EnumCheckBoxListDataType>
    {
        /// <summary>
        /// Method for performing special actions <b>before</b> creating the <see cref="IDataType" /> editor.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called <b>before</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        public override void Initialize(EnumCheckBoxListDataType dataType, DataTypeLoadEventArgs eventArgs)
        {
            var editor = dataType.DataEditor.Editor as EnumCheckBoxListDataEditor;

            if (editor != null && dataType.Data.Value != null)
            {
                // Ensure stored values are set
                editor.CheckBoxList.Load += (sender, args) =>
                    {
                        if (editor.CheckBoxList.SelectedItem == null)
                        {
                            editor.SetSelectedValues(dataType.Data.Value.ToString());
                        }
                    };
            }
        }
    }
}