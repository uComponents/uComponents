namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System;
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco;
    using umbraco.cms.businesslogic.media;
    using umbraco.editorControls.mediapicker;
    using umbraco.interfaces;

    /// <summary>a
    /// Factory for the <see cref="MemberPickerDataType"/>
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class MediaPickerDataTypeFactory : BaseDataTypeFactory<MemberPickerDataType>
    {
        /// <summary>
        /// Method for customizing the way the <paramref name="dataType" /> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <paramref name="dataType" />.</remarks>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(MemberPickerDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            int id;
            int.TryParse(value, out id);

            if (id > 0)
            {
                try
                {
                    var m = new Media(id);

                    // Return thumbnail if media type is Image
                    if (m.ContentType.Alias.Equals("Image"))
                    {
                        return
                            string.Format(
                                "<a href='editMedia.aspx?id={2}' title='Edit media'><img src='{0}' alt='{1}'/></a>",
                                m.GetImageThumbnailUrl(),
                                m.Text,
                                m.Id);
                    }

                    return
                        string.Format(
                            "<a href='editMedia.aspx?id={0}' title='Edit media'>{1}</a>",
                            m.Id,
                            m.Text);
                }
                catch (Exception ex)
                {
                    return string.Format("<span style='color: red;'>{0}</span>", ex.Message);
                }
            }

            return value;
        }

        /// <summary>
        /// Method for getting the backing object for the specified <paramref name="dataType" />.
        /// </summary>
        /// <remarks>Called when the method <see cref="GridCell.GetObject{T}()"/> method is called on a <see cref="GridCell"/>.</remarks>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <returns>The backing object.</returns>
        public override object GetObject(MemberPickerDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            int id;
            int.TryParse(value, out id);

            if (id > 0)
            {
                return new Media(id);
            }

            return default(Media);
        }

        /// <summary>
        /// Method for getting the control to use when validating the specified <see cref="IDataType" />.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="editorControl">The <see cref="IDataType" /> editor control.</param>
        /// <returns>The control to validate.</returns>
        public override Control GetControlToValidate(MemberPickerDataType dataType, Control editorControl)
        {
            var value = editorControl.Controls[0];

            return value;
        }
    }
}