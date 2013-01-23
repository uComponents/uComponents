// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 23.05.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.DataTypeOverrides
{
    using System.Linq;
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Interfaces;

    using umbraco.editorControls.radiobuttonlist;
    using umbraco;

    /// <summary>
    /// Functions for RadionButtonList
    /// </summary>
    public class RadioButtonListDataTypeFunctions : IDataTypeFunctions<RadioButtonListDataType>
    {
        #region Implementation of IDataTypeFunctions<RadioButtonListDataType>

        /// <summary>
        /// Converts the datatype value to a DTG compatible string
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <returns>A human-readable string</returns>
        public string ToDtgString(RadioButtonListDataType dataType)
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
        public void ConfigureForDtg(RadioButtonListDataType dataType, Control container)
        {
        }

        /// <summary>
        /// Saves the datatype for DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        public void SaveForDtg(RadioButtonListDataType dataType)
        {
        }

        #endregion
    }
}