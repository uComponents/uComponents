using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.editorControls;
using umbraco;
using System.Configuration;
using uComponents.DataTypes.Shared.Extensions;

namespace uComponents.DataTypes.XPathAutoComplete
{
    public class XPathAutoCompletePreValueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
    {
        /// <summary>
        /// Radio buttons to select type of node to pick from: Content / Media / Members
        /// </summary>
        private RadioButtonList typeRadioButtonList = new RadioButtonList();

        /// <summary>
        /// TextBox control to get the XPath expression
        /// </summary>
        private TextBox xPathTextBox = new TextBox();

        /// <summary>
        /// Property to use as a text value for the autocomplete to search against - if empty then .Name of (node / media / member ) is used
        /// </summary>
        private TextBox propertyTextBox = new TextBox();

        /// <summary>
        /// RequiredFieldValidator to ensure an XPath expression has been entered
        /// </summary>
        private RequiredFieldValidator xPathRequiredFieldValidator = new RequiredFieldValidator();

        /// <summary>
        /// Server side validation of XPath expression
        /// </summary>
        private CustomValidator xPathCustomValidator = new CustomValidator();

        /// <summary>
        /// Number of characters before data is requested (useful if the list size should visibily shrink as the data set narrows - else use a SELECT TOP x FROM .... clause)
        /// valid range 0 to 5 (where 0 if off - so sending all the data on initial load)
        /// </summary>
        private DropDownList minLengthDropDownList = new DropDownList();

        /// <summary>
        /// Min number of items that must be selected - defaults to 0
        /// </summary>
        private TextBox minItemsTextBox = new TextBox();

        /// <summary>
        /// Max number of items that can be selected - defaults to 0 (anything that's not a +ve integer imposes no limit)
        /// </summary>
        private TextBox maxItemsTextBox = new TextBox();

        /// <summary>
        /// Data object used to define the configuration status of this PreValueEditor
        /// </summary>
        private XPathAutoCompleteOptions options = null;

        /// <summary>
        /// Initialize a new instance of XPathAutoCompletePreValueEditor
        /// </summary>
        /// <param name="dataType">XPathAutoCompleteDataType</param>
        public XPathAutoCompletePreValueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
            : base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
        {
        }

        /// <summary>
        /// Gets the options data object that represents the current state of this datatypes configuration
        /// </summary>
        internal XPathAutoCompleteOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    // Deserialize any stored settings for this PreValueEditor instance
                    this.options = this.GetPreValueOptions<XPathAutoCompleteOptions>();

                    // If still null, ie, object couldn't be de-serialized from PreValue[0] string value
                    if (this.options == null)
                    {
                        // Create a new Options data object with the default values
                        this.options = new XPathAutoCompleteOptions(true);
                    }
                }

