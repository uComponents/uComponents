using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco;
using umbraco.cms.businesslogic.media;
using System.Text;

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

			this.Controls.Add(new Literal() { Text = this.ProcessMedia(this.MediaId) });
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
		/// Gets the HTML for file.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <returns></returns>
		private string GetHtmlForFile(Media media)
		{
			// check that its not null
			if (media != null)
			{
				// get the img src
				var file = media.GetProperty<string>(Constants.Umbraco.Media.File);

				// check that the file has a value
				if (!string.IsNullOrEmpty(file))
				{
					// define the anchor tag
					var anchor = "<a href=\"{0}\">{1}</a>";

					// format the anchor tag
					return string.Format(anchor, file, media.Text);
				}
			}

			// fall-back return an empty string
			return string.Empty;
		}

		/// <summary>
		/// Gets the HTML for folder.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <returns></returns>
		private string GetHtmlForFolder(Media media)
		{
			var html = new StringBuilder();

			html.AppendFormat("<dl><dt>{0}</dt>", media.Text);

			if (media.HasChildren)
			{
				foreach (var child in media.Children)
				{
					html.AppendFormat("<dd>{0}</dd>", this.ProcessMedia(child));
				}
			}

			html.Append("</dl>");

			return html.ToString();
		}

		/// <summary>
		/// Gets the HTML for an image tag, using the Media Id.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <returns>Returns a HTML image tag from a Media Id.</returns>
		private string GetHtmlForImage(Media media)
		{
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
						height = (int)this.Height.Value;
					}

					// get the width
					var width = media.GetProperty<int>(Constants.Umbraco.Media.Width);
					if (width <= 0)
					{
						width = (int)this.Width.Value;
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

		/// <summary>
		/// Processes the media.
		/// </summary>
		/// <returns></returns>
		private string ProcessMedia(string mediaId)
		{
			if (!string.IsNullOrEmpty(mediaId))
			{
				return this.ProcessMedia(uQuery.GetMedia(mediaId));
			}

			return string.Empty;
		}

		/// <summary>
		/// Processes the media.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <returns></returns>
		private string ProcessMedia(Media media)
		{
			if (media != null)
			{
				switch (media.ContentType.Alias.ToUpper())
				{
					case "FILE":
						return this.GetHtmlForFile(media);

					case "FOLDER":
						return this.GetHtmlForFolder(media);

					case "IMAGE":
						return this.GetHtmlForImage(media);

					default:
						// TODO: [LK] Implement a hook/handler for custom providers // e.g. Provider.GetHtml(media)
						break;
				}
			}

			return string.Empty;
		}
	}
}