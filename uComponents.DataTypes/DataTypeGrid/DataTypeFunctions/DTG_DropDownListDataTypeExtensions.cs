// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.DataTypeFunctions
{
    using System.Linq;
    using System.Web.UI;

    using uComponents.Core.DataTypes.DataTypeGrid.Interfaces;
    using umbraco.editorControls.dropdownlist;

    /// <summary>
    /// /// DTG extensions for the Dropdown List DataType
    /// </summary>
    internal class DropdownListDataTypeFunctions : IDataTypeFunctions<DropdownListDataType>
    {
        #region Implementation of IDataTypeFunctions<DropdownListDataType>

        /// <summary>
        /// Converts the datatype value to a DTG compatible string
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <returns>A human-readable string</returns>
        public string ToDtgString(DropdownListDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            var p = uQuery.GetPreValues(dataType.DataTypeDefinitionId);

            var v = p.SingleOrDefault(x => x.Id.ToString().Equals(value));

            if (v != null)
            {
                return v.Value;
            }

            return value;
        }

        /// <summary>
        /// Configures the datatype to be compatible with DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <param name="container">The container.</param>
        public void ConfigureForDtg(DropdownListDataType dataType, Control container)
        {
        }

        /// <summary>
        /// Saves the datatype for DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        public void SaveForDtg(DropdownListDataType dataType)
        {
        }

        #endregion
    }
}
