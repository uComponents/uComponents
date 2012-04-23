using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClientDependency.Core;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;

[assembly: WebResource("uComponents.DataTypes.ToggleBox.Scripts.jquery.togglebox.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.ToggleBox.Styles.ToggleBox.css", Constants.MediaTypeNames.Text.Css, PerformSubstitution = true)]
[assembly: WebResource("uComponents.DataTypes.ToggleBox.Images.shadow-bg.png", Constants.MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.DataTypes.ToggleBox.Images.handle-bg.png", Constants.MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.DataTypes.ToggleBox.Images.handle-left-bg.png", Constants.MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.DataTypes.ToggleBox.Images.handle-right-bg.png", Constants.MediaTypeNames.Image.Png)]

namespace uComponents.DataTypes.ToggleBox
{
	/// <summary>
	/// A ToggleBox control for Umbraco.
	/// http://code.google.com/p/togglebox/
	/// </summary>
	public class TB_Control : PlaceHolder
	{
		/// <summary>
		/// Gets or sets the options.
		/// </summary>
		/// <value>The options.</value>
		public TB_Options Options { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TB_Control"/> is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public bool Checked
		{
			get
			{
				return this.CheckBoxControl.Checked;
			}

			set
			{
				if (this.CheckBoxControl == null)
				{
					this.CheckBoxControl = new CheckBox();
				}

				this.CheckBoxControl.Checked = value;
			}
		}

		/// <summary>
		/// Gets or sets the CheckBox control.
		/// </summary>
		/// <value>The CheckBox control.</value>
		protected CheckBox CheckBoxControl { get; set; }

		/// <summary>
		/// Initialize the control, make sure children are created
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();
		}

		/// <summary>
		/// Add the resources (sytles/scripts)
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// Adds the client dependencies.
			this.AddResourceToClientDependency("uComponents.DataTypes.ToggleBox.Scripts.jquery.togglebox.js", ClientDependencyType.Javascript);
			this.AddResourceToClientDependency("uComponents.DataTypes.ToggleBox.Styles.ToggleBox.css", ClientDependencyType.Css);
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.EnsureChildControls();

			if (this.CheckBoxControl == null)
			{
				this.CheckBoxControl = new CheckBox();
				this.CheckBoxControl.Checked = this.Options.DefaultValue;
			}

			this.CheckBoxControl.ID = this.CheckBoxControl.ClientID;

			// add the control
			this.Controls.Add(this.CheckBoxControl);
		}

		/// <summary>
		/// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute("id", this.ClientID);
			writer.AddAttribute("class", "ToggleBox");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			this.CheckBoxControl.RenderControl(writer);

			writer.RenderEndTag();

			// construct slider options
			StringBuilder options = new StringBuilder();

			if (!string.IsNullOrEmpty(this.Options.OnText))
			{
				options.Append(" on_label: '").Append(this.Options.OnText).Append("'").Append(Constants.Common.COMMA);
			}

			if (!string.IsNullOrEmpty(this.Options.OnBackgroundColor))
			{
				options.Append(" on_bg_color: '").Append(this.Options.OnBackgroundColor).Append("'").Append(Constants.Common.COMMA);
			}

			if (!string.IsNullOrEmpty(this.Options.OffText))
			{
				options.Append(" off_label: '").Append(this.Options.OffText).Append("'").Append(Constants.Common.COMMA);
			}

			if (!string.IsNullOrEmpty(this.Options.OffBackgroundColor))
			{
				options.Append(" off_bg_color: '").Append(this.Options.OffBackgroundColor).Append("'").Append(Constants.Common.COMMA);
			}

			// add jquery window load event to create the ToggleBox
			var javascriptMethod = string.Format("jQuery('#{0}').checkToggle({{ {1} bypass_skin: true }});", this.CheckBoxControl.ClientID, options);
			var javascript = string.Concat("<script type='text/javascript'>jQuery(window).load(function(){ ", javascriptMethod, " });</script>");
			writer.WriteLine(javascript);
		}
	}
}
