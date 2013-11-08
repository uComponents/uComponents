namespace uComponents.DataTypes.DataTypeGrid.Handlers.DataTypes
{
    using System.IO;
    using System.Web;

    using uComponents.Core;
    using uComponents.DataTypes.DataTypeGrid.Model;
    using uComponents.DataTypes.FilePicker;

    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="FP_DataType"/> datatype.
    /// </summary>
    [DataTypeHandler(Priority = -1)]
    public class FilePickerDataTypeHandler : BaseDataTypeHandler<FP_DataType>
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
        /// <remarks>Called when the method <see cref="GridCell.GetPropertyValue()" /> method is called on a <see cref="GridCell" />.</remarks>
        public override object GetPropertyValue(FP_DataType dataType)
        {
            // Try to use registered property value converter first
            var converter = Helper.Resolvers.GetPropertyValueConverter(dataType);

            if (converter != null)
            {
                return converter.ConvertPropertyValue(dataType.Data.Value).Result;
            }

            // Fall back to custom value conversion
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