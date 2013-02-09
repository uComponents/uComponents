// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.DataTypeOverrides
{
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Interfaces;

    using umbraco.cms.businesslogic.media;
    using umbraco.editorControls.mediapicker;
    using umbraco;

    /// <summary>
    /// DTG extensions for the MediaPicker DataType
    /// </summary>
    internal class MediaPickerDataTypeFactory : IDataTypeFactory<MemberPickerDataType>
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
                var m = new Media(id);

                // Return thumbnail if media type is Image
                if (m.ContentType.Alias.Equals("Image"))
                {
                    return string.Format("<a href='editMedia.aspx?id={2}' title='Edit media'><img src='{0}' alt='{1}'/></a>", m.GetImageThumbnailUrl(), m.Text, m.Id);
                }
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
