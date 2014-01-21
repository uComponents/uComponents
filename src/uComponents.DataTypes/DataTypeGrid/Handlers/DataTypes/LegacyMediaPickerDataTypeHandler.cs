namespace uComponents.DataTypes.DataTypeGrid.Handlers.DataTypes
{
    using System;
    using System.Web.UI;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    using uComponents.Core;
    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco;
    using umbraco.editorControls.mediapicker;
    using umbraco.interfaces;

    /// <summary>a
    /// Factory for the <see cref="MemberPickerDataType"/>
    /// </summary>
    [DataTypeHandler(Priority = -1)]
    public class LegacyMediaPickerDataTypeHandler : BaseDataTypeHandler<MediaPickerDataType>
    {
        /// <summary>
        /// The umbraco helper
        /// </summary>
        private readonly UmbracoHelper umbracoHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPickerDataType"/> class.
        /// </summary>
        public LegacyMediaPickerDataTypeHandler()
        {
            this.umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
        }

        /// <summary>
        /// Method for customizing the way the <paramref name="dataType" /> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <paramref name="dataType" />.</remarks>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(MediaPickerDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            int id;
            int.TryParse(value, out id);

            if (id > 0)
            {
                try
                {
                    var m = this.umbracoHelper.TypedMedia(id);

                    // Return thumbnail if media type is Image
                    if (m.DocumentTypeAlias.Equals("Image"))
                    {
                        return
                            string.Format(
                                "<a href='editMedia.aspx?id={2}' title='Edit media'><img src='{0}' alt='{1}'/></a>",
                                this.GetThumbnailUrl(m),
                                m.Name,
                                m.Id);
                    }

                    return
                        string.Format(
                            "<a href='editMedia.aspx?id={0}' title='Edit media'>{1}</a>",
                            m.Id,
                            m.Name);
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
        /// <remarks>Called when the method <see cref="GridCell.GetPropertyValue()" /> method is called on a <see cref="GridCell"/>.</remarks>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <returns>The backing object.</returns>
        public override object GetPropertyValue(MediaPickerDataType dataType)
        {
            // Try to use registered property value converter first
            var converter = Helper.Resolvers.GetPropertyValueConverter(dataType);

            if (converter != null)
            {
                return converter.ConvertPropertyValue(dataType.Data.Value).Result;
            }

            // Fall back to custom value conversion
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            int id;
            int.TryParse(value, out id);

            if (id > 0)
            {
                return uQuery.GetMedia(id);
            }

            return default(Media);
        }

        /// <summary>
        /// Method for getting the control to use when validating the specified <see cref="IDataType" />.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="editorControl">The <see cref="IDataType" /> editor control.</param>
        /// <returns>The control to validate.</returns>
        public override Control GetControlToValidate(MediaPickerDataType dataType, Control editorControl)
        {
            var value = editorControl.Controls[0];

            return value;
        }

        /// <summary>
        /// Gets the thumbnail URL.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The thumbnail URL.</returns>
        private string GetThumbnailUrl(IPublishedContent content)
        {
            var extension = content.GetPropertyValue<string>("umbracoExtension");

            return content.Url.Replace("." + extension, "_thumb.jpg");
        }
    }
}