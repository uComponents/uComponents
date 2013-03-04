namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;
    using System.Xml.Linq;

    using uComponents.Core;
    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.editorControls.XPathCheckBoxList;
    using umbraco.interfaces;
    using umbraco.NodeFactory;

    /// <summary>
    /// Factory for the <see cref="XPathCheckBoxListDataType"/> datatype.
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class XpathCheckBoxListDataTypeFactory : BaseDataTypeFactory<XPathCheckBoxListDataType>
    {
        /// <summary>
        /// Method for performing special actions <b>before</b> creating the <see cref="IDataType" /> editor.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called <b>before</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        public override void Initialize(XPathCheckBoxListDataType dataType, DataTypeLoadEventArgs eventArgs)
        {
            var editor = dataType.DataEditor.Editor as XPathCheckBoxListDataEditor;

            if (editor != null && dataType.Data.Value != null)
            {
                var checkbox = editor.Controls[0] as CheckBoxList;

                if (checkbox != null) 
                {
                    // Ensure stored values are set
                    checkbox.Load += (sender, args) =>
                    {
                        if (checkbox.SelectedItem == null)
                        {
                            this.SetSelectedValues(checkbox, dataType.Data.Value.ToString());
                        }
                    };
                }
            }
        }

        /// <summary>
        /// Method for customizing the way the <see cref="IDataType">datatype</see> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(XPathCheckBoxListDataType dataType)
        {
            if (dataType.Data.Value != null)
            {
                var displayValue = string.Empty;

                var storedValues = this.GetValues(dataType.Data.Value.ToString());

                foreach (var value in storedValues)
                {
                    int id;
                    if (int.TryParse(value, out id) && id > 0)
                    {
                        var document = new Node(id);

                        displayValue += string.Format("<a href='editContent.aspx?id={0}' title='Edit content'>{1}</a>", document.Id, document.Name);

                        if (value != storedValues.Last())
                        {
                            displayValue += ", ";
                        }
                    }
                }

                return displayValue;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the values from the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A list of values.</returns>
        private List<string> GetValues(string data)
        {
            var values = new List<string>();

            if (Helper.Xml.CouldItBeXml(data))
            {
                // build selected values from XML fragment
                foreach (XElement nodeXElement in XElement.Parse(data).Elements())
                {
                    values.Add(nodeXElement.Value);
                }
            }
            else
            {
                // Assume a CSV source
                values = data.Split(Constants.Common.COMMA).ToList();
            }

            return values;
        }

        /// <summary>
        /// Sets the selected values.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="data">The data.</param>
        private void SetSelectedValues(ListControl list, string data)
        {
            var values = this.GetValues(data);

            // Find checkboxes where values match the stored values and set to selected
            foreach (var selectedValue in values)
            {
                var checkBoxListItem = list.Items.FindByValue(selectedValue);

                if (checkBoxListItem != null)
                {
                    checkBoxListItem.Selected = true;
                }
            }
        }
    }
}