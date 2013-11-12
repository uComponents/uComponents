using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.XPath;
using umbraco;
using umbraco.editorControls;

namespace uComponents.DataTypes.XPathChildNodePicker
{
	/// <summary>
	/// This Prevalue Editor will require an XPath expression to define the nodes to pick as CheckBox options.
	/// </summary>
	public class XPathChildNodePickerPrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// TextBox control to get the XPath expression
		/// </summary>
		private TextBox XPathTextBox;

		/// <summary>
		/// The DropDownList for the ListControl types.
		/// </summary>
		private DropDownList ListControlTypes;

		/// <summary>
		/// RequiredFieldValidator to ensure an XPath expression has been entered
		/// </summary>
		private RequiredFieldValidator XPathRequiredFieldValidator;

		/// <summary>
		/// Server side validation of XPath expression, to ensure some nodes are returned
		/// </summary>
		private CustomValidator XPathCustomValidator;

		/// <summary>
		/// Data object used to define the configuration status of this PreValueEditor
		/// </summary>
		private XPathChildNodePickerOptions options = null;

		/// <summary>
		/// Gets the options data object that represents the current state of this datatypes configuration
		/// </summary>
		internal XPathChildNodePickerOptions Options
		{
			get
			{
				if (this.options == null)
				{
					// Deserialize any stored settings for this PreValueEditor instance
					this.options = this.GetPreValueOptions<XPathChildNodePickerOptions>();

					// If still null, ie, object couldn't be de-serialized from PreValue[0] string value
					if (this.options == null)
					{
						// Create a new Options data object with the default values
						this.options = new XPathChildNodePickerOptions();
					}
				}

				return this.options;
			}
		}

		/// <summary>
		/// Initialize a new instance of XPathChildNodePickerPrevalueEditor
		/// </summary>
		/// <param name="dataType">XPathChildNodePickerDataType</param>
		public XPathChildNodePickerPrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
			: base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
		{
		}

		/// <summary>
		/// Creates all of the controls and assigns all of their properties
		/// </summary>
		protected override void CreateChildControls()
		{
			this.XPathTextBox = new TextBox()
			{
				ID = "xPathTextBox",
				CssClass = "umbEditorTextField"
			};

			this.XPathRequiredFieldValidator = new RequiredFieldValidator()
			{
				ControlToValidate = this.XPathTextBox.ID,
				Display = ValidatorDisplay.Dynamic,
				ErrorMessage = " XPath expression required"
			};

			this.XPathCustomValidator = new CustomValidator()
			{
				ControlToValidate = this.XPathTextBox.ID,
				Display = ValidatorDisplay.Dynamic
			};
			this.XPathCustomValidator.ServerValidate += new ServerValidateEventHandler(this.XPathCustomValidator_ServerValidate);

			this.ListControlTypes = new DropDownList()
			{
				ID = "ListControlTypes",
				DataSource = Enum.GetValues(typeof(XPathChildNodePickerOptions.ListControlTypes))
			};
			this.ListControlTypes.DataBind();

			this.Controls.AddPrevalueControls(
				this.XPathTextBox,
				this.XPathRequiredFieldValidator,
				this.XPathCustomValidator,
				this.ListControlTypes);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.XPathTextBox.Text = this.Options.XPath;
			this.ListControlTypes.SelectedIndex = (int)this.Options.ListControlType;
		}

		/// <summary>
		/// Will run the entered XPath expression to ensure it returns at least 1 node
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		private void XPathCustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
		{
			var xpath = e.Value;
			var isValid = false;

			try
			{
				if (uQuery.GetNodesByXPath(xpath).Count() >= 0)
				{
					isValid = true;
				}
			}
			catch (XPathException)
			{
				this.XPathCustomValidator.ErrorMessage = " Syntax error in XPath expression";
			}

			e.IsValid = isValid;
		}

		/// <summary>
		/// Saves the pre value data to Umbraco
		/// </summary>
		public override void Save()
		{
			if (this.Page.IsValid)
			{
				this.Options.XPath = this.XPathTextBox.Text;
				this.Options.ListControlType = (XPathChildNodePickerOptions.ListControlTypes)this.ListControlTypes.SelectedIndex;

				this.SaveAsJson(this.Options); // Serialize to Umbraco database field
			}
		}

		/// <summary>
		/// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
		/// </summary>
		/// <param name="writer"></param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow("XPath expression", this.XPathTextBox, this.XPathRequiredFieldValidator, this.XPathCustomValidator);
			writer.AddPrevalueRow("List type", this.ListControlTypes);
		}
	}
}
