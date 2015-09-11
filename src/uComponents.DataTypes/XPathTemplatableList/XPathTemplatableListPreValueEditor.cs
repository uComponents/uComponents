using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using umbraco;
using umbraco.editorControls;
using uComponents.DataTypes.Shared.macroRenderings;

namespace uComponents.DataTypes.XPathTemplatableList
{
    using System.ComponentModel;
    using System.Web.Hosting;

    using umbraco.cms.businesslogic.macro;

    /// <summary>
    /// Prevalue Editor for XPath Templatable List
    /// </summary>
    public class XPathTemplatableListPreValueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
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
        /// RequiredFieldValidator to ensure an XPath expression has been entered
        /// </summary>
        private RequiredFieldValidator xPathRequiredFieldValidator = new RequiredFieldValidator();

        /// <summary>
        /// Server side validation of XPath expression
        /// </summary>
        private CustomValidator xPathCustomValidator = new CustomValidator();

        /// <summary>
        /// Property to sort on
        /// </summary>
        private propertyTypePicker sortOnDropDown = new propertyTypePicker();

        /// <summary>
        /// to set the sort direction
        /// </summary>
        private RadioButtonList sortDirectionRadioButtonList = new RadioButtonList();

        /// <summary>
        /// To set an optional limit - 0 = no limit
        /// </summary>
        private TextBox limitToTextBox = new TextBox();

        /// <summary>
        /// 
        /// </summary>
        private RegularExpressionValidator limitToRegularExpressionValidator = new RegularExpressionValidator();

        /// <summary>
        /// Height of the source list control, in pixels, or 0 for no scrolling
        /// </summary>
        private TextBox listHeightTextBox = new TextBox();

        /// <summary>
        /// Must be a number
        /// </summary>
        private RegularExpressionValidator listHeightRegularExpressionValidator = new RegularExpressionValidator();

        /// <summary>
        /// 
        /// </summary>
        private DropDownList macroDropDownList = new DropDownList();

        /// <summary>
        /// for selection of a css file (or empty)
        /// </summary>
        private DropDownList cssFileDropDownList = new DropDownList();

        /// <summary>
        /// for selection of a script file (or empty)
        /// </summary>
        private DropDownList scriptFileDropDownList = new DropDownList();

        /// <summary>
        /// Min number of items that must be selected - defaults to 0
        /// </summary>
        private TextBox minItemsTextBox = new TextBox();

        /// <summary>
        /// 
        /// </summary>
        private RegularExpressionValidator minItemsRegularExpressionValidator = new RegularExpressionValidator();

        /// <summary>
        /// Custom validator to ensure that if relevant, then min items is less than or equal to max items
        /// </summary>
        private CustomValidator minItemsCustomValidator = new CustomValidator();

        /// <summary>
        /// Max number of items that can be selected - defaults to 0 (anything that's not a +ve integer imposes no limit)
        /// </summary>
        private TextBox maxItemsTextBox = new TextBox();

        /// <summary>
        /// 
        /// </summary>
        private RegularExpressionValidator maxItemsRegularExpressionValidator = new RegularExpressionValidator();

        /// <summary>
        /// If max items is set (ie not 0) then it must be greater than min items
        /// </summary>
        private CustomValidator maxItemsCustomValidator = new CustomValidator();

        /// <summary>
        /// if enabled then the same item can be seleted multiple times
        /// </summary>
        private CheckBox allowDuplicatesCheckBox = new CheckBox();

        /// <summary>
        /// Data object used to define the configuration status of this PreValueEditor
        /// </summary>
        private XPathTemplatableListOptions options = null;

        /// <summary>
        /// Initialize a new instance of XPathTemplatableListPreValueEditor
        /// </summary>
        /// <param name="dataType">XPathTemplatableListDataType</param>
        public XPathTemplatableListPreValueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
            : base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
        {
        }

