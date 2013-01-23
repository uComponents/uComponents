// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.DataTypeOverrides
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Interfaces;

    using umbraco.editorControls.datepicker;

    /// <summary>
    /// DTG extensions for the DateTime DataType
    /// </summary>
    internal class DateDataTypeFunctions : IDataTypeFunctions<DateDataType>
    {
        #region Implementation of IDtgFunctions<PagePickerDataType>

        /// <summary>
        /// Converts the datatype value to a DTG compatible string
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <returns></returns>
        public string ToDtgString(DateDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            DateTime d;

            if (HttpContext.Current.Request.UserLanguages != null && DateTime.TryParse(value, out d))
            {
                return d.ToString(CultureInfo.CreateSpecificCulture(HttpContext.Current.Request.UserLanguages[0]));
            }

            return value;
        }

        /// <summary>
        /// Configures the datatype to be compatible with DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <param name="container">The container.</param>
        public void ConfigureForDtg(DateDataType dataType, Control container)
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

        /// <summary>
        /// Saves for DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        public void SaveForDtg(DateDataType dataType)
        {
        }

        #endregion
    }
}
