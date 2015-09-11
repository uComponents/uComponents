namespace uComponents.DataTypes.DataTypeGrid.Interfaces
{
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using uComponents.DataTypes.DataTypeGrid.Model;

    /// <summary>
    /// Interface for the prevalue editor control factory
    /// </summary>
    public interface IPrevalueEditorControlFactory
    {
        /// <summary>
        /// Builds the data type drop down list.
        /// </summary>
        /// <returns>A DropDownList.</returns>
        DropDownList BuildDataTypeDropDownList();

        /// <summary>
        /// Builds the content priority dropdown list.
        /// </summary>
        /// <param name="configurations">The column configurations.</param>
        /// <param name="currentSortPriority">The current sort priority.</param>
        /// <returns>> DropDownList.</returns>
        DropDownList BuildContentPriorityDropdownList(IList<PreValueRow> configurations, string currentSortPriority);
    }
}