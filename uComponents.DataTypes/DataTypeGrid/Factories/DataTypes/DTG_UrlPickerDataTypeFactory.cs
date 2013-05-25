namespace uComponents.DataTypes.DataTypeGrid.Factories.DataTypes
{
    using Umbraco.Web;

    using uComponents.Core;
    using uComponents.DataTypes.DataTypeGrid.Model;
    using uComponents.DataTypes.UrlPicker;
    using uComponents.DataTypes.UrlPicker.Dto;

    using umbraco;
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
            // Deserialize stored value
            if (dataType.Data.Value != null && !string.IsNullOrEmpty(dataType.Data.Value.ToString()) && dataType.ContentEditor.State == null)
            {
                dataType.ContentEditor.State = UrlPickerState.Deserialize((string)dataType.Data.Value);
            }
        }

        /// <summary>
        /// Method for customizing the way the <see cref="IDataType">datatype</see> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(UrlPickerDataType dataType)
        {
            UrlPickerState state = null;

            var displayValue = string.Empty;

            // Deserialize stored value
            if (dataType.Data.Value != null && !string.IsNullOrEmpty(dataType.Data.Value.ToString()) && dataType.ContentEditor.State == null)
            {
                state = UrlPickerState.Deserialize((string)dataType.Data.Value);
            }

            // Generate display value
            if (state != null)
            {
                if (state.Mode == UrlPickerMode.Content && state.NodeId != null)
                {
                    var node = uQuery.GetNode(state.NodeId.Value);

                    displayValue = string.Format(
                        "<span>{0}: </span><span>{1}</span><br/><span>{2}: </span><a href='{3}' title='{4}'>{5}</a>",
                        Helper.Dictionary.GetDictionaryItem("OpenInNewWindow", "Open in new window"),
                        state.NewWindow,
                        Helper.Dictionary.GetDictionaryItem("Node", "Node"),
                        node.NiceUrl,
                        Helper.Dictionary.GetDictionaryItem("OpenContent", "Open content"),
                        node.Name);

                    if (!string.IsNullOrEmpty(state.Title))
                    {
                        displayValue += string.Format(
                        "<br/><span>{0}: </span><span>{1}</span>",
                        Helper.Dictionary.GetDictionaryItem("Title", "Title"),
                        state.Title);
                    }
                }
                else if (state.Mode == UrlPickerMode.URL)
                {
                    displayValue = string.Format(
                        "<span>{0}: </span><span>{1}</span><br/><span>{2}: </span><a href='{3}' title='{4}' target='_blank'>{5}</a>",
                        Helper.Dictionary.GetDictionaryItem("OpenInNewWindow", "Open in new window"),
                        state.NewWindow,
                        Helper.Dictionary.GetDictionaryItem("URL", "URL"),
                        state.Url,
                        Helper.Dictionary.GetDictionaryItem("OpenUrl", "Open URL"),
                        state.Url);

                    if (!string.IsNullOrEmpty(state.Title))
                    {
                        displayValue = string.Format(
                        "<br/><span>{0}: </span><span>{1}</span>",
                        Helper.Dictionary.GetDictionaryItem("Title", "Title"),
                        state.Title);
                    }
                }
                else if (state.Mode == UrlPickerMode.Media && state.NodeId != null)
                {
                    var media = uQuery.GetMedia(state.NodeId.Value);

                    displayValue = string.Format(
                        "<span>{0}: </span><span>{1}</span><br/><span>{2}: </span><a href='{3}' title='{4}'>{5}</a>",
                        Helper.Dictionary.GetDictionaryItem("OpenInNewWindow", "Open in new window"),
                        state.NewWindow,
                        Helper.Dictionary.GetDictionaryItem("Node", "Node"),
                        media.getProperty("umbracoFile"),
                        Helper.Dictionary.GetDictionaryItem("OpenMedia", "Open media"),
                        media.Text);

                    if (!string.IsNullOrEmpty(state.Title))
                    {
                        displayValue += string.Format(
                        "<br/><span>{0}: </span><span>{1}</span>",
                        Helper.Dictionary.GetDictionaryItem("Title", "Title"),
                        state.Title);
                    }
                }
            }

            return displayValue;
        }
    }
}