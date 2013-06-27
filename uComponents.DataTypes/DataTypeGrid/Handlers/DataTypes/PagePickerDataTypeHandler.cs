namespace uComponents.DataTypes.DataTypeGrid.Handlers.DataTypes
{
    using System.Web.UI;

    using uComponents.Core;
    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.NodeFactory;
    using umbraco.editorControls.pagepicker;
    using umbraco.interfaces;

    /// <summary>a
    /// Factory for the <see cref="PagePickerDataTypeHandler"/>
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class PagePickerDataTypeHandler : BaseDataTypeHandler<PagePickerDataType>
    {
        /// <summary>
        /// Method for customizing the way the <paramref name="dataType" /> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <paramref name="dataType" />.</remarks>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(PagePickerDataType dataType)
        {
            if (dataType.Data.Value != null)
            {
                int id;

                if (int.TryParse(dataType.Data.Value.ToString(), out id))
                {
                    var node = new Node(id);

                    return string.Format("<a href='editContent.aspx?id={0}' title='Edit content'>{1}</a>", node.Id, node.Name);
                }

                return dataType.Data.Value.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Method for getting the backing object for the specified <paramref name="dataType" />.
        /// </summary>
        /// <remarks>Called when the method <see cref="GridCell.GetPropertyValue()"/> method is called on a <see cref="GridCell"/>.</remarks>
        /// <param name="dataType">The <paramref name="dataType" /> instance.</param>
        /// <returns>The backing object.</returns>
        public override object GetPropertyValue(PagePickerDataType dataType)
        {
            // Try to use registered property value converter first
            var converter = Helper.Resolvers.GetPropertyValueConverter(dataType);

            if (converter != null)
            {
                return converter.ConvertPropertyValue(dataType.Data.Value).Result;
            }

            // Fall back to custom value conversion
            if (dataType.Data.Value != null)
            {
                int id;

                if (int.TryParse(dataType.Data.Value.ToString(), out id))
                {
                    return new Node(id);
                }
            }

            return default(Node);
        }

        /// <summary>
        /// Method for getting the control to use when validating the specified <see cref="IDataType" />.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="editorControl">The <see cref="IDataType" /> editor control.</param>
        /// <returns>The control to validate.</returns>
        public override Control GetControlToValidate(PagePickerDataType dataType, Control editorControl)
        {
            var value = editorControl.Controls[0];

            return value;
        }
    }
}