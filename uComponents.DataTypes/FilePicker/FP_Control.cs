using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using umbraco;

namespace uComponents.DataTypes.FilePicker
{
	/// <summary>
	/// The FilePicker control.
	/// </summary>
	[ValidationProperty("Text")]
	public class FP_Control : PlaceHolder
	{
		/// <summary>
		/// Gets or sets the selected directory.
		/// </summary>
		/// <value>The selected directory.</value>
		public string SelectedDirectory { get; set; }

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
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.EnsureChildControls();

			// create the controls
			this.TextBoxControl.ID = this.TextBoxControl.ClientID;
			this.TextBoxControl.Width = Unit.Pixel(330);
			this.TextBoxControl.CssClass = "guiInputText";

			// add the controls
			this.Controls.Add(this.TextBoxControl);

			// create the image
			HtmlImage image = new HtmlImage() { Src = string.Concat(GlobalSettings.Path, "/images/foldericon.png") };
			image.Style.Add("padding-left", "5px");

			// create the anchor link
			HtmlAnchor anchor = new HtmlAnchor() { HRef = "javascript:void(0);" };
			anchor.Attributes.Add("onclick", string.Format("javascript:UmbClientMgr.openModalWindow('{0}/developer/packages/directoryBrowser.aspx?target={1}&path={2}', 'Choose a file or a folder', true, 400, 500, 0, 0); return false;", GlobalSettings.Path, this.TextBoxControl.ClientID, this.SelectedDirectory));

			// add the image to the anchor link
			anchor.Controls.Add(image);

			// add the anchor link to the data-type property
			this.Controls.Add(anchor);
		}
	}
}
