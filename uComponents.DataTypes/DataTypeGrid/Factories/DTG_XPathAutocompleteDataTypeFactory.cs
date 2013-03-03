namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System;
    using System.Linq;
    using System.Xml;

    using uComponents.DataTypes.DataTypeGrid.Constants;
    using uComponents.DataTypes.DataTypeGrid.Model;
    using uComponents.DataTypes.XPathAutoComplete;

    using umbraco;
    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="XPathAutoCompleteDataType"/> class.
    /// </summary>
    public class XPathAutocompleteDataTypeFactory : BaseDataTypeFactory<XPathAutoCompleteDataType>
    {
        /// <summary>
        /// Method for performing special actions <b>before</b> creating the <see cref="IDataType" /> editor.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called <b>before</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        public override void Initialize(XPathAutoCompleteDataType dataType, DataTypeLoadEventArgs eventArgs)
        {
            var editor = dataType.DataEditor as XPathAutoCompleteDataEditor;

            if (editor != null && dataType.Data.Value != null)
            {
                editor.SelectedItemsHiddenField.Value = dataType.Data.Value.ToString();
            }
        }

        public override void Configure(XPathAutoCompleteDataType dataType, DataTypeLoadEventArgs eventArgs)
        {
            base.Configure(dataType, eventArgs);
        }

        /// <summary>
        /// Method for executing special actions before saving the editor value to the database.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeSaveEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called when the grid is saved for the specified <see cref="IDataType">datatype</see>.</remarks>
        public override void Save(XPathAutoCompleteDataType dataType, DataTypeSaveEventArgs eventArgs)
        {
            if (eventArgs.Action == DataTypeAction.Update)
            {
                
            }

            base.Save(dataType, eventArgs);
        }

        /// <summary>
        /// Method for customizing the way the <see cref="IDataType">datatype</see> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(XPathAutoCompleteDataType dataType)
        {
            if (dataType.Data.Value != null)
            {
                var doc = new XmlDocument();
                doc.LoadXml(dataType.Data.Value.ToString());

                if (doc.DocumentElement != null)
                {
                    var displayValue = string.Empty;

                    var objectType = uQuery.GetUmbracoObjectType(new Guid(doc.DocumentElement.GetAttribute("Type")));
                    var items = doc.SelectNodes("//Item");

                    if (objectType == uQuery.UmbracoObjectType.Document)
                    {
                        foreach (XmlNode item in items)
                        {
                            var textAttribute = item.Attributes["Text"];
                            var valueAttribute = item.Attributes["Value"];

                            displayValue += string.Format("<a href='editContent.aspx?id={0}' title='Edit content'>{1}</a>", valueAttribute.Value, textAttribute.Value);

                            if (item != items[items.Count - 1])
                            {
                                displayValue += ", ";
                            }
                        }
                    }

                    if (objectType == uQuery.UmbracoObjectType.Media)
                    {
                        foreach (XmlNode item in items)
                        {
                            var textAttribute = item.Attributes["Text"];
                            var valueAttribute = item.Attributes["Value"];

                            displayValue += string.Format("<a href='editMedia.aspx?id={0}' title='Edit media'>{1}</a>", valueAttribute.Value, textAttribute.Value);

                            if (item != items[items.Count - 1])
                            {
                                displayValue += ", ";
                            }
                        }
                    }

                    if (objectType == uQuery.UmbracoObjectType.Member)
                    {
                        foreach (XmlNode item in items)
                        {
                            var textAttribute = item.Attributes["Text"];
                            var valueAttribute = item.Attributes["Value"];

                            displayValue += string.Format("<a href='editMedia.aspx?id={0}' title='Edit member'>{1}</a>", valueAttribute.Value, textAttribute.Value);

                            if (item != items[items.Count - 1])
                            {
                                displayValue += ", ";
                            }
                        }
                    }

                    return displayValue;
                }
            }

            return base.GetDisplayValue(dataType);
        }
    }
}