                return this.options;
            }
        }





        /// <summary>
        /// Creates all of the controls and assigns all of their properties
        /// </summary>
        protected override void CreateChildControls()
        {
            //radio buttons to select type of nodes that can be picked (Document, Media or Member)
            this.typeRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Document.GetFriendlyName(), uQuery.UmbracoObjectType.Document.GetGuid().ToString()));
            this.typeRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Media.GetFriendlyName(), uQuery.UmbracoObjectType.Media.GetGuid().ToString()));
            this.typeRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Member.GetFriendlyName(), uQuery.UmbracoObjectType.Member.GetGuid().ToString()));

            this.xPathTextBox.ID = "xPathTextBox";
            this.xPathTextBox.CssClass = "umbEditorTextField";

            this.xPathRequiredFieldValidator.ControlToValidate = this.xPathTextBox.ID;
            this.xPathRequiredFieldValidator.Display = ValidatorDisplay.Dynamic;
            this.xPathRequiredFieldValidator.ErrorMessage = " XPath expression required";

            this.xPathCustomValidator.ControlToValidate = this.xPathTextBox.ID;
            this.xPathCustomValidator.Display = ValidatorDisplay.Dynamic;
            this.xPathCustomValidator.ServerValidate += new ServerValidateEventHandler(this.XPathCustomValidator_ServerValidate);

            this.propertyTextBox.ID = "propertyTextBox";
            this.propertyTextBox.CssClass = "umbEditorTextField";

            this.minLengthDropDownList.ID = "minLengthDropDownList";
            this.minLengthDropDownList.Items.Add(new ListItem("1", "1"));
            this.minLengthDropDownList.Items.Add(new ListItem("2", "2"));
            this.minLengthDropDownList.Items.Add(new ListItem("3 (default)", "3"));
            this.minLengthDropDownList.Items.Add(new ListItem("4", "4"));
            this.minLengthDropDownList.Items.Add(new ListItem("5", "5"));
            //this.minLengthDropDownList.Items.Add(new ListItem("First Space", "first-space")); // TODO: potential feature ?

            this.minItemsTextBox.ID = "minSelectionItemsTextBox";
            this.minItemsTextBox.Width = 30;
            this.minItemsTextBox.MaxLength = 2;
            this.minItemsTextBox.AutoCompleteType = AutoCompleteType.None;

            this.maxItemsTextBox.ID = "maxSelectionItemsTextBox";
            this.maxItemsTextBox.Width = 30;
            this.maxItemsTextBox.MaxLength = 2;
            this.maxItemsTextBox.AutoCompleteType = AutoCompleteType.None;

            this.Controls.AddPrevalueControls(
                this.xPathTextBox,
                this.xPathRequiredFieldValidator,
                this.xPathCustomValidator,
                this.minLengthDropDownList,
                this.minItemsTextBox,
                this.maxItemsTextBox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.Page.IsPostBack)
            {
                this.xPathTextBox.Text = this.Options.XPath;

                
                this.minLengthDropDownList.SetSelectedValue(this.Options.MinLength.ToString());
                this.minItemsTextBox.Text = this.Options.MinItems.ToString();
                this.maxItemsTextBox.Text = this.Options.MaxItems.ToString();
            }
        }

        /// <summary>
        /// Will run the entered Sql expression to ensure it is valid
        /// </summary>
        /// <param name="source">xPathCustomValidator</param>
        /// <param name="args"></param>
        private void XPathCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string sql = args.Value;
            bool isValid = false;

            try
            {
                isValid = true;
            }
            catch
            {
                this.xPathCustomValidator.ErrorMessage = " Error in XPath expression";
            }

            args.IsValid = isValid;
        }

        /// <summary>
        /// Saves the pre value data to Umbraco
        /// </summary>
        public override void Save()
        {
            if (this.Page.IsValid)
            {
                this.Options.XPath = this.xPathTextBox.Text;
                this.Options.Property = this.propertyTextBox.Text;
                this.Options.Type = this.typeRadioButtonList.SelectedValue;
                
                this.Options.MinLength = int.Parse(this.minLengthDropDownList.SelectedValue);

                // ensure min and max items are valid numbers
                int minItems;
                int.TryParse(this.minItemsTextBox.Text, out minItems);
                this.Options.MinItems = minItems;

                int maxItems;
                int.TryParse(this.maxItemsTextBox.Text, out maxItems);
                this.Options.MaxItems = maxItems;

                this.SaveAsJson(this.Options);  // Serialize to Umbraco database field
            }
        }

        /// <summary>
        /// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("Type", @"", this.typeRadioButtonList);
            writer.AddPrevalueRow("XPath Expression", @"", this.xPathTextBox, this.xPathRequiredFieldValidator, this.xPathCustomValidator);
            writer.AddPrevalueRow("Property Name", @"value of property to query and use as item labels", this.propertyTextBox);
            writer.AddPrevalueRow("Min Length", "number of chars in the autocomplete text box before querying for data", this.minLengthDropDownList);
            writer.AddPrevalueRow("Min Items", "number of items that must be selected", this.minItemsTextBox);
            writer.AddPrevalueRow("Max Items", "number of items that can be selected - 0 means no limit", this.maxItemsTextBox);
        }
    }
}
