// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 23.05.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.Core.DataTypes.DataTypeGrid.DataTypeFunctions
{
    using System.Web.UI;

    using uComponents.Core.DataTypes.DataTypeGrid.Interfaces;

    using umbraco.editorControls.pagepicker;
    using umbraco.NodeFactory;

    internal class PagePickerDataTypeFunctions : IDataTypeFunctions<PagePickerDataType>
    {
        #region Implementation of IDtgFunctions<PagePickerDataType>

        /// <summary>
        /// Converts the datatype value to a DTG compatible string
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>A human-readable string</returns>
        public string ToDtgString(PagePickerDataType dataType)
        {
            if (dataType.Data.Value != null)
            {
                int id;

                if (int.TryParse(dataType.Data.Value.ToString(), out id))
                {
                    var node = new Node(id);

                    return node.Name;
                }

                return dataType.Data.Value.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Configures the datatype to be compatible with DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <param name="container">The container.</param>
        public void ConfigureForDtg(PagePickerDataType dataType, Control container)
        {
        }

        /// <summary>
        /// Saves for DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        public void SaveForDtg(PagePickerDataType dataType)
        {
        }

        #endregion
    }
}