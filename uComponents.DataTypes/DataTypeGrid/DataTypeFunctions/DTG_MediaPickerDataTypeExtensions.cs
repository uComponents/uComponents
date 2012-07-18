// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.DataTypeFunctions
{
    using System.Web.UI;

    using uComponents.Core.DataTypes.DataTypeGrid.Interfaces;
	using umbraco.cms.businesslogic.media;
    using umbraco.editorControls.mediapicker;

    /// <summary>
    /// DTG extensions for the MediaPicker DataType
    /// </summary>
    internal class MediaPickerDataTypeFunctions : IDataTypeFunctions<MemberPickerDataType>
    {
        #region Implementation of IDataTypeFunctions<MemberPickerDataType>

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
                var m = new Media(id);

                // Return thumbnail if media type is Image
                if (m.ContentType.Alias.Equals("Image"))
                {
                    return string.Format("<img src='{0}' alt='{1}'/>", m.GetImageThumbnailUrl(), m.Text);
                }
            }

            return value;
        }

        /// <summary>
        /// Configures the datatype to be compatible with DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <param name="container">The container.</param>
        public void ConfigureForDtg(MemberPickerDataType dataType, Control container)
        {
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
