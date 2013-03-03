namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using uComponents.DataTypes.DataTypeGrid.Model;
    using uComponents.DataTypes.UrlPicker;
    using uComponents.DataTypes.UrlPicker.Dto;

    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="UrlPickerDataType"/> class.
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class UrlPickerDataTypeFactory : BaseDataTypeFactory<UrlPickerDataType>
    {
        /// <summary>
        /// Method for performing special actions while creating the <see cref="IDataType">datatype</see> editor.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called when the grid creates the editor controls for the specified <see cref="IDataType">datatype</see>.</remarks>
        public override void Configure(UrlPickerDataType dataType, DataTypeLoadEventArgs eventArgs)
        {
            if (dataType.Data.Value != null && dataType.ContentEditor.State == null)
            {
                dataType.ContentEditor.State = UrlPickerState.Deserialize((string)dataType.Data.Value);
            }
        }
    }
}