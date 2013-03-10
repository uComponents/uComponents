namespace uComponents.DataTypes.DataTypeGrid.Factories.DataTypes
{
    using System.IO;
    using System.Web;

    using uComponents.DataTypes.DataTypeGrid.Model;
    using uComponents.DataTypes.FilePicker;

    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="FP_DataType"/> datatype.
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class FilePickerDataTypeFactory : BaseDataTypeFactory<FP_DataType>
    {
        /// <summary>
        /// Method for customizing the way the <see cref="IDataType">datatype</see> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(FP_DataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            var path = HttpContext.Current.Server.MapPath(value);

            if (!string.IsNullOrEmpty(value) && File.Exists(path))
            {
                return string.Format("<a target='_blank' href='{0}'>{0}</a>", value);
            }

            return string.Empty;
        }

        /// <summary>
        /// Method for getting the backing object for the specified <see cref="IDataType">datatype</see>.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The backing object.</returns>
        /// <remarks>Called when the method <see cref="GridCell.GetObject{T}()" /> method is called on a <see cref="GridCell" />.</remarks>
        public override object GetObject(FP_DataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            try
            {
                var path = HttpContext.Current.Server.MapPath(value);

                if (File.Exists(path))
                {
                    return File.OpenRead(path);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}