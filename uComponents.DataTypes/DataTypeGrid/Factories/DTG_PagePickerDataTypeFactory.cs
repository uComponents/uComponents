namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.editorControls.pagepicker;
    using umbraco.NodeFactory;

    /// <summary>a
    /// Factory for the <see cref="PagePickerDataTypeFactory"/>
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class PagePickerDataTypeFactory : BaseDataTypeFactory<PagePickerDataType>
    {
        /// <summary>
        /// Method for customizing the way the <typeparamref name="PagePickerDataType">datatype</typeparamref> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <typeparamref name="PagePickerDataType">datatype</typeparamref>.</remarks>
        /// <param name="dataType">The <typeparamref name="PagePickerDataType">datatype</typeparamref> instance.</param>
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
        /// Method for getting the backing object for the specified <typeparamref name="PagePickerDataType">datatype</typeparamref>.
        /// </summary>
        /// <remarks>Called when the method <see cref="GridCell.GetObject{T}()"/> method is called on a <see cref="GridCell"/>.</remarks>
        /// <param name="dataType">The <typeparamref name="PagePickerDataType">datatype</typeparamref> instance.</param>
        /// <returns>The backing object.</returns>
        public override object GetObject(PagePickerDataType dataType)
        {
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
    }
}