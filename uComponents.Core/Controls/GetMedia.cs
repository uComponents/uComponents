using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;

namespace uComponents.Core.Controls
{
	/// <summary>
	/// GetMedia server control.
	/// </summary>
	[DefaultProperty("MediaId")]
	[ToolboxBitmap(typeof(ResourceExtensions), Settings.FaviconResourcePath)]
	[ToolboxData("<{0}:GetMedia runat=server MediaId=></{0}:GetMedia>")]
	public class GetMedia : WebControl
	{
		/// <summary>
		/// Gets or sets the media id.
		/// </summary>
		/// <value>The media id.</value>
		[DefaultValue("")]
		public string MediaId { get; set; }

		/// <summary>
		/// Renders the HTML opening tag of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			// base.RenderBeginTag(writer);
		}

		/// <summary>
		/// Renders the HTML closing tag of the control into the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		public override void RenderEndTag(HtmlTextWriter writer)
		{
			// base.RenderEndTag(writer);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!string.IsNullOrEmpty(this.MediaId))
			{
				var img = XsltExtensions.Media.GetImageHtml(this.MediaId, (int)this.Height.Value, (int)this.Width.Value);
				var ltrl = new Literal() { Text = img };

				this.Controls.Add(ltrl);
			}
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			base.RenderContents(writer);
		}
	}
}