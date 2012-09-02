using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.editorControls;

namespace uComponents.DataTypes.SqlAutoComplete
{
    public class SqlAutoCompletePreValueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
    {
        /// <summary>
        /// TextBox control to get the Sql expression
        /// </summary>
        private TextBox sqlTextBox = new TextBox();

        /// <summary>
        /// RequiredFieldValidator to ensure a Sql expression has been entered
        /// </summary>
        private RequiredFieldValidator sqlRequiredFieldValidator = new RequiredFieldValidator();

        /// <summary>
        /// Server side validation of Sql expression
        /// </summary>
        private CustomValidator sqlCustomValidator = new CustomValidator();

        /// <summary>
        /// optional connection string (if not specified then the current umbraco db connection string is used
        /// </summary>
        private TextBox connectionStringTextBox = new TextBox();

        /// <summary>
        /// Number of characters before data is requested (useful if the list size should visibily shrink as the data set narrows - else use a SELECT TOP x FROM .... clause)
        /// valid range 0 to 5 (where 0 if off - so sending all the data on initial load)
        /// </summary>
        private DropDownList minLengthDropDownList = new DropDownList();

        /// <summary>
        /// Data object used to define the configuration status of this PreValueEditor
        /// </summary>
        private SqlAutoCompleteOptions options = null;

        /// <summary>
        /// Initialize a new instance of SqlAutoCompletePreValueEditor
        /// </summary>
        /// <param name="dataType">SqlAutoCompleteDataType</param>
        public SqlAutoCompletePreValueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
            : base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
        {
        }

        /// <summary>
        /// Gets the options data object that represents the current state of this datatypes configuration
        /// </summary>
        internal SqlAutoCompleteOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    // Deserialize any stored settings for this PreValueEditor instance
                    this.options = this.GetPreValueOptions<SqlAutoCompleteOptions>();

                    // If still null, ie, object couldn't be de-serialized from PreValue[0] string value
                    if (this.options == null)
                    {
                        // Create a new Options data object with the default values
                        this.options = new SqlAutoCompleteOptions(true);
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
            this.sqlTextBox.ID = "sqlTextBox";
            this.sqlTextBox.TextMode = TextBoxMode.MultiLine;
            this.sqlTextBox.Rows = 10;
            this.sqlTextBox.Columns = 80;
            // this.sqlTextBox.CssClass = "umbEditorTextField";

            this.sqlRequiredFieldValidator.ControlToValidate = this.sqlTextBox.ID;
            this.sqlRequiredFieldValidator.Display = ValidatorDisplay.Dynamic;
            this.sqlRequiredFieldValidator.ErrorMessage = " SQL expression required";

            this.sqlCustomValidator.ControlToValidate = this.sqlTextBox.ID;
            this.sqlCustomValidator.Display = ValidatorDisplay.Dynamic;
            this.sqlCustomValidator.ServerValidate += new ServerValidateEventHandler(this.SqlCustomValidator_ServerValidate);

            this.connectionStringTextBox.ID = "connectionStringTextBox";
            this.connectionStringTextBox.Columns = 120;
            this.connectionStringTextBox.TextMode = TextBoxMode.SingleLine;

            this.minLengthDropDownList.ID = "minLengthDropDownList";
            this.minLengthDropDownList.Items.Add(new ListItem("1", "1"));
            this.minLengthDropDownList.Items.Add(new ListItem("2", "2"));
            this.minLengthDropDownList.Items.Add(new ListItem("3 (default)", "3"));
            this.minLengthDropDownList.Items.Add(new ListItem("4", "4"));
            this.minLengthDropDownList.Items.Add(new ListItem("5", "5"));

            // TODO: potential feature ?
            //this.minLengthDropDownList.Items.Add(new ListItem("First Space", "first-space"));

            this.Controls.AddPrevalueControls(
                this.sqlTextBox,
                this.sqlRequiredFieldValidator,
                this.sqlCustomValidator,
                this.connectionStringTextBox,
                this.minLengthDropDownList);
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
                this.sqlTextBox.Text = this.Options.Sql;
                this.connectionStringTextBox.Text = this.Options.ConnectionString;
                this.minLengthDropDownList.SelectedIndex = this.minLengthDropDownList.Items.IndexOf(this.minLengthDropDownList.Items.FindByValue(this.Options.MinLength.ToString()));
            }
        }

        /// <summary>
        /// Will run the entered Sql expression to ensure it is valid
        /// </summary>
        /// <param name="source">xPathCustomValidator</param>
        /// <param name="args"></param>
        private void SqlCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string sql = args.Value;
            bool isValid = false;

            try
            {
                // TODO: [HR] SqlCustomValidator_ServerValidate

                isValid = true;
            }
            catch
            {
                this.sqlCustomValidator.ErrorMessage = " Error in SQL expression";
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
                this.Options.Sql = this.sqlTextBox.Text;
                this.Options.ConnectionString = this.connectionStringTextBox.Text;
                this.Options.MinLength = int.Parse(this.minLengthDropDownList.SelectedValue);

                this.SaveAsJson(this.Options);  // Serialize to Umbraco database field
            }
        }

        /// <summary>
        /// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("SQL Expression", @" expects a result set with two fields : 'Text' and 'Value' - can include the tokens : $currentId and $autcompleteText", this.sqlTextBox, this.sqlRequiredFieldValidator, this.sqlCustomValidator);
            writer.AddPrevalueRow("Connection String", "(optional) if empty then the current umbraco connection string is used", this.connectionStringTextBox);
            writer.AddPrevalueRow("Min Length", "number of chars in the autocomplete text box before querying for data", this.minLengthDropDownList);
        }
    }
}
