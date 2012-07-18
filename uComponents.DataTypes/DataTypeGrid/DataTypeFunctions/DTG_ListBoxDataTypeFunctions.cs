// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.Core.DataTypes.DataTypeGrid.DataTypeFunctions
{
    using System.Linq;
    using System.Web.UI;

    using uComponents.Core.DataTypes.DataTypeGrid.Interfaces;

    using umbraco.editorControls.listbox;

    /// <summary>
    /// DTG extensions for the Dropdown List Multiple DataType
    /// </summary>
    internal class ListBoxDataTypeFunctions : IDataTypeFunctions<ListBoxDataType>
    {
        #region Implementation of IDataTypeFunctions<ListBoxDataType>

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
        public void ConfigureForDtg(ListBoxDataType dataType, Control container)
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
