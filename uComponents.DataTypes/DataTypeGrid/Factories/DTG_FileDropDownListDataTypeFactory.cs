namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.IO;
    using System.Web;

    using uComponents.DataTypes.DataTypeGrid.Model;
    using uComponents.DataTypes.FileDropDownList;

    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="FileDropDownListDataType"/> datatype.
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class FileDropDownListDataTypeFactory : BaseDataTypeFactory<FileDropDownListDataType>
    {
        /// <summary>
        /// Method for customizing the way the <see cref="IDataType">datatype</see> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(FileDropDownListDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            var prevalueEditor = dataType.PrevalueEditor as FileDropDownListPrevalueEditor;

            if (prevalueEditor != null)
            {
                var options = prevalueEditor.GetPreValueOptions<FileDropDownListOptions>();

                var path = HttpContext.Current.Server.MapPath(options.Directory + "/" + value);

                if (!string.IsNullOrEmpty(value) && File.Exists(path))
                {
                    return string.Format("<a target='_blank' href='{0}'>{0}</a>", value);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Method for getting the backing object for the specified <see cref="IDataType">datatype</see>.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The backing object.</returns>
        /// <remarks>Called when the method <see cref="GridCell.GetObject{T}()" /> method is called on a <see cref="GridCell" />.</remarks>
        public override object GetObject(FileDropDownListDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            var prevalueEditor = dataType.PrevalueEditor as FileDropDownListPrevalueEditor;

            if (prevalueEditor != null)
            {
                var options = prevalueEditor.GetPreValueOptions<FileDropDownListOptions>();

                var path = HttpContext.Current.Server.MapPath(options.Directory + "/" + value);

                if (File.Exists(path))
                {
                    return File.OpenRead(path);
                }
            }

            return null;
        }
    }
}