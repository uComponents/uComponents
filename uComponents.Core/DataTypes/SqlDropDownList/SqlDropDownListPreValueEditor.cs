using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.XPath;

using uComponents.Core.Shared;
using uComponents.Core.Shared.PrevalueEditors;

using umbraco.cms.businesslogic.datatype;
using umbraco.DataLayer;

namespace uComponents.Core.DataTypes.SqlDropDownList
{
	class SqlDropDownListPreValueEditor : AbstractJsonPrevalueEditor
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
		public SqlDropDownListPreValueEditor(BaseDataType dataType)
			: base(dataType, DBTypes.Nvarchar)
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

			this.connectionStringTextBox.ID = "connectionStringTextBox";
			this.connectionStringTextBox.Columns = 120;
			this.connectionStringTextBox.TextMode = TextBoxMode.SingleLine;
			
			this.Controls.Add(this.sqlTextBox);
			this.Controls.Add(this.sqlRequiredFieldValidator);
			this.Controls.Add(this.sqlCustomValidator);
			this.Controls.Add(this.connectionStringTextBox);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.sqlTextBox.Text = this.Options.Sql;
			this.connectionStringTextBox.Text = this.Options.ConnectionString;
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
				// TODO:

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
			writer.AddPrevalueRow("Connection String", "(optional)", this.connectionStringTextBox);
		}

	}
}