namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using uComponents.DataTypes.IncrementalTextBox;

    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="IT_DataType"/> datatype.
    /// </summary>
    public class IncrementalTextBoxDataTypeFactory : BaseDataTypeFactory<IT_DataType>
    {
        /// <summary>
        /// Method for performing special actions <b>after</b> the <see cref="IDataType" /> <see cref="IDataEditor">editor</see> has been loaded.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="container">The editor control container.</param>
        /// <remarks>Called <b>after</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        public override void Configure(IT_DataType dataType, System.Web.UI.Control container)
        {
            container.Page.ClientScript.RegisterClientScriptInclude(container.Page.GetType(), "jquery.alphanumeric", "/umbraco_client/ui/jquery.alphanumeric.js");
        }
    }
}