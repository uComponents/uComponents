using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;

namespace uComponents.DataTypes.ToggleBox
{
	/// <summary>
	/// The PreValue Editor for the ToggleBox data-type.
	/// </summary>
	public class TB_PrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// The CheckBox control for the default value ('on' or 'off').
		/// </summary>
		private CheckBox DefaultValue;

		/// <summary>
		/// The TextBox control for the background-color of the 'off' state.
		/// </summary>
		/// <remarks>This will be changed to a color-picker in the next version. [LK]</remarks>
		private TextBox OffBackgroundColor;

		/// <summary>
		/// The TextBox control for the label of the 'off' state.
		/// </summary>
		private TextBox OffText;

		/// <summary>
		/// The TextBox control for the background-color of the 'on' state.
		/// </summary>
		/// <remarks>This will be changed to a color-picker in the next version. [LK]</remarks>
		private TextBox OnBackgroundColor;

		/// <summary>
		/// The TextBox control for the label of the 'on' state.
		/// </summary>
		private TextBox OnText;

        /// <summary>
        /// Gets the documentation URL.
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return string.Concat(base.DocumentationUrl, "/data-types/toggle-box/");
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="TB_PrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public TB_PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
			: base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Integer)
		{
		}

		/// <summary>
		/// Saves the data-type PreValue options.
		/// </summary>
		public override void Save()
		{
			// set the options
			var options = new TB_Options()
			{
				DefaultValue = this.DefaultValue.Checked,
				OffBackgroundColor = this.OffBackgroundColor.Text,
				OffText = this.OffText.Text,
				OnBackgroundColor = this.OnBackgroundColor.Text,
				OnText = this.OnText.Text
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

			this.RegisterEmbeddedClientResource("uComponents.DataTypes.Shared.Resources.Scripts.mColorPicker.js", ClientDependencyType.Javascript);
		}

		/// <summary>
		/// Creates child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// set-up child controls
			this.DefaultValue = new CheckBox() { ID = "DefaultValue" };

			this.OffBackgroundColor = new TextBox() { ID = "OffBackgroundColor", CssClass = "guiInputText" };
			this.OffBackgroundColor.Attributes.Add("type", "color");
			this.OffBackgroundColor.Attributes.Add("data-hex", "true");

			this.OffText = new TextBox() { ID = "OffText", CssClass = "guiInputText" };

			this.OnBackgroundColor = new TextBox() { ID = "OnBackgroundColor", CssClass = "guiInputText" };
			this.OnBackgroundColor.Attributes.Add("type", "color");
			this.OnBackgroundColor.Attributes.Add("data-hex", "true");

			this.OnText = new TextBox() { ID = "OnText", CssClass = "guiInputText" };

			// add the child controls
			this.Controls.AddPrevalueControls(this.DefaultValue, this.OffBackgroundColor, this.OffText, this.OnBackgroundColor, this.OnText);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// get PreValues, load them into the controls.
			var options = this.GetPreValueOptions<TB_Options>();

			// if the options are null, then load the defaults
			if (options == null)
			{
				options = new TB_Options(true);
			}

			// set the values
			this.DefaultValue.Checked = options.DefaultValue;
			this.OffBackgroundColor.Text = options.OffBackgroundColor;
			this.OffText.Text = options.OffText;
			this.OnBackgroundColor.Text = options.OnBackgroundColor;
			this.OnText.Text = options.OnText;
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			// add property fields
			writer.AddPrevalueRow("'On' label", "The label text for the true (on) state", this.OnText);
			writer.AddPrevalueRow("'On' background", "The background-color for the true (on) state", this.OnBackgroundColor);
			writer.AddPrevalueRow("'Off' label", "The label text for the false (off) state", this.OffText);
			writer.AddPrevalueRow("'Off' background", "The background-color for the false (off) state", this.OffBackgroundColor);
			writer.AddPrevalueRow("Default value", "The default value for the ToggleBox, either 'on' or 'off'", this.DefaultValue);
		}
	}
}
