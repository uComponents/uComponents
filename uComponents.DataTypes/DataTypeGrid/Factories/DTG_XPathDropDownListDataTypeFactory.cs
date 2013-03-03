namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System;
    using System.Reflection;
    using System.Web.UI.WebControls;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco;
    using umbraco.cms.businesslogic.member;
    using umbraco.editorControls.XPathDropDownList;
    using umbraco.interfaces;
    using umbraco.NodeFactory;

    using Umbraco.Web;

    /// <summary>
    /// Factory for the <see cref="XPathDropDownListDataType"/> datatype.
    /// NOTE: Only compatible with Umbraco 6.0.2 and onwards
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class XPathDropDownListDataTypeFactory : BaseDataTypeFactory<XPathDropDownListDataType>
    {
        /// <summary>
        /// Method for performing special actions <b>before</b> creating the <see cref="IDataType" /> editor.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called <b>before</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        public override void Initialize(XPathDropDownListDataType dataType, DataTypeLoadEventArgs eventArgs)
        {
            var editor = dataType.DataEditor.Editor as XPathDropDownListDataEditor;

            if (editor != null)
            {
                // Set selected value
                if (dataType.Data.Value != null)
                {
                    var dropdown = editor.Controls[1] as DropDownList;

                    if (dropdown != null)
                    {
                        // Ensure stored values are set
                        dropdown.Load += (sender, args) =>
                        {
                            if (dropdown.SelectedValue == "-1")
                            {
                                // Get selected items from Node Name or Node Id
                                var dropDownListItem = dropdown.Items.FindByValue(dataType.Data.Value.ToString());

                                if (dropDownListItem != null)
                                {
                                    // Reset selected item
                                    dropdown.SelectedItem.Selected = false;

                                    // Set new selected item
                                    dropDownListItem.Selected = true;
                                }
                            }
                        };
                    }
                }
            }

            // Set default value to prevent YSOD
            if (dataType.Data.Value == null)
            {
                dataType.Data.Value = string.Empty;
            }
        }

        /// <summary>
        /// Method for customizing the way the <see cref="IDataType">datatype</see> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(XPathDropDownListDataType dataType)
        {
            if (dataType.Data.Value != null && !string.IsNullOrEmpty(dataType.Data.Value.ToString())) 
            {
                int id;
                if (int.TryParse(dataType.Data.Value.ToString(), out id) && id > 0)
                {
                    var document = new Node(id);

                    return string.Format("<a href='editContent.aspx?id={0}' title='Edit content'>{1}</a>", document.Id, document.Name);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Saves the specified data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="eventArgs">The <see cref="DataTypeSaveEventArgs"/> instance containing the event data.</param>
        public override void Save(XPathDropDownListDataType dataType, DataTypeSaveEventArgs eventArgs)
        {
            var editor = dataType.DataEditor.Editor as XPathDropDownListDataEditor;

            if (editor != null)
            {
                var dropdown = editor.Controls[1] as DropDownList;

                if (dropdown != null)
                {
                    dataType.Data.Value = dropdown.SelectedValue;
                }
            }
        }
    }
}