using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.Xml;
using System.IO;

using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using umbraco.BusinessLogic;
using umbraco.presentation.nodeFactory;

using uComponents.Core.Shared;
using uComponents.Core.Shared.PrevalueEditors;
using uComponents.Core.uQueryExtensions;

namespace uComponents.Core.DataTypes.AutoComplete
{
    /// <summary>
    /// Pre value editor for the AutoComplete data type
    /// </summary>
    public class AutoCompletePreValueEditor : AbstractJsonPrevalueEditor
    {
        /// <summary>
        /// Radio buttons to select type of node to pick from: Content / Media / Members
        /// </summary>
        private RadioButtonList typeToPickRadioButtonList = new RadioButtonList();

        /// <summary>
        /// XPath statement to select nodes from which a selection may be made
        /// </summary>
        private TextBox xPathTextBox = new TextBox();

        /// <summary>
        /// Validator to ensure an XPath statement has been populated
        /// </summary>
        private RequiredFieldValidator xPathRequiredFieldValidator = new RequiredFieldValidator();

        /// <summary>
        /// Data object used to define the configuration status of this PreValueEditor
        /// </summary>
        private AutoCompleteOptions autoCompleteOptions; //this object will be serialized, (can remove helper method to parse xml configuration string)

		/// <summary>
		/// Gets the options.
		/// </summary>
		/// <value>The options.</value>
		/// <remarks>Lazy load the options data object that represents the current state of this datatypes configuration</remarks>
        internal AutoCompleteOptions Options
        {
            get
            {
                //this will be called for the constructor to the DataEditor, and for reading the config values in this PreValueEditor
                if (this.autoCompleteOptions == null)
                {
                    //load it
                    this.autoCompleteOptions = this.GetPreValueOptions<AutoCompleteOptions>();

                    //if still null, ie, object couldn't be de-serialized from PreValue[0] string value, then create a new instance, this will set defaults
                    if (this.autoCompleteOptions == null)
                    {
                        this.autoCompleteOptions = new AutoCompleteOptions();
                    }
                }

                return this.autoCompleteOptions;
            }
        }

		/// <summary>
		/// Gets the type to pick.
		/// </summary>
		/// <value>The type to pick.</value>
		/// <remarks>Returns the object type picked in the radio button list</remarks>
        private uQuery.UmbracoObjectType TypeToPick
        {
            get
            {
                if (string.IsNullOrEmpty(this.typeToPickRadioButtonList.SelectedValue)) 
                { 
                    return uQuery.UmbracoObjectType.Unknown; 
                }
                else 
                { 
                    return uQuery.GetUmbracoObjectType(new Guid(this.typeToPickRadioButtonList.SelectedValue)); 
                }
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="AutoCompletePreValueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
        public AutoCompletePreValueEditor(BaseDataType dataType)
            : base(dataType, DBTypes.Nvarchar)
        {
        }

        /// <summary>
        /// Creates all of the controls and assigns all of their properties
        /// </summary>
        protected override void CreateChildControls()
        {
            //radio buttons to select type of nodes that can be picked (Document, Media or Member)
            this.typeToPickRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Document.GetFriendlyName(), uQuery.UmbracoObjectType.Document.GetGuid().ToString()));
            this.typeToPickRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Media.GetFriendlyName(), uQuery.UmbracoObjectType.Media.GetGuid().ToString()));
            this.typeToPickRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Member.GetFriendlyName(), uQuery.UmbracoObjectType.Member.GetGuid().ToString()));

            this.xPathTextBox.ID = "xPathTextBox";
            this.xPathTextBox.CssClass = "umbEditorTextField";

            this.xPathRequiredFieldValidator.ControlToValidate = this.xPathTextBox.ID;
            this.xPathRequiredFieldValidator.ErrorMessage = " Required";

            this.Controls.Add(this.typeToPickRadioButtonList);
            this.Controls.Add(this.xPathTextBox);
            this.Controls.Add(this.xPathRequiredFieldValidator);
        }

		/// <summary>
		/// Sets the values of the controls
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            //read in stored values
            this.typeToPickRadioButtonList.SelectedValue = this.Options.TypeToPick;
            this.xPathTextBox.Text = this.Options.XPath;
        }

        /// <summary>
        /// Saves the pre value data to Umbraco
        /// </summary>
        public override void Save()
        {
            if (this.Page.IsValid)
            {
                this.Options.TypeToPick = this.typeToPickRadioButtonList.SelectedValue;
                this.Options.XPath = this.xPathTextBox.Text;

                this.SaveAsJson(this.Options);  // Serialize to Umbraco database field
            }
        }

		/// <summary>
		/// Renders the control output
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("Type of nodes to pick from", this.typeToPickRadioButtonList);
            writer.AddPrevalueRow("XPath for selectable nodes", this.xPathTextBox, this.xPathRequiredFieldValidator);
        }

    }
}
