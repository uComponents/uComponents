namespace uComponents.DataTypes.DataTypeGrid.Handlers.DataTypes
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Web;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.cms.businesslogic.packager;
    using umbraco.editorControls;
    using umbraco.editorControls.datepicker;
    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="DateDataTypeHandler"/>
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class DateDataTypeHandler : BaseDataTypeHandler<DateDataType>
    {
        /// <summary>
        /// Method for customizing the way the <paramref name="dataType" /> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <paramref name="dataType" />.</remarks>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(DateDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            DateTime d;

            if (HttpContext.Current.Request.UserLanguages != null && DateTime.TryParse(value, out d))
            {
                return d.ToString(CultureInfo.CreateSpecificCulture(HttpContext.Current.Request.UserLanguages[0]).DateTimeFormat.ShortDatePattern);
            }

            return value;
        }

        /// <summary>
        /// Saves the specified data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="eventArgs">The <see cref="DataTypeSaveEventArgs"/> instance containing the event data.</param>
        public override void Save(DateDataType dataType, DataTypeSaveEventArgs eventArgs)
        {
            // Persist value from page to dataType.Data
            base.Save(dataType, eventArgs);

            DateTime d;

            // Parse value and save data again using reflection to prevent value from being saved with wrong culture
            if (dataType.Data.Value != null && DateTime.TryParse(dataType.Data.Value.ToString(), out d))
            {
                var t = typeof(dateField).GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance);

                if (t != null)
                {
                    dataType.Data.Value = d.ToString("s");

                    t.SetValue(dataType.DataEditor, dataType.Data);
                }
            }
        }

        /// <summary>
        /// Method for performing special actions while creating the <paramref name="dataType" /> editor.
        /// </summary>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called when the grid creates the editor controls for the specified <paramref name="dataType" />.</remarks>
        public override void Configure(DateDataType dataType, DataTypeLoadEventArgs eventArgs)
        {
            var e = dataType.DataEditor as umbraco.editorControls.dateField;

            if (e != null)
            {
                // Datefield doesn't recreate ID's on load. Need to fix that
                e.Editor.Controls[0].ID = e.ID + "_ctl0";
                e.Editor.Controls[0].Controls[0].ID = e.ID + "_ctl0_ctl0";

                DateTime d;

                if (dataType.Data.Value != null && DateTime.TryParse(dataType.Data.Value.ToString(), out d))
                {
                    // Manually set data to stored value
                    e.DateTime = d;
                }
            }
        }
    }
}