        /// <summary>
        /// Gets the options data object that represents the current state of this datatypes configuration
        /// </summary>
        internal XPathTemplatableListOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    // Deserialize any stored settings for this PreValueEditor instance
                    this.options = this.GetPreValueOptions<XPathTemplatableListOptions>();

                    // If still null, ie, object couldn't be de-serialized from PreValue[0] string value
                    if (this.options == null)
                    {
                        // Create a new Options data object with the default values
                        this.options = new XPathTemplatableListOptions();
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
            this.typeRadioButtonList.ID = "typeRadioButtonList";

            this.xPathTextBox.ID = "xPathTextBox";
            this.xPathTextBox.CssClass = "umbEditorTextField";

            this.xPathRequiredFieldValidator.ControlToValidate = this.xPathTextBox.ID;
            this.xPathRequiredFieldValidator.Display = ValidatorDisplay.Dynamic;
            this.xPathRequiredFieldValidator.CssClass = "validator";
            this.xPathRequiredFieldValidator.ErrorMessage = " XPath expression required";

            this.xPathCustomValidator.ControlToValidate = this.xPathTextBox.ID;
            this.xPathCustomValidator.Display = ValidatorDisplay.Dynamic;
            this.xPathRequiredFieldValidator.CssClass = "validator";
            this.xPathCustomValidator.ServerValidate += this.XPathCustomValidator_ServerValidate;

            this.sortOnDropDown.ID = "sortOnDropDown";
            this.sortOnDropDown.AutoPostBack = true;
            this.sortOnDropDown.SelectedIndexChanged += this.SortOnDropDown_SelectedIndexChanged;

            this.sortDirectionRadioButtonList.ID = "sortDirectionRadioButtonlist";

            this.limitToTextBox.ID = "limitToTextBox";
            this.limitToTextBox.Width = 30;
            this.limitToTextBox.MaxLength = 2;
            this.limitToTextBox.AutoCompleteType = AutoCompleteType.None;

            this.limitToRegularExpressionValidator.ControlToValidate = this.limitToTextBox.ID;
            this.ConfigureNumberValidation(ref this.limitToRegularExpressionValidator);

            this.listHeightTextBox.ID = "listHeightTextBox";
            this.listHeightTextBox.Width = 30;
            this.listHeightTextBox.MaxLength = 4;

            this.listHeightRegularExpressionValidator.ControlToValidate = this.listHeightTextBox.ID;
            this.ConfigureNumberValidation(ref this.listHeightRegularExpressionValidator);

            this.macroDropDownList.ID = "macroDropDownList";
            this.cssFileDropDownList.ID = "cssDropDownList";
            this.scriptFileDropDownList.ID = "scriptDropDownList";

            this.minItemsTextBox.ID = "minSelectionItemsTextBox";
            this.minItemsTextBox.Width = 30;
            this.minItemsTextBox.MaxLength = 2;
            this.minItemsTextBox.AutoCompleteType = AutoCompleteType.None;

            this.minItemsRegularExpressionValidator.ControlToValidate = this.minItemsTextBox.ID;
            this.ConfigureNumberValidation(ref this.minItemsRegularExpressionValidator);

            this.minItemsCustomValidator.ControlToValidate = this.minItemsTextBox.ID;
            this.minItemsCustomValidator.Display = ValidatorDisplay.Dynamic;
            this.minItemsCustomValidator.CssClass = "validator";
            this.minItemsCustomValidator.ServerValidate += this.MinItemsCustomValidatorServerValidate;

            this.maxItemsTextBox.ID = "maxSelectionItemsTextBox";
            this.maxItemsTextBox.Width = 30;
            this.maxItemsTextBox.MaxLength = 2;
            this.maxItemsTextBox.AutoCompleteType = AutoCompleteType.None;

            this.maxItemsRegularExpressionValidator.ControlToValidate = this.maxItemsTextBox.ID;
            this.ConfigureNumberValidation(ref this.maxItemsRegularExpressionValidator);

            this.maxItemsCustomValidator.ControlToValidate = this.maxItemsTextBox.ID;
            this.maxItemsCustomValidator.Display = ValidatorDisplay.Dynamic;
            this.maxItemsCustomValidator.CssClass = "validator";
            this.maxItemsCustomValidator.ServerValidate += this.MaxItemsCustomValidator_ServerValidate;

            this.allowDuplicatesCheckBox.ID = "allowDuplicatesCheckBox";

            this.Controls.AddPrevalueControls(
                this.typeRadioButtonList,
                this.xPathTextBox,
                this.xPathRequiredFieldValidator,
                this.xPathCustomValidator,
                this.sortOnDropDown,
                this.sortDirectionRadioButtonList,
                this.limitToTextBox,
                this.limitToRegularExpressionValidator,
                this.listHeightTextBox,
                this.listHeightRegularExpressionValidator,
                this.macroDropDownList,
                this.cssFileDropDownList,
                this.scriptFileDropDownList,
                this.minItemsTextBox,
                this.minItemsRegularExpressionValidator,
                this.minItemsCustomValidator,
                this.maxItemsTextBox,
                this.minItemsRegularExpressionValidator,
                this.maxItemsCustomValidator,
                this.allowDuplicatesCheckBox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // check to see if this control is being loaded for the first time (could be a postback of a different datatype - having changed the render control setting)
            if (this.ViewState[this.DataType.Id.ToString()] == null)
            {
                // set viewstate so we know this control has rendered for next time
                this.ViewState[this.DataType.Id.ToString()] = true;
            
                this.typeRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Document.GetFriendlyName(), uQuery.UmbracoObjectType.Document.GetGuid().ToString()));
                this.typeRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Media.GetFriendlyName(), uQuery.UmbracoObjectType.Media.GetGuid().ToString()));
                this.typeRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Member.GetFriendlyName(), uQuery.UmbracoObjectType.Member.GetGuid().ToString()));

                // the oninit event of the propertyTypePicker loads data first
                this.sortOnDropDown.Items.Insert(1, new ListItem("<Name>", "Name"));
                this.sortOnDropDown.Items.Insert(2, new ListItem("<Update Date>", "UpdateDate"));
                this.sortOnDropDown.Items.Insert(3, new ListItem("<Create Date>", "CreateDate"));
                
                this.sortDirectionRadioButtonList.Items.Add(new ListItem(ListSortDirection.Ascending.ToString()));
                this.sortDirectionRadioButtonList.Items.Add(new ListItem(ListSortDirection.Descending.ToString()));
                
                this.macroDropDownList.DataValueField = "Alias"; // key
                this.macroDropDownList.DataTextField = "Name";
                this.macroDropDownList.DataSource = Macro.GetAll();
                this.macroDropDownList.DataBind();
                this.macroDropDownList.Items.Insert(0, string.Empty);

                this.cssFileDropDownList.DataSource = this.GetAllFilesForDropDownList(HostingEnvironment.MapPath("~/css/"), "*.css");                
                this.cssFileDropDownList.DataBind();
                this.cssFileDropDownList.Items.Insert(0, string.Empty);

                this.scriptFileDropDownList.DataSource = this.GetAllFilesForDropDownList(HostingEnvironment.MapPath("~/scripts/"), "*.js");
                this.scriptFileDropDownList.DataBind();
                this.scriptFileDropDownList.Items.Insert(0, string.Empty);
            }

            this.typeRadioButtonList.SelectedValue = this.Options.Type;
            this.xPathTextBox.Text = this.Options.XPath;
            this.sortOnDropDown.SelectedValue = this.Options.SortOn;
            this.sortDirectionRadioButtonList.SelectedValue = this.Options.SortDirection.ToString();
            this.limitToTextBox.Text = this.Options.LimitTo.ToString();
            this.listHeightTextBox.Text = this.Options.ListHeight.ToString();

            this.macroDropDownList.SetSelectedValue(this.Options.MacroAlias);
            this.cssFileDropDownList.SetSelectedValue(this.Options.CssFile);
            this.scriptFileDropDownList.SetSelectedValue(this.Options.ScriptFile);

            this.minItemsTextBox.Text = this.Options.MinItems.ToString();
            this.maxItemsTextBox.Text = this.Options.MaxItems.ToString();
            this.allowDuplicatesCheckBox.Checked = this.Options.AllowDuplicates;

            this.sortDirectionRadioButtonList.Visible = !string.IsNullOrWhiteSpace(this.sortOnDropDown.SelectedValue);
        }

        private void SortOnDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.sortDirectionRadioButtonList.Visible = !string.IsNullOrWhiteSpace(this.sortOnDropDown.SelectedValue);
        }

        #region Validation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">xPathCustomValidator</param>
        /// <param name="args"></param>
        private void XPathCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string xpath = args.Value;
            bool isValid = false;

            uQuery.UmbracoObjectType umbracoObjectType = uQuery.GetUmbracoObjectType(new Guid(this.typeRadioButtonList.SelectedValue));

            try
            {
                switch (umbracoObjectType)
                {
                    case uQuery.UmbracoObjectType.Document: uQuery.GetNodesByXPath(xpath); 
                        break;
                    case uQuery.UmbracoObjectType.Media: uQuery.GetMediaByXPath(xpath);
                        break;
                    case uQuery.UmbracoObjectType.Member: uQuery.GetMembersByXPath(xpath);
                        break;
                }

                isValid = true;
            }
            catch
            {
                this.xPathCustomValidator.ErrorMessage = " Error in XPath expression";
            }

            args.IsValid = isValid;
        }


        // TODO: TextTemplateTextBoxCustomValidator


        private void MinItemsCustomValidatorServerValidate(object source, ServerValidateEventArgs args)
        {            
            bool isValid = false;

            int minItems;
            int maxItems;

            if (int.TryParse(args.Value, out minItems))
            {
                if (minItems >= 0)
                {
                    if (int.TryParse(this.maxItemsTextBox.Text, out maxItems))
                    {
                        if (minItems <= maxItems || maxItems == 0)
                        {
                            isValid = true;
                        }
                        else
                        {
                            this.minItemsCustomValidator.ErrorMessage = " Min Items must be less than or equal to Max Items";            
                        }
                    }
                }
                else
                {
                    this.minItemsCustomValidator.ErrorMessage = " Min Items must be 0 or greater";
                }
            }
            else
            {
                this.minItemsCustomValidator.ErrorMessage = " Min Items must be a number";
            }

            args.IsValid = isValid;
        }
        
        private void MaxItemsCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool isValid = false;

            int minItems;
            int maxItems;

            if (int.TryParse(args.Value, out maxItems))
            {
                if (maxItems >= 0)
                {
                    if (int.TryParse(this.minItemsTextBox.Text, out minItems))
                    {
                        if (maxItems >= minItems || maxItems == 0)
                        {
                            isValid = true;
                        }
                        else
                        {
                            this.maxItemsCustomValidator.ErrorMessage = " Max Items must be greater than or equal to Min Items";
                        }
                    }
                }
                else
                {
                    this.maxItemsCustomValidator.ErrorMessage = " Max Items must be 0 or greater";
                }
            }
            else
            {
                this.maxItemsCustomValidator.ErrorMessage = " Max Items must be a number";
            }

            args.IsValid = isValid;
        }

        #endregion

        /// <summary>
        /// Saves the pre value data to Umbraco
        /// </summary>
        public override void Save()
        {
            if (this.Page.IsValid)
            {
                this.Options.Type = this.typeRadioButtonList.SelectedValue;
                this.Options.XPath = this.xPathTextBox.Text;
                this.Options.SortOn = this.sortOnDropDown.SelectedValue;
                if (!string.IsNullOrWhiteSpace(this.sortDirectionRadioButtonList.SelectedValue))
                {
                    this.Options.SortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), this.sortDirectionRadioButtonList.SelectedValue);
                }

                int limitTo;
                int.TryParse(this.limitToTextBox.Text, out limitTo);
                this.Options.LimitTo = limitTo;

                int listHeight;
                int.TryParse(this.listHeightTextBox.Text, out listHeight);
                this.Options.ListHeight = listHeight;

                this.Options.MacroAlias = this.macroDropDownList.SelectedValue;

                this.Options.CssFile = this.cssFileDropDownList.SelectedValue;
                this.Options.ScriptFile = this.scriptFileDropDownList.SelectedValue;

                int minItems;
                int.TryParse(this.minItemsTextBox.Text, out minItems);
                this.Options.MinItems = minItems;

                int maxItems;
                int.TryParse(this.maxItemsTextBox.Text, out maxItems);
                this.Options.MaxItems = maxItems;

                this.Options.AllowDuplicates = this.allowDuplicatesCheckBox.Checked;

                this.SaveAsJson(this.Options);  // Serialize to Umbraco database field
            }
            else
            {
                //// save default options
                //this.SaveAsJson(this.Options);
            }
        }

        /// <summary>
        /// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("Type", @"xml schema to query", this.typeRadioButtonList);
            writer.AddPrevalueRow("XPath Expression", @"expects a result set of node, meda or member elements", this.xPathTextBox, this.xPathRequiredFieldValidator, this.xPathCustomValidator);
            writer.AddPrevalueRow("Sort On", "property to sort the source data on - empty = xml order", this.sortOnDropDown);

            if (this.sortDirectionRadioButtonList.Visible)
            {
                writer.AddPrevalueRow("Sort Direction", "", this.sortDirectionRadioButtonList);
            }
            else
            {
                // render the control so it's state is persisted, but without any additional layout markup
                this.sortDirectionRadioButtonList.RenderControl(writer);
            }

            writer.AddPrevalueRow("Limit To", "limit the source data count - 0 means no limit", this.limitToTextBox, this.limitToRegularExpressionValidator);
            writer.AddPrevalueRow("List Height", "px height of the source list - 0 means not set / no scrolling", this.listHeightTextBox, this.listHeightRegularExpressionValidator);

            writer.AddPrevalueRow("Macro", "macro expects an int paramter named 'id'", this.macroDropDownList);
            writer.AddPrevalueRow("Css File", "can use classes: .xpath-templatable-list.datatype-id-" + this.DataType.DataTypeDefinitionId + ".property-alias-????", this.cssFileDropDownList);
            writer.AddPrevalueRow("Script File", "contents passed as callback parameter to datatype initialization (can use classes above)", this.scriptFileDropDownList);

            writer.AddPrevalueRow("Min Items", "number of items that must be selected", this.minItemsTextBox, this.minItemsRegularExpressionValidator, this.minItemsCustomValidator);
            writer.AddPrevalueRow("Max Items", "number of items that can be selected - 0 means no limit", this.maxItemsTextBox, this.maxItemsCustomValidator); // BUG: this.maxItemsRegularExpressionValidator doens't work here !
            writer.AddPrevalueRow("Allow Duplicates", "when checked, duplicate values can be selected", this.allowDuplicatesCheckBox);
        }

        private void ConfigureNumberValidation(ref RegularExpressionValidator regularExpressionValidator)
        {
            regularExpressionValidator.Display = ValidatorDisplay.Dynamic;
            regularExpressionValidator.CssClass = "validator";
            regularExpressionValidator.ErrorMessage = " Must be a number";
            regularExpressionValidator.ValidationExpression = @"^\d{1,3}$";
        }

        private IEnumerable<ListItem> GetAllFilesForDropDownList(string path, string searchPattern)
        {
            var root = new Uri(HostingEnvironment.MapPath("~/"));

            return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                .Select(x => 
                    new ListItem(
                        Uri.UnescapeDataString(
                            "/" + root.MakeRelativeUri(new Uri(x)).ToString())));
        }
    }
}
