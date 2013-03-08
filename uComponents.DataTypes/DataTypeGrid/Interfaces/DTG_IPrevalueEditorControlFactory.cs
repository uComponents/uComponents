namespace uComponents.DataTypes.DataTypeGrid.Interfaces
{
    using System.Web.UI.WebControls;

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
    }
}