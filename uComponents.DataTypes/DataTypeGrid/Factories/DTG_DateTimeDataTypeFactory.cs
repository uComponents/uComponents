using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Globalization;
    using System.Web;
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.editorControls.datefieldmultiple;

    /// <summary>
    /// Factory for the <see cref="DateTimeDataTypeFactory"/>
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class DateTimeDataTypeFactory : BaseDataTypeFactory<DataTypeDatefieldMultiple>
    {
        /// <summary>
        /// Method for customizing the way the <paramref name="dataType" /> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <paramref name="dataType" />.</remarks>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(DataTypeDatefieldMultiple dataType)
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
        /// Method for performing special actions while creating the <paramref name="dataType" /> editor.
        /// </summary>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <param name="container">The editor control container.</param>
        /// <remarks>Called when the grid creates the editor controls for the specified <paramref name="dataType" />.</remarks>
        public override void Configure(DataTypeDatefieldMultiple dataType, Control container)
        {
            var e = dataType.DataEditor as umbraco.editorControls.dateField;

            if (e != null)
            {
                e.ShowTime = true;

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
