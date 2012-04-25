// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.DataTypeFunctions
{
    using umbraco.editorControls.tinyMCE3;

    /// <summary>
    /// DTG functions for the Richtext Editor DataType
    /// </summary>
    internal class TinyMCE3DataTypeFunctions
    {
        /// <summary>
        /// Saves for DTG.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        public static void SaveForDtg(tinyMCE3dataType dataType)
        {
            dataType.DataEditor.Save();
        }
    }
}
