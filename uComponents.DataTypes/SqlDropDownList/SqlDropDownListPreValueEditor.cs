using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;
using System.Configuration;

namespace uComponents.DataTypes.SqlDropDownList
{
	class SqlDropDownListPreValueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
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
        /// drop down list of all web.config connection strings strings + default of the umbraco app setting connection string
        /// </summary>
        private DropDownList connectionStringDropDownList = new DropDownList();

		/// <summary>
		/// Data object used to define the configuration status of this PreValueEditor
		/// </summary>
		private SqlDropDownListOptions options = null;

		/// <summary>
		/// Gets the options data object that represents the current state of this datatypes configuration
		/// </summary>
		internal SqlDropDownListOptions Options
		{
			get
			{
				if (this.options == null)
				{
					// Deserialize any stored settings for this PreValueEditor instance
					this.options = this.GetPreValueOptions<SqlDropDownListOptions>();

					// If still null, ie, object couldn't be de-serialized from PreValue[0] string value
					if (this.options == null)
					{
						// Create a new Options data object with the default values
						this.options = new SqlDropDownListOptions();
					}
				}
				return this.options;
			}
		}

		/// <summary>
		/// Initialize a new instance of XPathCheckBoxlistPreValueEditor
		/// </summary>
		/// <param name="dataType">XPathCheckBoxListDataType</param>
		public SqlDropDownListPreValueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
			: base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Nvarchar)
		{
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
			//this.sqlTextBox.CssClass = "umbEditorTextField";

			this.sqlRequiredFieldValidator.ControlToValidate = this.sqlTextBox.ID;
			this.sqlRequiredFieldValidator.Display = ValidatorDisplay.Dynamic;
			this.sqlRequiredFieldValidator.ErrorMessage = " SQL expression required";

			this.sqlCustomValidator.ControlToValidate = this.sqlTextBox.ID;
			this.sqlCustomValidator.Display = ValidatorDisplay.Dynamic;
			this.sqlCustomValidator.ServerValidate += new ServerValidateEventHandler(this.SqlCustomValidator_ServerValidate);

            this.connectionStringDropDownList.ID = "connectionStringDeopDownList";
            this.connectionStringDropDownList.Items.Add(new ListItem("Umbraco (default)", string.Empty));

            foreach (ConnectionStringSettings connectionStringSettings in ConfigurationManager.ConnectionStrings)
            {
                this.connectionStringDropDownList.Items.Add(new ListItem(connectionStringSettings.Name, connectionStringSettings.Name));
            }
			
			this.Controls.Add(this.sqlTextBox);
			this.Controls.Add(this.sqlRequiredFieldValidator);
			this.Controls.Add(this.sqlCustomValidator);
			this.Controls.Add(this.connectionStringDropDownList);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.sqlTextBox.Text = this.Options.Sql;

            ListItem selectListItem = this.connectionStringDropDownList.Items.FindByValue(this.options.ConnectionStringName);
            if (selectListItem != null)
            {
                selectListItem.Selected = true;
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
				this.Options.ConnectionStringName = this.connectionStringDropDownList.SelectedValue;

				this.SaveAsJson(this.Options);  // Serialize to Umbraco database field
			}
		}

		/// <summary>
		/// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
		/// </summary>
		/// <param name="writer"></param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow("SQL Expression", this.sqlTextBox, this.sqlRequiredFieldValidator, this.sqlCustomValidator);
            writer.AddPrevalueRow("Connection String", "add items to the web.config &lt;connectionStrings /&gt; section to list here", this.connectionStringDropDownList);
		}

	}
}