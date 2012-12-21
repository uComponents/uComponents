// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 11.08.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Linq;
using System.Web.UI;
using uComponents.Core;
using uComponents.DataTypes.DataTypeGrid.Configuration;
using uComponents.DataTypes.DataTypeGrid.WebServices;

[assembly: WebResource("uComponents.DataTypes.DataTypeGrid.Css.DTG_PrevalueEditor.css", Constants.MediaTypeNames.Text.Css)]
[assembly: WebResource("uComponents.DataTypes.DataTypeGrid.Scripts.DTG_PrevalueEditor.js", Constants.MediaTypeNames.Application.JavaScript)]

namespace uComponents.DataTypes.DataTypeGrid
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Web.Script.Serialization;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	using Model;
	using uComponents.DataTypes.DataTypeGrid.Functions;
	using umbraco;
	using umbraco.cms.businesslogic.datatype;
	using umbraco.editorControls;
	using Umbraco.Core.IO;

    /// <summary>
    /// The PreValue Editor for the DTG DataType.
    /// </summary>
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Javascript, "ui/jqueryui.js",
        "UmbracoClient")]
    public class PrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
    {
        /// <summary>
        /// An object to temporarily lock writing to the database.
        /// </summary>
        private static readonly object m_Locker = new object();

        /// <summary>
        /// The container for the accordion
        /// </summary>
        private Panel _accordionContainer = new Panel();

        /// <summary>
        /// Wether to show the property name label or not
        /// </summary>
        private CheckBox _showLabel = new CheckBox();

        /// <summary>
        /// Wether to show the table header or not
        /// </summary>
        private CheckBox _showHeader = new CheckBox();

        /// <summary>
        /// Wether to show the table footer or not
        /// </summary>
        private CheckBox _showFooter = new CheckBox();

        /// <summary>
        /// The number of rows to show in the table
        /// </summary>
        private TextBox _numberOfRows = new TextBox() { Text = "10" };

        /////// <summary>
        /////// Flag for indicating if a delete operation is in process
        /////// </summary>
        ////private bool _deleteMode = false;

        /// <summary>
        /// The validator for _numberOfRows
        /// </summary>
        private RegularExpressionValidator _numberOfRowsValidator = new RegularExpressionValidator();

        /// <summary>
        /// The array containing the stored values.
        /// </summary>
        private IList<PreValueRow> _preValues;

        /// <summary>
        /// 
        /// </summary>
        private PreValueRow _newPreValue;

        /// <summary>
        /// 
        /// </summary>
        private PreValueEditorSettings _settings;

        /// <summary>
        /// 
        /// </summary>
        public PreValueEditorSettings Settings
        {
            get
            {
                if (this._settings == null)
                {
                    var prevalues = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);

                    var serializer = new JavaScriptSerializer();

                    return serializer.Deserialize<PreValueEditorSettings>(((PreValue)prevalues[0]).Value);
                }

                return new PreValueEditorSettings();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrevalueEditor"/> class.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        public PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
            : base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
        {
            // Ensure settings file exists
            DtgHelpers.EnsureFileExists(
                IOHelper.MapPath("~/config/DataTypeGrid.config"),
                DtgConfiguration.DataTypeGrid,
                m_Locker);

            // Ensure webservice file exists
            var dtgFolder = DtgHelpers.EnsureFolderExists("DataTypeGrid", m_Locker);
            DtgHelpers.EnsureFileExists(Path.Combine(dtgFolder.FullName, "PreValueWebService.asmx"), DtgWebServices.PreValueWebService, m_Locker);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public override void Save()
        {
            lock (m_Locker)
            {
                var prevalues = new List<object>();

                // Set settings
                if (this._settings == null)
                {
                    this._settings = DtgHelpers.GetSettings(this.m_DataType.DataTypeDefinitionId);
                }

                this._settings.ShowLabel = this._showLabel != null && this._showLabel.Checked;
                this._settings.ShowTableHeader = this._showHeader != null && this._showHeader.Checked;
                this._settings.ShowTableFooter = this._showFooter != null && this._showFooter.Checked;
                this._settings.NumberOfRows = this._numberOfRows != null ? int.Parse(this._numberOfRows.Text) : 10;
                this._settings.ContentSorting = this.GetContentSorting(this._preValues);
                prevalues.Add(this._settings);

                // Add existing prevalues;
                foreach (var t in this._preValues)
                {
                    var parsedprevalue = ParsePrevalue(t);

                    if (parsedprevalue != null)
                    {
                        prevalues.Add(parsedprevalue);
                    }
                }

                // Add last (new) prevalue
                var newprevalue = ParsePrevalue(this._newPreValue);

                if (newprevalue != null)
                {
                    prevalues.Add(newprevalue);
                }

                if (prevalues.Count > 0)
                {
                    // Delete former values
                    PreValues.DeleteByDataTypeDefinition(this.m_DataType.DataTypeDefinitionId);

                    // Add new values
                    AddPrevalues(prevalues);

                    // Must not refresh on initial load. [LK]
                    if (prevalues.Count > 1)
                    {
                        // Reload IFrame to show changes
                        this.Page.Response.Redirect(this.Page.Request.Url.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.EnsureChildControls();

            // Adds the client dependencies.
            this.RegisterEmbeddedClientResource(Constants.PrevalueEditorCssResourcePath, ClientDependencyType.Css);
            this.RegisterEmbeddedClientResource(
                "uComponents.DataTypes.Shared.Resources.Scripts.json2.js", ClientDependencyType.Javascript);
            this.RegisterEmbeddedClientResource(
                "uComponents.DataTypes.DataTypeGrid.Css.DTG_PrevalueEditor.css", ClientDependencyType.Css);
            this.RegisterEmbeddedClientResource(
                "uComponents.DataTypes.DataTypeGrid.Scripts.DTG_PrevalueEditor.js", ClientDependencyType.Javascript);
        }

        /// <summary>
        /// Creates child controls for this control
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            // Get configuration
            this._newPreValue = new PreValueRow();
            this._preValues = new List<PreValueRow>();
            this._settings = DtgHelpers.GetSettings(this.m_DataType.DataTypeDefinitionId);
            this.GetConfig();

            // Instantiate default controls
            this._accordionContainer = new Panel
                { ID = "dtg_accordion_" + this.m_DataType.DataTypeDefinitionId, CssClass = "dtg_accordion" };
            this._showLabel = new CheckBox() { ID = "showLabel", Checked = this._settings.ShowLabel };
            this._showHeader = new CheckBox() { ID = "showHeader", Checked = this._settings.ShowTableHeader };
            this._showFooter = new CheckBox() { ID = "showFooter", Checked = this._settings.ShowTableFooter };
            this._numberOfRows = new TextBox() { ID = "NumberOfRows", Text = this._settings.NumberOfRows.ToString() };
            this._numberOfRowsValidator = new RegularExpressionValidator()
                {
                    ID = "NumberOfRowsValidator",
                    CssClass = "validator",
                    ValidationExpression = @"^[1-9]*[0-9]*$",
                    ControlToValidate = _numberOfRows.ClientID,
                    Display = ValidatorDisplay.Dynamic,
                    ErrorMessage = Helper.Dictionary.GetDictionaryItem("MustBeANumber", "Must be a number")
                };

            // Write controls for adding new entry
            var addNewProperty = new Panel() { ID = "newProperty", CssClass = "addNewProperty" };

            var addNewPropertyHeader = new Panel() { CssClass = "propertyHeader" };

            var addNewPropertyTitle = new HtmlGenericControl("h3")
                { InnerText = Helper.Dictionary.GetDictionaryItem("AddNewDataType", "Add new datatype") };
            addNewPropertyTitle.Attributes["class"] = "propertyTitle";

            var icnNewError = new HtmlGenericControl("span")
                { InnerText = Helper.Dictionary.GetDictionaryItem("Error", "Error") };
            icnNewError.Attributes["class"] = "ErrorProperty";

            addNewPropertyHeader.Controls.Add(addNewPropertyTitle);
            addNewPropertyHeader.Controls.Add(icnNewError);

            var addNewPropertyControls = new Panel() { ID = "addNewPropertyControls", CssClass = "propertyControls" };
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "<ul>" });

            // NAME
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

            // Instantiate controls
            var txtNewName = new TextBox() { ID = "newName", CssClass = "newName" };
            var lblNewName = new Label()
                { Text = Helper.Dictionary.GetDictionaryItem("Name", "Name"), CssClass = "label" };
            var valNewName = new RequiredFieldValidator()
                {
                    ID = "newNameValidator",
                    CssClass = "validator",
                    Enabled = false,
                    ControlToValidate = txtNewName.ClientID,
                    Display = ValidatorDisplay.Dynamic,
                    ErrorMessage = Helper.Dictionary.GetDictionaryItem("YouMustSpecifyAName", "You must specify a name")
                };

            // Add controls to control
            addNewPropertyControls.Controls.Add(lblNewName);
            addNewPropertyControls.Controls.Add(txtNewName);
            addNewPropertyControls.Controls.Add(valNewName);
            ((PreValueRow)this._newPreValue).Controls.Add(txtNewName);
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });

            // ALIAS
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

            // Instantiate controls
            var txtNewAlias = new TextBox() { ID = "newAlias", CssClass = "newAlias" };
            var lblNewAlias = new Label()
                { Text = Helper.Dictionary.GetDictionaryItem("Alias", "Alias"), CssClass = "label" };
            var valNewAlias = new RequiredFieldValidator()
                {
                    ID = "newAliasValidator",
                    CssClass = "validator",
                    Enabled = false,
                    ControlToValidate = txtNewAlias.ClientID,
                    Display = ValidatorDisplay.Dynamic,
                    ErrorMessage =
                        Helper.Dictionary.GetDictionaryItem("YouMustSpecifyAnAlias", "You must specify an alias")
                };

            var valNewAliasExists = new CustomValidator()
                {
                    ID = "newAliasExistsValidator",
                    CssClass = "validator exists",
                    Enabled = false,
                    ControlToValidate = txtNewAlias.ClientID,
                    Display = ValidatorDisplay.Dynamic,
                    ClientValidationFunction = "ValidateNewAliasExists",
                    ErrorMessage = Helper.Dictionary.GetDictionaryItem("AliasAlreadyExists", "Alias already exists!")
                };
            valNewAliasExists.ServerValidate += valNewAliasExists_ServerValidate;

            // Add controls to control
            addNewPropertyControls.Controls.Add(lblNewAlias);
            addNewPropertyControls.Controls.Add(txtNewAlias);
            addNewPropertyControls.Controls.Add(valNewAlias);
            addNewPropertyControls.Controls.Add(valNewAliasExists);
            ((PreValueRow)this._newPreValue).Controls.Add(txtNewAlias);
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });

            // DATATYPE
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

            // Instantiate controls
            var ddlNewType = DtgHelpers.GetDataTypeDropDown();
            ddlNewType.ID = "newType";
            var lblNewType = new Label()
                { Text = Helper.Dictionary.GetDictionaryItem("DataType", "Datatype"), CssClass = "label" };

            // Add controls to control
            addNewPropertyControls.Controls.Add(lblNewType);
            addNewPropertyControls.Controls.Add(ddlNewType);
            ((PreValueRow)this._newPreValue).Controls.Add(ddlNewType);
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });

            // VALIDATION
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

            // Instantiate controls
            var txtNewValidation = new TextBox()
                {
                    ID = "newValidation",
                    TextMode = TextBoxMode.MultiLine,
                    Rows = 2,
                    Columns = 20,
                    CssClass = "newValidation"
                };
            var lblNewValidation = new Label()
                { Text = Helper.Dictionary.GetDictionaryItem("Validation", "Validation"), CssClass = "label" };
            var lnkNewValidation = new HyperLink
                {
                    ID = "newValidationLink",
                    CssClass = "validationLink",
                    NavigateUrl = "#",
                    Text =
                        Helper.Dictionary.GetDictionaryItem(
                            "SearchForARegularExpression", "Search for a regular expression")
                };
            var valNewValidation = new CustomValidator()
                {
                    ID = "newValidationValidator",
                    CssClass = "validator",
                    ControlToValidate = txtNewValidation.ClientID,
                    Display = ValidatorDisplay.Dynamic,
                    ClientValidationFunction = "ValidateRegex",
                    ErrorMessage =
                        Helper.Dictionary.GetDictionaryItem(
                            "ValidationStringIsNotValid", "Validation string is not valid")
                };
            valNewValidation.ServerValidate += this.valNewValidation_ServerValidate;

            // Add controls to control
            addNewPropertyControls.Controls.Add(lblNewValidation);
            addNewPropertyControls.Controls.Add(txtNewValidation);
            addNewPropertyControls.Controls.Add(valNewValidation);
            addNewPropertyControls.Controls.Add(lnkNewValidation);
            ((PreValueRow)this._newPreValue).Controls.Add(txtNewValidation);

            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });


            // CONTENT SORT PRIORITY
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

            // Instantiate controls
            var ddlNewContentSortPriority = new DropDownList()
                { ID = "newContentSortPriority", CssClass = "newContentSortPriority", };
            PopulatePriorityDropDownList(ddlNewContentSortPriority, this._preValues, null);

            var lblNewContentSortPriority = new Label()
                {
                    Text = Helper.Dictionary.GetDictionaryItem("ContentSortPriority", "Content Sort Priority"),
                    CssClass = "label"
                };

            // Add controls to control
            addNewPropertyControls.Controls.Add(lblNewContentSortPriority);
            addNewPropertyControls.Controls.Add(ddlNewContentSortPriority);
            ((PreValueRow)this._newPreValue).Controls.Add(ddlNewContentSortPriority);
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });


            // CONTENT SORT ORDER
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

            // Instantiate controls
            var ddlNewContentSortOrder = new DropDownList()
                { ID = "newContentSortOrder", CssClass = "newContentSortOrder", };
            ddlNewContentSortOrder.Items.Add(new ListItem(string.Empty, string.Empty));
            ddlNewContentSortOrder.Items.Add(new ListItem(uQuery.GetDictionaryItem("Ascending", "Ascending"), "asc"));
            ddlNewContentSortOrder.Items.Add(new ListItem(uQuery.GetDictionaryItem("Descending", "Descending"), "desc"));
            var lblNewContentSortOrder = new Label()
                {
                    Text = Helper.Dictionary.GetDictionaryItem("ContentSortOrder", "Content Sort Order"),
                    CssClass = "label"
                };

            // Add controls to control
            addNewPropertyControls.Controls.Add(lblNewContentSortOrder);
            addNewPropertyControls.Controls.Add(ddlNewContentSortOrder);
            ((PreValueRow)this._newPreValue).Controls.Add(ddlNewContentSortOrder);
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });


            // PREVALUE SORT ORDER

            // Instantiate controls
            var hdnNewSortOrder = new HiddenField() { Value = (this._preValues.Count + 1).ToString() };
            addNewPropertyControls.Controls.Add(hdnNewSortOrder);
            ((PreValueRow)this._newPreValue).Controls.Add(hdnNewSortOrder);

            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "</ul>" });

            addNewProperty.Controls.Add(addNewPropertyHeader);
            addNewProperty.Controls.Add(addNewPropertyControls);

            this._accordionContainer.Controls.Add(addNewProperty);

            // Write stored entries)
            foreach (var s in this._preValues)
            {
                var editProperty = new Panel() { ID = "editProperty_" + s.Id.ToString(), CssClass = "editProperty" };

                var editPropertyHeader = new Panel() { CssClass = "propertyHeader" };
                var editPropertyTitle = new HtmlGenericControl("h3")
                    {
                        InnerText =
                            s.Name + " (" + s.Alias + "), " + uQuery.GetDictionaryItem("Type", "Type") + ": "
                            + ddlNewType.Items.FindByValue(s.DataTypeId.ToString()).Text
                    };
                editPropertyTitle.Attributes["class"] = "propertyTitle";

                var lnkDelete = new LinkButton
                    {
                        CssClass = "DeleteProperty ui-button ui-widget ui-state-default ui-corner-all ui-button-icon-only",
                        Text =
                            "<span class='ui-button-icon-primary ui-icon ui-icon-close'>&nbsp;</span><span class='ui-button-text'>Delete</span>",
                        OnClientClick =
                            "return confirm('"
                            +
                            uQuery.GetDictionaryItem(
                                "AreYouSureYouWantToDeleteThisColumn", "Are you sure you want to delete this column")
                            + "?');",
                        CommandArgument = s.Id.ToString(),
                        CommandName = "Delete"
                    };
                lnkDelete.Command += new CommandEventHandler(this.lnkDelete_Command);

                var icnEditError = new HtmlGenericControl("span")
                    { InnerText = Helper.Dictionary.GetDictionaryItem("Error", "Error") };
                icnEditError.Attributes["class"] = "ErrorProperty";

                editPropertyHeader.Controls.Add(editPropertyTitle);
                editPropertyHeader.Controls.Add(lnkDelete);
                editPropertyHeader.Controls.Add(icnEditError);

                var editPropertyControls = new Panel() { CssClass = "propertyControls" };

                editPropertyControls.Controls.Add(new LiteralControl() { Text = "<ul>" });

                // NAME
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

                // Instantiate controls
                var txtEditName = new TextBox()
                    { ID = "editName_" + this._preValues.IndexOf(s), CssClass = "editName", Text = s.Name };
                var lblEditName = new Label()
                    { Text = Helper.Dictionary.GetDictionaryItem("Name", "Name"), CssClass = "label" };
                var valEditName = new RequiredFieldValidator()
                    {
                        ID = "editNameValidator_" + this._preValues.IndexOf(s),
                        CssClass = "validator",
                        ControlToValidate = txtEditName.ClientID,
                        Display = ValidatorDisplay.Dynamic,
                        ErrorMessage = "You must specify a name"
                    };

                // Add controls to control
                editPropertyControls.Controls.Add(lblEditName);
                editPropertyControls.Controls.Add(txtEditName);
                editPropertyControls.Controls.Add(valEditName);
                s.Controls.Add(txtEditName);
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });


                // ALIAS
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

                // Instantiate controls
                var txtEditAlias = new TextBox()
                    { ID = "editAlias_" + this._preValues.IndexOf(s), CssClass = "editAlias", Text = s.Alias };
                var lblEditAlias = new Label()
                    { Text = Helper.Dictionary.GetDictionaryItem("Alias", "Alias"), CssClass = "label" };
                var valEditAlias = new RequiredFieldValidator()
                    {
                        ID = "editAliasValidator_" + this._preValues.IndexOf(s),
                        CssClass = "validator",
                        ControlToValidate = txtEditAlias.ClientID,
                        Display = ValidatorDisplay.Dynamic,
                        ErrorMessage = "You must specify an alias"
                    };
                var valEditAliasExists = new CustomValidator()
                    {
                        ID = "editAliasExistsValidator_" + this._preValues.IndexOf(s),
                        CssClass = "validator exists",
                        ControlToValidate = txtEditAlias.ClientID,
                        Display = ValidatorDisplay.Dynamic,
                        ClientValidationFunction = "ValidateAliasExists",
                        ErrorMessage = "Alias already exists!"
                    };
                valEditAliasExists.ServerValidate += valEditAliasExists_ServerValidate;

                // Add controls to control
                editPropertyControls.Controls.Add(lblEditAlias);
                editPropertyControls.Controls.Add(txtEditAlias);
                editPropertyControls.Controls.Add(valEditAlias);
                editPropertyControls.Controls.Add(valEditAliasExists);
                s.Controls.Add(txtEditAlias);
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });


                // DATATYPE
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

                // Instantiate controls
                var ddlEditType = DtgHelpers.GetDataTypeDropDown();
                ddlEditType.ID = "editDataType_" + this._preValues.IndexOf(s);
                var lblEditType = new Label()
                    { Text = Helper.Dictionary.GetDictionaryItem("DataType", "DataType"), CssClass = "label" };

                // Add controls to control
                editPropertyControls.Controls.Add(lblEditType);
                ddlEditType.SelectedValue = s.DataTypeId.ToString();
                editPropertyControls.Controls.Add(ddlEditType);
                s.Controls.Add(ddlEditType);
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });

                // VALIDATION
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

                // Instantiate control
                var txtEditValidation = new TextBox()
                    {
                        ID = "editValidation_" + this._preValues.IndexOf(s),
                        TextMode = TextBoxMode.MultiLine,
                        Rows = 2,
                        Columns = 20,
                        CssClass = "editValidation",
                        Text = s.ValidationExpression
                    };
                var lblEditValidation = new Label()
                    { Text = Helper.Dictionary.GetDictionaryItem("Validation", "Validation"), CssClass = "label" };
                var lnkEditValidation = new HyperLink
                    {
                        CssClass = "validationLink",
                        NavigateUrl = "#",
                        Text =
                            Helper.Dictionary.GetDictionaryItem(
                                "SearchForARegularExpression", "Search for a regular expression")
                    };
                var valEditValidation = new CustomValidator()
                    {
                        ID = "editValidationValidator_" + this._preValues.IndexOf(s),
                        CssClass = "validator",
                        ControlToValidate = txtEditValidation.ClientID,
                        Display = ValidatorDisplay.Dynamic,
                        ClientValidationFunction = "ValidateRegex",
                        ErrorMessage =
                            Helper.Dictionary.GetDictionaryItem(
                                "ValidationStringIsNotValid", "Validation string is not valid")
                    };
                valEditValidation.ServerValidate += valEditValidation_ServerValidate;

                // Add controls to control
                editPropertyControls.Controls.Add(lblEditValidation);
                editPropertyControls.Controls.Add(txtEditValidation);
                editPropertyControls.Controls.Add(valEditValidation);
                editPropertyControls.Controls.Add(lnkEditValidation);
                s.Controls.Add(txtEditValidation);

                editPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });


                // CONTENT SORT PRIORITY
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

                // Instantiate controls
                var ddlEditContentSortPriority = new DropDownList()
                    {
                        ID = "editContentSortPriority_" + this._preValues.IndexOf(s),
                        CssClass = "editContentSortPriority",
                        Text = s.Alias
                    };
                PopulatePriorityDropDownList(ddlEditContentSortPriority, this._preValues, s.ContentSortPriority);

                var lblEditContentSortPriority = new Label()
                    {
                        Text = Helper.Dictionary.GetDictionaryItem("ContentSortPriority", "Content Sort Priority"),
                        CssClass = "label"
                    };

                // Add controls to control
                editPropertyControls.Controls.Add(lblEditContentSortPriority);
                editPropertyControls.Controls.Add(ddlEditContentSortPriority);
                s.Controls.Add(ddlEditContentSortPriority);
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });


                // CONTENT SORT ORDER
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

                // Instantiate controls
                var ddlEditContentSortOrder = new DropDownList()
                    {
                        ID = "editContentSortOrder_" + this._preValues.IndexOf(s),
                        CssClass = "editContentSortOrder",
                        Text = s.Alias
                    };
                ddlEditContentSortOrder.Items.Add(new ListItem(string.Empty, string.Empty));
                ddlEditContentSortOrder.Items.Add(new ListItem("Ascending", "asc"));
                ddlEditContentSortOrder.Items.Add(new ListItem("Descending", "desc"));
                ddlEditContentSortOrder.SelectedValue = string.IsNullOrEmpty(s.ContentSortOrder)
                                                            ? string.Empty
                                                            : s.ContentSortOrder;

                var lblEditContentSortOrder = new Label()
                    {
                        Text = Helper.Dictionary.GetDictionaryItem("ContentSortOrder", "Content Sort Order"),
                        CssClass = "label"
                    };

                // Add controls to control
                editPropertyControls.Controls.Add(lblEditContentSortOrder);
                editPropertyControls.Controls.Add(ddlEditContentSortOrder);
                s.Controls.Add(ddlEditContentSortOrder);
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });


                // SORT ORDER

                // Instantiate controls
                var hdnEditSortOrderWrapper = new Panel() { CssClass = "sortOrder" };
                var hdnEditSortOrder = new HiddenField() { Value = s.SortOrder.ToString() };
                hdnEditSortOrderWrapper.Controls.Add(hdnEditSortOrder);
                editPropertyControls.Controls.Add(hdnEditSortOrderWrapper);
                s.Controls.Add(hdnEditSortOrder);

                editPropertyControls.Controls.Add(new LiteralControl() { Text = "</ul>" });

                editProperty.Controls.Add(editPropertyHeader);
                editProperty.Controls.Add(editPropertyControls);

                this._accordionContainer.Controls.Add(editProperty);
            }

            this.Controls.Add(this._showLabel);
            this.Controls.Add(this._showHeader);
            this.Controls.Add(this._showFooter);
            this.Controls.Add(this._numberOfRows);
            this.Controls.Add(this._numberOfRowsValidator);
            this.Controls.Add(this._accordionContainer);
        }

        private void valEditValidation_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = DtgHelpers.ValidateRegex(e.Value);
        }

        /// <summary>
        /// Handles the ServerValidate event of the regexValidation control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        private void valNewValidation_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = DtgHelpers.ValidateRegex(e.Value);
        }

        /// <summary>
        /// Handles the ServerValidate event of the valNewAliasExistsValidator control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        private void valNewAliasExists_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = ValidateAliasExists((CustomValidator)source);
        }

        /// <summary>
        /// Handles the ServerValidate event of the valEditAliasExists control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        private void valEditAliasExists_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = ValidateAliasExists((CustomValidator)source);
        }

        private bool ValidateAliasExists(IValidator source)
        {
            var exists = true;
            var prevalue = ParsePrevalue(this._newPreValue);
            var val = source as CustomValidator;

            if (prevalue != null)
            {
                exists = ContainsPreValue(this._preValues.ToList(), prevalue.Alias);
            }

            if (exists && val != null && prevalue != null)
            {
                val.ErrorMessage = "A PreValue with alias: " + prevalue.Alias + " already exists!";
            }

            return !exists;
        }

        /// <summary>
        /// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("Show Label", this._showLabel);
            writer.AddPrevalueRow("Show Table Header", this._showHeader);
            writer.AddPrevalueRow("Show Table Footer", this._showFooter);
            writer.AddPrevalueRow("Number of rows", new Control[] { this._numberOfRows, this._numberOfRowsValidator });
            this._accordionContainer.RenderControl(writer);

            // Add javascript preview of alias
            var javascriptLivePreview =
                string.Format(
                    "$('#{0}').keyup(function(){{ $('#{1}').val(safeAlias($(this).val())); }});",
                    ((TextBox)this._accordionContainer.FindControl("newName")).ClientID,
                    ((TextBox)this._accordionContainer.FindControl("newAlias")).ClientID);
            var safeAliasScript =
                "var UMBRACO_FORCE_SAFE_ALIAS = true; var UMBRACO_FORCE_SAFE_ALIAS_VALIDCHARS = '_-abcdefghijklmnopqrstuvwxyz1234567890'; var UMBRACO_FORCE_SAFE_ALIAS_INVALID_FIRST_CHARS = '01234567890';function safeAlias(alias) { if (UMBRACO_FORCE_SAFE_ALIAS) { var safeAlias = ''; var aliasLength = alias.length; for (var i = 0; i < aliasLength; i++) { currentChar = alias.substring(i, i + 1); if (UMBRACO_FORCE_SAFE_ALIAS_VALIDCHARS.indexOf(currentChar.toLowerCase()) > -1) { if (safeAlias == '' && UMBRACO_FORCE_SAFE_ALIAS_INVALID_FIRST_CHARS.indexOf(currentChar.toLowerCase()) > 0) { currentChar = ''; } else { if (safeAlias.length == 0) currentChar = currentChar.toLowerCase(); if (i < aliasLength - 1 && safeAlias != '' && alias.substring(i - 1, i) == ' ') currentChar = currentChar.toUpperCase(); safeAlias += currentChar; } } } return safeAlias; } else { return alias; } }";
            var javascript = string.Concat(
                "<script type='text/javascript'>$(document).ready(function(){",
                safeAliasScript,
                javascriptLivePreview,
                "});</script>");
            writer.WriteLine(javascript);
        }

        private void lnkDelete_Command(object sender, CommandEventArgs e)
        {
            var id = int.Parse(e.CommandArgument.ToString());

            this._preValues.Remove(this._preValues.Single(s => s.Id == id));

            //// this._deleteMode = true;
            this.Save();
        }

        /// <summary>
        /// Gets the DTG config.
        /// </summary>
        public void GetConfig()
        {
            // Add blank PreValue row in the beginning
            this._newPreValue = new PreValueRow()
                { Id = DtgHelpers.GetAvailableId(this.m_DataType.DataTypeDefinitionId) };

            // Add the stored values
            foreach (var s in DtgHelpers.GetConfig(this.m_DataType.DataTypeDefinitionId))
            {
                this._preValues.Add(s);
            }
        }

        /// <summary>
        /// Parses the prevalue.
        /// </summary>
        /// <param name="t">The config object.</param>
        /// <returns></returns>
        private BasePreValueRow ParsePrevalue(PreValueRow t)
        {
            if (t != null && t.Controls.Count == 7)
            {
                // Get values
                var name = t.Controls[0] != null ? ((TextBox)t.Controls[0]).Text : null;
                var alias = t.Controls[1] != null ? ((TextBox)t.Controls[1]).Text : null;
                var dataTypeId = t.Controls[2] != null ? int.Parse(((DropDownList)t.Controls[2]).SelectedValue) : 0;
                var validation = t.Controls[3] != null ? ((TextBox)t.Controls[3]).Text : null;
                var contentSortPriority = t.Controls[4] != null
                                              ? ((DropDownList)t.Controls[4]).SelectedValue
                                              : string.Empty;
                var contentSortOrder = t.Controls[5] != null
                                           ? ((DropDownList)t.Controls[5]).SelectedValue
                                           : string.Empty;
                var sortOrder = t.Controls[6] != null ? int.Parse(((HiddenField)t.Controls[6]).Value) : 0;

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(alias))
                {
                    // Set the options
                    var preValueRow = new BasePreValueRow()
                        {
                            Id = t.Id,
                            Name = name,
                            Alias = alias,
                            DataTypeId = dataTypeId,
                            ValidationExpression = validation,
                            ContentSortPriority = contentSortPriority,
                            ContentSortOrder = contentSortOrder,
                            SortOrder = sortOrder
                        };

                    // Add new value to database
                    return preValueRow;
                }
            }

            return null;
        }

        private void AddPrevalues(IList s)
        {
            foreach (var config in s)
            {
                // Serialize the options into JSON
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(config);

                // Add new value to database
                if (config.GetType().Name.Equals("PreValueEditorSettings"))
                {
                    uQuery.MakeNewPreValue(this.m_DataType.DataTypeDefinitionId, json, string.Empty, 0);
                }
                else
                {
                    uQuery.MakeNewPreValue(
                        this.m_DataType.DataTypeDefinitionId, json, string.Empty, ((BasePreValueRow)config).SortOrder);
                }
            }
        }

        private bool ContainsPreValue(IList s, string alias)
        {
            foreach (var config in s)
            {
                // Check if list already contains object with alias
                if (!config.GetType().Name.Equals("PreValueEditorSettings")
                    && ((BasePreValueRow)config).Alias.Equals(alias))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Populates the priority drop down list.
        /// </summary>
        /// <param name="ddl">The DDL.</param>
        /// <param name="s">The s.</param>
        /// <param name="currentSortPriority">The current sort priority.</param>
        private void PopulatePriorityDropDownList(DropDownList ddl, IList<PreValueRow> s, string currentSortPriority)
        {
            // Add blank item to beginning
            ddl.Items.Add(new ListItem(string.Empty, string.Empty));

            // Add a number for each stored prevalue
            foreach (var storedConfig in s)
            {
                var priority = s.IndexOf(storedConfig) + 1;

                ddl.Items.Add(new ListItem(priority.ToString(), priority.ToString()));
            }

            ddl.SelectedValue = currentSortPriority ?? string.Empty;
        }

        /// <summary>
        /// Gets the content sorting.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        private string GetContentSorting(IList<PreValueRow> s)
        {
            var list =
                s.Where(
                    storedConfig =>
                    !string.IsNullOrEmpty(storedConfig.ContentSortPriority)
                    && !string.IsNullOrEmpty(storedConfig.ContentSortOrder)).OrderBy(
                        storedConfig => storedConfig.ContentSortPriority);

            var sorting = "[[0, 'asc']]";

            if (list.Count() > 0)
            {
                sorting = "[";

                foreach (var storedConfig in list)
                {
                    sorting += "[ " + (s.IndexOf(storedConfig) + 2) + ", '" + storedConfig.ContentSortOrder + "']";

                    if (list.ToList().IndexOf(storedConfig) < list.Count() - 1)
                    {
                        sorting += ",";
                    }
                }

                sorting += "]";
            }

            return sorting;
        }
    }
}
