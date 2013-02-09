// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.DataTypeOverrides
{
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Interfaces;

    using umbraco.cms.businesslogic.member;
    using umbraco.editorControls.memberpicker;

    /// <summary>
    /// DTG extensions for the MemberPicker DataType
    /// </summary>
    internal class MemberPickerDataTypeFactory : IDataTypeFactory<MemberPickerDataType>
    {
        #region Implementation of IDataTypeFactory<MemberPickerDataType>

        /// <summary>
        /// Converts the datatype value to a DTG compatible string
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <returns>A human-readable string</returns>
        public string ToDtgString(MemberPickerDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            int id;
            int.TryParse(value, out id);

            if (id > 0)
            {
                var m = new Member(id);

                // Return member name
                return string.Format("<a href='editMember.aspx?id={0}' title='Edit content'>{1}</a>", m.Id, m.Text);
            }

            return value;
        }

        /// <summary>
        /// Configures the datatype to be compatible with DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <param name="container">The container.</param>
        public void Configure(MemberPickerDataType dataType, Control container)
        {
            // Set default value to blank to prevent YSOD
            if (dataType.Data.Value == null)
            {
                dataType.Data.Value = string.Empty;
            }
        }

        /// <summary>
        /// Saves the datatype for DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        public void SaveForDtg(MemberPickerDataType dataType)
        {
        }

        #endregion
    }
}
