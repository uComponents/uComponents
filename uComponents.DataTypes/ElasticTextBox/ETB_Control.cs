using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClientDependency.Core;
using uComponents.Core;
using uComponents.Core.Extensions;

[assembly: WebResource("uComponents.DataTypes.ElasticTextBox.Scripts.jquery.elastic.js", Constants.MediaTypeNames.Application.JavaScript)]

namespace uComponents.DataTypes.ElasticTextBox
{
	/// <summary>
	/// The ElasticTextBox data type lets a textarea grown and shrink to fit it's content.
	/// </summary>
	[ValidationProperty("Text")]
	public class ETB_Control : PlaceHolder
	{
		/// <summary>
		/// Gets or sets the options.
		/// </summary>
		/// <value>The options.</value>
		public ETB_Options Options { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text for the TextBoxControl.</value>
		public string Text
		{
			get
			{
				return this.TextBoxControl.Text;
			}

			set
			{
				if (this.TextBoxControl == null)
				{
					this.TextBoxControl = new TextBox();
				}

				this.TextBoxControl.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the TextBox control.
		/// </summary>
		/// <value>The text box control.</value>
		protected TextBox TextBoxControl { get; set; }

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
			this.AddResourceToClientDependency("uComponents.DataTypes.ElasticTextBox.Scripts.jquery.elastic.js", ClientDependencyType.Javascript);
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.EnsureChildControls();

			// create the controls
			this.TextBoxControl.ID = this.TextBoxControl.ClientID;
			this.TextBoxControl.TextMode = TextBoxMode.MultiLine;
			this.TextBoxControl.Attributes.Add("style", this.Options.Css);
			this.TextBoxControl.Height = Unit.Pixel(this.Options.Height);
			this.TextBoxControl.Width = Unit.Pixel(this.Options.Width);
			this.TextBoxControl.CssClass = "umbEditorTextFieldMultiple";

			// add the controls
			this.Controls.Add(this.TextBoxControl);
		}

		/// <summary>
		/// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute("class", "ElasticTextBox");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			this.TextBoxControl.RenderControl(writer);

			writer.RenderEndTag(); // ElasticTextBox

			// add jquery window load event to create the js elastic
			var javascriptMethod = string.Format("jQuery('#{0}').elastic();", this.TextBoxControl.ClientID);
			var javascript = string.Concat("<script type='text/javascript'>jQuery(window).load(function(){", javascriptMethod, "});</script>");
			writer.WriteLine(javascript);
		}
	}
}
