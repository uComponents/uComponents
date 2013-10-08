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

    using uComponents.DataTypes.DataTypeGrid.Functions;
    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco;
    using umbraco.cms.businesslogic.datatype;
    using umbraco.editorControls;
    using umbraco.IO;

    /// <summary>
    /// The PreValue Editor for the DTG DataType.
    /// </summary>
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Javascript, "ui/jqueryui.js", "UmbracoClient")]
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
        /// Whether to show the property name label or not
        /// </summary>
        private CheckBox _showLabel = new CheckBox();

        /// <summary>
        /// Whether to show the table header or not
        /// </summary>
        private CheckBox _showHeader = new CheckBox();

        /// <summary>
        /// Whether to show the table footer or not
        /// </summary>
        private CheckBox _showFooter = new CheckBox();

        /// <summary>
        /// Whether the grid is read only or not
        /// </summary>
        private CheckBox _readOnly = new CheckBox();

        /// <summary>
        /// The table height
        /// </summary>
        private TextBox _tableHeight = new TextBox() { Text = "300" };

        /// <summary>
        /// The validator for tableHeight
        /// </summary>
        private RegularExpressionValidator _tableHeightValidator = new RegularExpressionValidator();

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
            Helper.IO.EnsureFileExists(
                IOHelper.MapPath("~/config/DataTypeGrid.config"),
                DtgConfiguration.DataTypeGrid);

            // Ensure webservice file exists
            var dtgFolder = Helper.IO.EnsureFolderExists(Path.Combine(DataTypes.Settings.BaseDir.FullName, "DataTypeGrid"));
            Helper.IO.EnsureFileExists(Path.Combine(dtgFolder.FullName, "PreValueWebService.asmx"), DtgWebServices.PreValueWebService);
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
                this._settings.ShowGridHeader = this._showHeader != null && this._showHeader.Checked;
                this._settings.ShowGridFooter = this._showFooter != null && this._showFooter.Checked;
                this._settings.ReadOnly = this._readOnly != null && this._readOnly.Checked;
                this._settings.TableHeight = this._tableHeight != null ? int.Parse(this._tableHeight.Text) : 300;
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
            this._showHeader = new CheckBox() { ID = "showHeader", Checked = this._settings.ShowGridHeader };
            this._showFooter = new CheckBox() { ID = "showFooter", Checked = this._settings.ShowGridFooter };
            this._readOnly = new CheckBox() { ID = "readOnly", Checked = this._settings.ReadOnly };
            this._tableHeight = new TextBox() { ID = "TableHeight", Text = this._settings.TableHeight.ToString() };
            this._tableHeightValidator = new RegularExpressionValidator()
                {
                    ID = "NumberOfRowsValidator",
                    CssClass = "validator",
                    ValidationExpression = @"^[1-9]*[0-9]*$",
                    ControlToValidate = this._tableHeight.ClientID,
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

            // MANDATORY
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

            // Instantiate controls
            var chkNewMandatory = new CheckBox() { ID = "newMandatory", CssClass = "newMandatory" };
            var lblNewMandatory = new Label() { Text = Helper.Dictionary.GetDictionaryItem("Mandatory", "Mandatory"), CssClass = "label" };

            // Add controls to control
            addNewPropertyControls.Controls.Add(lblNewMandatory);
            addNewPropertyControls.Controls.Add(chkNewMandatory);
            ((PreValueRow)this._newPreValue).Controls.Add(chkNewMandatory);
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });

            // Visible
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

            // Instantiate controls
            var chkNewVisible = new CheckBox() { ID = "newVisible", CssClass = "newVisible", Checked = true };
            var lblNewVisible = new Label() { Text = Helper.Dictionary.GetDictionaryItem("Visible", "Visible"), CssClass = "label" };

            // Add controls to control
            addNewPropertyControls.Controls.Add(lblNewVisible);
            addNewPropertyControls.Controls.Add(chkNewVisible);
            ((PreValueRow)this._newPreValue).Controls.Add(chkNewVisible);
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
            addNewPropertyControls.Controls.Add(new LiteralControl() { Text = "<br/>" });
            addNewPropertyControls.Controls.Add(lnkNewValidation);
            ((PreValueRow)this._newPreValue).Controls.Add(txtNewValidation);

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

            // Write stored entries
            foreach (var s in this._preValues)
            {
                var editProperty = new Panel() { ID = "editProperty_" + s.Id.ToString(), CssClass = "editProperty" };

                var editDataType = ddlNewType.Items.FindByValue(s.DataTypeId.ToString());

                var editPropertyHeader = new Panel() { CssClass = "propertyHeader" };
                var editPropertyTitle = new HtmlGenericControl("h3")
                                            {
                                                InnerText =
                                                    string.Format(
                                                        "{0} ({1}), {2}: {3}",
                                                        s.Name.StartsWith("#")
                                                            ? uQuery.GetDictionaryItem(
                                                                s.Name.Substring(
                                                                    1, s.Name.Length - 1),
                                                                s.Name.Substring(
                                                                    1, s.Name.Length - 1))
                                                            : s.Name,
                                                        s.Alias,
                                                        uQuery.GetDictionaryItem("Type", "Type"),
                                                        editDataType != null
                                                            ? editDataType.Text
                                                            : "ERROR: This datatype is not supported")
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


                // MANDATORY
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

                // Instantiate controls
                var chkEditMandatory = new CheckBox() { ID = "editMandatory_" + this._preValues.IndexOf(s), CssClass = "editMandatory", Checked = s.Mandatory};
                var lblEditMandatory = new Label() { Text = Helper.Dictionary.GetDictionaryItem("Mandatory", "Mandatory"), CssClass = "label" };

                // Add controls to control
                editPropertyControls.Controls.Add(lblEditMandatory);
                editPropertyControls.Controls.Add(chkEditMandatory);
                s.Controls.Add(chkEditMandatory);
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "</li>" });


                // VISIBLE
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "<li>" });

                // Instantiate controls
                var chkEditVisible = new CheckBox() { ID = "editVisible_" + this._preValues.IndexOf(s), CssClass = "editVisible", Checked = s.Visible };
                var lblEditVisible = new Label() { Text = Helper.Dictionary.GetDictionaryItem("Visible", "Visible"), CssClass = "label" };

                // Add controls to control
                editPropertyControls.Controls.Add(lblEditVisible);
                editPropertyControls.Controls.Add(chkEditVisible);
                s.Controls.Add(chkEditVisible);
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
                editPropertyControls.Controls.Add(new LiteralControl() { Text = "<br/>" });
                editPropertyControls.Controls.Add(lnkEditValidation);
                s.Controls.Add(txtEditValidation);

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
            this.Controls.Add(this._readOnly);
            this.Controls.Add(this._tableHeight);
            this.Controls.Add(this._tableHeightValidator);
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
            writer.AddPrevalueRow(Helper.Dictionary.GetDictionaryItem("ShowLabel", "Show Label"), this._showLabel);
            writer.AddPrevalueRow(Helper.Dictionary.GetDictionaryItem("ShowGridHeader", "Show Grid Header"), this._showHeader);
            writer.AddPrevalueRow(Helper.Dictionary.GetDictionaryItem("ShowGridFooter", "Show Grid Footer"), this._showFooter);
            writer.AddPrevalueRow(Helper.Dictionary.GetDictionaryItem("ReadOnly", "Read Only"), this._readOnly);
            writer.AddPrevalueRow(Helper.Dictionary.GetDictionaryItem("TableHeight", "Table Height"), new Control[] { this._tableHeight, this._tableHeightValidator });
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
                var mandatory = t.Controls[3] != null ? ((CheckBox)t.Controls[3]).Checked : false;
                var visible = t.Controls[4] != null ? ((CheckBox)t.Controls[4]).Checked : true;
                var validation = t.Controls[5] != null ? ((TextBox)t.Controls[5]).Text : null;
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
                            Mandatory = mandatory,
                            Visible = visible,
                            ValidationExpression = validation,
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
    }
}
