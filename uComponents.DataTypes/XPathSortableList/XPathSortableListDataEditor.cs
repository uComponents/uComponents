using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using uComponents.Core;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.property;
using umbraco.editorControls;
using umbraco.interfaces;
using umbraco.NodeFactory;

[assembly: WebResource("uComponents.DataTypes.XPathSortableList.XPathSortableList.css", Constants.MediaTypeNames.Text.Css)]
[assembly: WebResource("uComponents.DataTypes.XPathSortableList.XPathSortableList.js", Constants.MediaTypeNames.Application.JavaScript)]
namespace uComponents.DataTypes.XPathSortableList
{
    /// <summary>
    /// DataEditor for the XPath AutoComplete data-type.
    /// </summary>
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Javascript, "ui/jqueryui.js", "UmbracoClient")]
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Css, "ui/ui-lightness/jquery-ui.custom.css", "UmbracoClient")]
    public class XPathSortableListDataEditor : CompositeControl, IDataEditor
    {
        /// <summary>
        /// Field for the data.
        /// </summary>
        private IData data;

        /// <summary>
        /// local for the SourceData property
        /// </summary>
        private Dictionary<int, string> sourceData = null;

        /// <summary>
        /// Field for the options.
        /// </summary>
        private XPathSortableListOptions options;

        /// <summary>
        /// Wrappiing div
        /// </summary>
        private HtmlGenericControl div = new HtmlGenericControl("div");

        /// <summary>
        /// ul containing the source data in li attributes
        /// </summary>
        private HtmlGenericControl sourceListUl = new HtmlGenericControl("ul");

        /// <summary>
        /// Stores the selected values
        /// </summary>
        private HiddenField selectedItemsHiddenField = new HiddenField();

        /// <summary>
        /// Ensure number of items selected is within any min and max configuration settings
        /// </summary>
        private CustomValidator customValidator = new CustomValidator();

        /// <summary>
        /// Initializes a new instance of XPathSortableListDataEditor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="options"></param>
        internal XPathSortableListDataEditor(IData data, XPathSortableListOptions options)
        {
            this.data = data;
            this.options = options;
        }

        /// <summary>
        /// Gets the property / datatypedefinition id - used to identify the current instance
        /// </summary>
        private int DataTypeDefinitionId
        {
            get
            {
                return ((XmlData)this.data).DataTypeDefinitionId;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [treat as rich text editor].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [treat as rich text editor]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool TreatAsRichTextEditor
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [show label].
        /// </summary>
        /// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
        public virtual bool ShowLabel
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the editor.
        /// </summary>
        /// <value>The editor.</value>
        public Control Editor
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets the data using the xpath configured in the datatype setup
        /// </summary>
        private Dictionary<int, string> SourceData
        {
            get
            {
                if (this.sourceData == null)
                {
                    switch (this.options.UmbracoObjectType)
                    {
                        case uQuery.UmbracoObjectType.Document:

                            sourceData = uQuery.GetNodesByXPath(this.options.XPath).Where(x => x.Id != -1).ToNameIds();
                            break;

                        case uQuery.UmbracoObjectType.Media:

                            sourceData = uQuery.GetMediaByXPath(this.options.XPath).Where(x => x.Id != -1).ToNameIds();
                            break;

                        case uQuery.UmbracoObjectType.Member:

                            sourceData = uQuery.GetMembersByXPath(this.options.XPath).ToNameIds();
                            break;
                    }
                }

                return this.sourceData;
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            this.div.Attributes.Add("class", "xpath-sortable-list");
            this.div.Attributes.Add("data-min-items", this.options.MinItems.ToString());
            this.div.Attributes.Add("data-max-items", this.options.MaxItems.ToString());
            this.div.Attributes.Add("data-allow-duplicates", this.options.AllowDuplicates.ToString());

            this.sourceListUl.Attributes.Add("class", "source-list");
            this.div.Controls.Add(this.sourceListUl);

            HtmlGenericControl sortableListUl = new HtmlGenericControl("ul");
            sortableListUl.Attributes.Add("class", "sortable-list");
            this.div.Controls.Add(sortableListUl);

            this.div.Controls.Add(this.selectedItemsHiddenField);
           
            this.Controls.Add(this.div);
            this.Controls.Add(this.customValidator);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.EnsureChildControls();

            if (!this.Page.IsPostBack)
            {
                this.selectedItemsHiddenField.Value = this.data.Value.ToString();
            }

            this.PopulateSourceList();

            this.RegisterEmbeddedClientResource("uComponents.DataTypes.XPathSortableList.XPathSortableList.css", ClientDependencyType.Css);
            this.RegisterEmbeddedClientResource("uComponents.DataTypes.XPathSortableList.XPathSortableList.js", ClientDependencyType.Javascript);

            string startupScript = @"
                <script language='javascript' type='text/javascript'>
                    $(document).ready(function () {
                        XPathSortableList.init(jQuery('div#" + this.div.ClientID + @"'));
                    });
                </script>";

            ScriptManager.RegisterStartupScript(this, typeof(XPathSortableListDataEditor), this.ClientID + "_init", startupScript, false);
        }

        /// <summary>
        /// Called by Umbraco when saving the node
        /// </summary>
        public void Save()
        {
            var xml = this.selectedItemsHiddenField.Value;
            //var items = 0; // to validate on the number of items selected

            //// There should be a valid xml fragment (or empty) in the hidden field
            //if (!string.IsNullOrWhiteSpace(xml))
            //{
            //    var xmlDocument = new XmlDocument();
            //    xmlDocument.LoadXml(xml);

            //    items = xmlDocument.SelectNodes("//Item").Count;
            //}

            //if (this.options.MinItems > 0 && items < this.options.MinItems)
            //{
            //    customValidator.IsValid = false;
            //}

            //// fail safe check as UI shouldn't allow excess items to be selected (datatype configuration could have been changed since data was stored)
            //if (this.options.MaxItems > 0 && items > this.options.MaxItems)
            //{
            //    customValidator.IsValid = false;
            //}

            //if (!customValidator.IsValid)
            //{
            //    var property = new Property(((XmlData)this.data).PropertyId);
            //    // property.PropertyType.Mandatory - IGNORE, always use the configuration parameters

            //    this.customValidator.ErrorMessage = ui.Text("errorHandling", "errorRegExpWithoutTab", property.PropertyType.Name, User.GetCurrent());
            //}

            this.data.Value = xml;
        }

        /// <summary>
        /// using the data in the hidden field, the source list itesm are generated (this markup is the also the data source for the js)
        /// </summary>
        private void PopulateSourceList()
        {
            foreach (KeyValuePair<int, string> dataItem in this.SourceData)
            {
                HtmlGenericControl li = new HtmlGenericControl("li");

                if (!(!this.options.AllowDuplicates && this.GetSelectedValues().Contains(dataItem.Key)))
                {
                    li.Attributes.Add("class", "active");
                }

                li.Attributes.Add("data-text", dataItem.Value);
                li.Attributes.Add("data-value", dataItem.Key.ToString());

                HtmlGenericControl a = new HtmlGenericControl("a");
                a.Attributes.Add("class", "add");
                a.Attributes.Add("title", "add");
                a.Attributes.Add("href", "javascript:void(0);");
                a.Attributes.Add("onclick", "XPathSortableList.addItem(this);");
                a.InnerText = dataItem.Value;

                li.Controls.Add(a);
                this.sourceListUl.Controls.Add(li);
            }
        }

        /// <summary>
        /// Gets ids for the selected items from the hidden field (in order)
        /// </summary>
        /// <returns></returns>
        private IEnumerable<int> GetSelectedValues()
        {
            //<XPathSortableList Type="c66ba18e-eaf3-4cff-8a22-41b16d66a972">
            //    <Item Value="1" />
            //    <Item Value="9" />
            //</XPathSortableList>

            if (!String.IsNullOrWhiteSpace(this.selectedItemsHiddenField.Value))
            {
                return XDocument.Parse(this.selectedItemsHiddenField.Value)
                                .Descendants("Item")
                                .Select(x => int.Parse(x.Attribute("Value").Value));
            }
            else
            {
                return new int[] { };
            }
        }
    }
}
