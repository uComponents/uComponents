namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Drawing;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.editorControls.colorpicker;
    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="ColorPickerDataType"/> datatype.
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class ColorPickerDataTypeFactory : BaseDataTypeFactory<ColorPickerDataType>
    {
        /// <summary>
        /// Method for customizing the way the <see cref="IDataType">datatype</see> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(ColorPickerDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                var color = (Color)new ColorConverter().ConvertFromString("#" + value);
                var contrastColor = this.GetContrastColor(color);

                return string.Format("<span style='display: inline-block; font-size: 0.9em; border: 1px solid {1}; line-height: 18px; padding: 0 3px; background-color: #{0}; color: {1};'>{2}</span>", value, ColorTranslator.ToHtml(contrastColor), color.Name);
            }

            return value;
        }

        /// <summary>
        /// Gets the contrast color for the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The contrast color.</returns>
        private Color GetContrastColor(Color color)
        {
            // Counting the perceptive luminance - human eye favors green color... 
            var a = 1 - (((0.299 * color.R) + (0.587 * color.G) + (0.114 * color.B)) / 255);

            // Get color rgb value
            var d = a < 0.5 ? 0 : 255;

            return Color.FromArgb(d, d, d);
        }
    }
}