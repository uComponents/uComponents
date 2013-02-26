namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using uComponents.DataTypes.EnumCheckBoxList;

    /// <summary>
    /// Factory for the <see cref="EnumCheckBoxListDataType"/> datatype.
    /// </summary>
    public class EnumCheckBoxListDataTypeFactory : BaseDataTypeFactory<EnumCheckBoxListDataType>
    {
        public override void Initialize(EnumCheckBoxListDataType dataType, System.Web.UI.Control container)
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