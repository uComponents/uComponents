// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.Core.DataTypes.DataTypeGrid.Extensions
{
    using System.Web.UI;

    using uComponents.Core.DataTypes.DataTypeGrid.DataTypeFunctions;

    using umbraco.editorControls.datepicker;
    using umbraco.editorControls.dropdownlist;
    using umbraco.editorControls.listbox;
    using umbraco.editorControls.tinyMCE3;
    using umbraco.interfaces;

    /// <summary>
    /// Extension methods for datatypes
    /// </summary>
    internal static class DataTypeExtensions
    {
        /// <summary>
        /// Gets the specified data type in DTG compatible way.
        /// </summary>
        /// <param name="dataType">The datatype.</param>
        /// <returns></returns>
        public static string ToDtgString(this IDataType dataType)
        {
            var guid = dataType.Id.ToString();
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            // Dropdown List
            if (dataType is DropdownListDataType)
            {
                return DropdownListDataTypeFunctions.ToDtgString(dataType as DropdownListDataType);
            }
            
            // Dropdown List Multiple
            if (dataType is ListBoxDataType)
            {
                return ListBoxDataTypeFunctions.ToDtgString(dataType as ListBoxDataType);
            }
            
            // Media Picker
            if (dataType is umbraco.editorControls.mediapicker.MemberPickerDataType)
            {
                return MediaPickerDataTypeFunctions.ToDtgString(dataType as umbraco.editorControls.mediapicker.MemberPickerDataType);
            }

            // Date Picker || Date Picker with time
            if (dataType is DateDataType) 
            {
                return DateDataTypeFunctions.ToDtgString(dataType as DateDataType);
            }

            // Member Picker
            if (dataType is umbraco.editorControls.memberpicker.MemberPickerDataType)
            {
                return MemberPickerDataTypeFunctions.ToDtgString(dataType as umbraco.editorControls.memberpicker.MemberPickerDataType);
            }

            return value;
        }

        /// <summary>
        /// Configures for DTG.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="container">The container.</param>
        public static void ConfigureForDtg(this IDataType dataType, Control container)
        {
            // Date Picker || Date Picker with time
            if (dataType is DateDataType)
            {
                DateDataTypeFunctions.ConfigureForDtg(dataType as DateDataType, container);
            }
            // Member picker
            else if (dataType is umbraco.editorControls.memberpicker.MemberPickerDataType)
            {
                MemberPickerDataTypeFunctions.ConfigureForDtg(dataType as umbraco.editorControls.memberpicker.MemberPickerDataType, container);
            }
        }

        /// <summary>
        /// Saves for DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        public static void SaveForDtg(this IDataType dataType)
        {
            // Richtext Editor
            if (dataType is tinyMCE3dataType)
            {
                TinyMCE3DataTypeFunctions.SaveForDtg(dataType as tinyMCE3dataType);
            }
            else
            {
                dataType.DataEditor.Save();
            }
        }
    }
}
