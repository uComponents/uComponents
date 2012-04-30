using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.UI.WebControls;

namespace uComponents.Core.Shared.Extensions
{
    /// <summary>
    /// Extension methods for DropDownList
    /// </summary>
    public static class DropDownListExtensions
    {
        /// <summary>
        /// Sets the selected index of the drop down list to that of the list item with the supplied value
        /// </summary>
        /// <param name="dropDownList">The drop down list.</param>
        /// <param name="value">The value.</param>
        public static void SetSelectedValue(this DropDownList dropDownList, string value)
        {
            ListItem listItem = dropDownList.Items.FindByValue(value);
            if (listItem != null)
            {
                dropDownList.SelectedIndex = dropDownList.Items.IndexOf(listItem);
            }
        }
    }
}
