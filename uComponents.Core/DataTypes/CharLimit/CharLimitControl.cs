using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ClientDependency.Core;
using uComponents.Core.Shared;

[assembly: WebResource("uComponents.Core.DataTypes.CharLimit.CharLimit.js", MediaTypeNames.Application.JavaScript)]

namespace uComponents.Core.DataTypes.CharLimit
{
	/// <summary>
	/// The CharLimit control sets a character limit on a TextBox.
	/// </summary>
	[ValidationProperty("Text")]
	public class CharLimitControl : PlaceHolder
    {
		/// <summary>
		/// Gets or sets the options.
		/// </summary>
		/// <value>The options.</value>
		public CharLimitOptions Options { get; set; }

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
			this.AddResourceToClientDependency("uComponents.Core.DataTypes.CharLimit.CharLimit.js", ClientDependencyType.Javascript);
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.EnsureChildControls();

			// create the controls
			if (this.TextBoxControl == null)
			{
				this.TextBoxControl = new TextBox();
			}

			// set the control's attributes
			this.TextBoxControl.ID = this.TextBoxControl.ClientID;
			this.TextBoxControl.TextMode = this.Options.TextBoxMode;
			this.TextBoxControl.CssClass = this.Options.TextBoxMode == TextBoxMode.SingleLine ? "CharLimit umbEditorTextField" : "CharLimit umbEditorTextFieldMultiple";
			this.TextBoxControl.Attributes.Add("rel", this.Options.Limit.ToString());
            this.TextBoxControl.Attributes.Add("enforce", this.Options.EnforceCharLimit.ToString());

			// add the controls
			this.Controls.Add(this.TextBoxControl);
		}

		/// <summary>
		/// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			this.TextBoxControl.RenderControl(writer);

            String message = String.Format("You have {0} characters left.", this.Options.Limit - this.Text.Length);

            if(!this.Options.EnforceCharLimit && this.Text.Length > this.Options.Limit){
                message = String.Format("You have gone past the limit of {0}. Total: {1} characters.", this.Options.Limit, this.Text.Length);
            }
            // in the case where they've Not enforced the character limit, entered too many characters, and then turned 'enforce' on.
            else if (this.Options.EnforceCharLimit && this.Text.Length > this.Options.Limit)
            {
                message = String.Format("You used {0} characters. You are only allowed {1} characters.", this.Text.Length, this.Options.Limit);
            }

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "CharLimitStatus");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.Write(message);
			writer.RenderEndTag(); // .CharLimitStatus

			writer.RenderEndTag(); // .CharLimit

			// add jquery window load event
			var javascriptMethod = string.Format("jQuery('#{0}').CharLimit();", this.ClientID);
			var javascript = string.Concat("<script type='text/javascript'>jQuery(window).load(function(){", javascriptMethod, "});</script>");
			writer.WriteLine(javascript);
		}
    }
}