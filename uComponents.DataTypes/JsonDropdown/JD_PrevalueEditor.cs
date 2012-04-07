using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.DataTypes.JsonDropdown
{
	/// <summary>
	/// The PreValue Editor for the JSON DropDown data-type.
	/// </summary>
	public class JD_PrevalueEditor : AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// TextBox control for the URL.
		/// </summary>
		private TextBox UrlTextBox;

		/// <summary>
		/// TextBox control for the expression.
		/// </summary>
		private TextBox ExpressionTextBox;

		/// <summary>
		/// TextBox control for the key.
		/// </summary>
		private TextBox KeyTextBox;

		/// <summary>
		/// TextBox control for the value.
		/// </summary>
		private TextBox ValueTextbox;

		/// <summary>
		/// Initializes a new instance of the <see cref="JD_PrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public JD_PrevalueEditor(BaseDataType dataType)
			: base(dataType, DBTypes.Ntext)
		{
		}

		/// <summary>
		/// Saves the data-type PreValue options.
		/// </summary>
		public override void Save()
		{
			// set the options
			var options = new JD_Options
			{
				UrlToJson = this.UrlTextBox.Text,
				Expression = this.ExpressionTextBox.Text,
				Key = this.KeyTextBox.Text,
				Value = this.ValueTextbox.Text
			};

			// save the options as JSON
			this.SaveAsJson(options);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();
		}

		/// <summary>
		/// Creates child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// set-up child controls
			this.UrlTextBox = new TextBox { ID = "UrlTextBox", CssClass = "guiInputText umbEditorTextField" };
			this.ExpressionTextBox = new TextBox { ID = "ExpressionTextBox", CssClass = "guiInputText" };
			this.KeyTextBox = new TextBox { ID = "KeyTextBox", CssClass = "guiInputText" };
			this.ValueTextbox = new TextBox { ID = "ValueTextbox", CssClass = "guiInputText" };

			// add the child controls
			this.Controls.AddPrevalueControls(this.UrlTextBox, this.ExpressionTextBox, this.KeyTextBox, this.ValueTextbox);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// get PreValues, load them into the controls.
			var options = this.GetPreValueOptions<JD_Options>();

			// if the options are null, then load the defaults
			if (options == null)
			{
				options = new JD_Options(true);
			}

			// set the values
			this.UrlTextBox.Text = options.UrlToJson;
			this.ExpressionTextBox.Text = options.Expression;
			this.KeyTextBox.Text = options.Key;
			this.ValueTextbox.Text = options.Value;
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			// add property fields
			writer.AddPrevalueRow("Url to JSON data", "Url string to JSON data", this.UrlTextBox);
			writer.AddPrevalueRow("jQuery Expression", "jQuery Expression", this.ExpressionTextBox);
			writer.AddPrevalueRow("The key data for dropdown list", "The key data for dropdown list", this.KeyTextBox);
			writer.AddPrevalueRow("The value data for dropdown list", "The key data for dropdown list", this.ValueTextbox);
		}
	}
}