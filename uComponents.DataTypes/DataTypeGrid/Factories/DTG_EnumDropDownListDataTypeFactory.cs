namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using uComponents.DataTypes.DataTypeGrid.Model;
    using uComponents.DataTypes.EnumDropDownList;

    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="EnumDropDownListDataType"/> datatype.
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class EnumDropDownListDataTypeFactory : BaseDataTypeFactory<EnumDropDownListDataType>
    {
        /// <summary>
        /// Method for performing special actions <b>before</b> creating the <see cref="IDataType" /> editor.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called <b>before</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        public override void Initialize(EnumDropDownListDataType dataType, DataTypeLoadEventArgs eventArgs)
        {
            var editor = dataType.DataEditor.Editor as EnumDropDownListDataEditor;

            if (editor != null)
            {
                // Set selected value
                if (dataType.Data.Value != null) 
                { 
                    // Ensure stored values are set
                    editor.DropDownList.Load += (sender, args) =>
                    {
                        if (editor.DropDownList.SelectedValue == "-1")
                        {
                            // Get selected items from Node Name or Node Id
                            var dropDownListItem = editor.DropDownList.Items.FindByValue(dataType.Data.Value.ToString());

                            if (dropDownListItem != null)
                            {
                                // Reset selected item
                                editor.DropDownList.SelectedItem.Selected = false;

                                // Set new selected item
                                dropDownListItem.Selected = true;
                            }
                        }
                    };
                }
            }
        }

        /// <summary>
        /// Saves the specified data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="eventArgs">The <see cref="DataTypeSaveEventArgs"/> instance containing the event data.</param>
        public override void Save(EnumDropDownListDataType dataType, DataTypeSaveEventArgs eventArgs)
        {
            var editor = dataType.DataEditor.Editor as EnumDropDownListDataEditor;

            if (editor != null)
            {
                dataType.Data.Value = editor.DropDownList.SelectedValue;
            }
        }
    }
}