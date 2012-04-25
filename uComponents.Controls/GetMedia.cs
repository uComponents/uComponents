using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco;

namespace uComponents.Controls
{
	/// <summary>
	/// GetMedia server control.
	/// </summary>
	[DefaultProperty("MediaId")]
	[ToolboxBitmap(typeof(Constants), Constants.FaviconResourcePath)]
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
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!string.IsNullOrEmpty(this.MediaId))
			{
				var img = this.GetImageHtml(this.MediaId, (int)this.Height.Value, (int)this.Width.Value);
				var ltrl = new Literal() { Text = img };

				this.Controls.Add(ltrl);
			}
		}

		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			base.RenderContents(writer);
		}

		/// <summary>
		/// Gets the HTML for an image tag, using the Media Id.
		/// </summary>
		/// <param name="mediaId">The media id.</param>
		/// <param name="defaultHeight">The default height.</param>
		/// <param name="defaultWidth">The default width.</param>
		/// <returns>Returns a HTML image tag from a Media Id.</returns>
		private string GetImageHtml(string mediaId, int defaultHeight, int defaultWidth)
		{
			// get the media item
			var media = uQuery.GetMedia(mediaId);

			// check that its not null
			if (media != null)
			{
				// get the img src
				var src = media.GetProperty<string>(Constants.Umbraco.Media.File);

				// check that the img src has a value
				if (!string.IsNullOrEmpty(src))
				{
					// define the img tag
					var img = "<img src=\"{0}\" alt=\"{1}\" height=\"{2}\" width=\"{3}\" />";

					// get the height
					var height = media.GetProperty<int>(Constants.Umbraco.Media.Height);
					if (height <= 0)
					{
						height = defaultHeight;
					}

					// get the width
					var width = media.GetProperty<int>(Constants.Umbraco.Media.Width);
					if (width <= 0)
					{
						width = defaultWidth;
					}

					// if the default dimensions are less then zero...
					if (height <= 0 && width <= 0)
					{
						// default the height & width to 100px;
						height = 100;
						width = 100;
					}

					// format the img tag
					return string.Format(img, src, media.Text, height, width);
				}
			}

			// fall-back return an empty string
			return string.Empty;
		}
	}
}