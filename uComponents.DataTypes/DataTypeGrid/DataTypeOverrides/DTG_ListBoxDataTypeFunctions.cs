// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.DataTypeOverrides
{
    using System.Linq;
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Interfaces;

    using umbraco.editorControls.listbox;
    using umbraco;

    /// <summary>
    /// DTG extensions for the Dropdown List Multiple DataType
    /// </summary>
    internal class ListBoxDataTypeFactory : IDataTypeFactory<ListBoxDataType>
    {
        #region Implementation of IDataTypeFactory<ListBoxDataType>

        /// <summary>
        /// Converts the datatype value to a DTG compatible string
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <returns>A human-readable string</returns>
        public string ToDtgString(ListBoxDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            var p = uQuery.GetPreValues(dataType.DataTypeDefinitionId);

            var v = p.Where(x => value.Split(',').Contains(x.Id.ToString())).Select(x => x.Value);

            return string.Join(", ", v.ToArray());
        }

        /// <summary>
        /// Configures the datatype to be compatible with DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <param name="container">The container.</param>
        public void Configure(ListBoxDataType dataType, Control container)
        {
        }

        /// <summary>
        /// Saves the datatype for DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        public void SaveForDtg(ListBoxDataType dataType)
        {
        }

        #endregion
    }
}
