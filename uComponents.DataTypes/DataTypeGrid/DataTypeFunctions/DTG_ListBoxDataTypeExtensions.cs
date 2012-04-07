// -----------------------------------------------------------------------
// <copyright file="ListBoxDataTypeExtensions.cs" company="">
// TODO: [OA] Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.DataTypeFunctions
{
    using System.Linq;

    using umbraco.editorControls.listbox;

    /// <summary>
    /// DTG extensions for the Dropdown List Multiple DataType
    /// </summary>
    internal class ListBoxDataTypeFunctions
    {
        /// <summary>
        /// Converts the datatype value to a DTG compatible string
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <returns></returns>
        public static string ToDtgString(ListBoxDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            var p = uQuery.GetPreValues(dataType.DataTypeDefinitionId);

            var v = p.Where(x => value.Split(',').Contains(x.Id.ToString())).Select(x => x.Value);

            return string.Join(", ", v.ToArray());
        }
    }
}
