// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.Core.DataTypes.DataTypeGrid.DataTypeFunctions
{
    using System.Web.UI;

    using umbraco.cms.businesslogic.member;
    using umbraco.editorControls.memberpicker;

    /// <summary>
    /// DTG extensions for the MemberPicker DataType
    /// </summary>
    internal class MemberPickerDataTypeFunctions
    {
        /// <summary>
        /// Converts the datatype value to a DTG compatible string
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <returns></returns>
        public static string ToDtgString(MemberPickerDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            int id;
            int.TryParse(value, out id);

            if (id > 0)
            {
                var m = new Member(id);

                // Return member name
                return m.Text;
            }

            return value;
        }

        /// <summary>
        /// Configures the datatype to be compatible with DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <param name="container">The container.</param>
        public static void ConfigureForDtg(MemberPickerDataType dataType, Control container)
        {
            // Set default value to blank to prevent YSOD
            if (dataType.Data.Value == null)
            {
                dataType.Data.Value = string.Empty;
            }
        }
    }
}
