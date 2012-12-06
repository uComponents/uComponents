using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using uComponents.Core;
using umbraco;
using umbraco.IO;
using System;

namespace uComponents.XsltExtensions
{
	/// <summary>
	/// The Media class exposes XSLT extensions to access media from Umbraco.
	/// </summary>
	[XsltExtension("ucomponents.media")]
	public class Media
	{
		/// <summary>
		/// Gets the media by CSV.
		/// </summary>
		/// <param name="csv">The CSV of media IDs.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the media nodes from the CSV list.
		/// </returns>
		public static XPathNodeIterator GetMediaByCsv(string csv)
		{
			return GetMediaByCsv(csv, false);
		}

		/// <summary>
		/// Gets the media by CSV.
		/// </summary>
		/// <param name="csv">The CSV of media IDs.</param>
		/// <param name="deep">if set to <c>true</c> [deep].</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the media nodes from the CSV list.
		/// </returns>
		public static XPathNodeIterator GetMediaByCsv(string csv, bool deep)
		{
			var xd = new XmlDocument();
			xd.LoadXml("<root/>");

			var mediaNodes = uQuery.GetMediaByCsv(csv);

			foreach (var mediaNode in mediaNodes)
			{
				var xn = mediaNode.ToXml(xd, deep);
				xd.DocumentElement.AppendChild(xn);
			}

			return xd.CreateNavigator().Select("descendant::*[@id]");
		}

		/// <summary>
		/// Gets the media item by it's node name.
		/// </summary>
		/// <param name="nodeName">Name of the node.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the media nodes from specified node name.
		/// </returns>
		public static XPathNodeIterator GetMediaByName(string nodeName)
		{
			var xpath = string.Concat("descendant::*[@nodeName='", nodeName, "']");
			return GetMediaByXPath(xpath);
		}

		/// <summary>
		/// Gets the media items by node type alias.
		/// </summary>
		/// <param name="nodeTypeAlias">The node type alias.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the media nodes from specified node type alias.
		/// </returns>
		public static XPathNodeIterator GetMediaByType(string nodeTypeAlias)
		{
			var xpath = string.Concat("descendant::*[@nodeTypeAlias='", nodeTypeAlias, "']");
			return GetMediaByXPath(xpath);
		}

		/// <summary>
		/// Gets the media items by URL name.
		/// </summary>
		/// <param name="urlName">The URL name of the media item.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the media nodes from specified URL name.
		/// </returns>
		public static XPathNodeIterator GetMediaByUrlName(string urlName)
		{
			var xpath = string.Concat("descendant::*[@urlName='", urlName, "']");
			return GetMediaByXPath(xpath);
		}

		/// <summary>
		/// Gets the media items by an XPath expression.
		/// </summary>
		/// <param name="xpath">The XPath expression.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the media nodes from specified XPath expression.
		/// </returns>
		public static XPathNodeIterator GetMediaByXPath(string xpath)
		{
			var xml = uQuery.GetPublishedXml(uQuery.UmbracoObjectType.Media);
			if (xml != null)
			{
				var nav = xml.CreateNavigator();
				return nav.Select(xpath);
			}

			return null;
		}

		/// <summary>
		/// Gets the media Id by URL.
		/// </summary>
		/// <param name="url">The URL to get the media id from.</param>
		/// <returns>Returns the media Id.</returns>
		public static int GetMediaIdByUrl(string url)
		{
			if (!string.IsNullOrWhiteSpace(url))
			{
				var mediaPath = IOHelper.ResolveUrl(SystemDirectories.Media);
				if (url.StartsWith(mediaPath) && url.Length > mediaPath.Length)
				{
					var parts = url.Substring(mediaPath.Length).Split(new[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
					int propertyId;

					if (parts.Length > 1 && int.TryParse(parts[1], out propertyId))
					{
						return Cms.GetContentIdByPropertyId(propertyId);
					}
				}
			}

			return uQuery.RootNodeId;
		}

		/// <summary>
		/// Gets the published Xml.
		/// </summary>
		/// <returns>
		/// Returns an XPathNodeIterator of all the media nodes.
		/// </returns>
		public static XPathNodeIterator GetPublishedXml()
		{
			return GetMediaByXPath("/");
		}

		/// <summary>
		/// Gets the unique id of a media node.
		/// </summary>
		/// <param name="mediaId">The media id.</param>
		/// <returns>Returns the unique id of a media node.</returns>
		public static string GetUniqueId(int mediaId)
		{
			return Cms.GetUniqueId(mediaId);
		}

		/// <summary>
		/// Gets the HTML for an image tag, using the Media Id.
		/// </summary>
		/// <param name="mediaId">The media id.</param>
		/// <returns>
		/// Returns a HTML image tag from a Media Id.
		/// </returns>
		public static string GetImageHtml(string mediaId)
		{
			return GetImageHtml(mediaId, 0, 0);
		}

		/// <summary>
		/// Gets the HTML for an image tag, using the Media Id.
		/// </summary>
		/// <param name="mediaId">The media id.</param>
		/// <param name="defaultHeight">The default height.</param>
		/// <param name="defaultWidth">The default width.</param>
		/// <returns>Returns a HTML image tag from a Media Id.</returns>
		public static string GetImageHtml(string mediaId, int defaultHeight, int defaultWidth)
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
						// .. get the dimensions from the file itself.
						GetImageHeightAndWidth(src, ref height, ref width);
					}

					// format the img tag
					return string.Format(img, src, media.Text, height, width);
				}
			}

			// fall-back return an empty string
			return string.Empty;
		}

		/// <summary>
		/// Gets the height of the image.
		/// </summary>
		/// <param name="path">The path of the image.</param>
		/// <returns>Returns the height of the image.</returns>
		public static int GetImageHeight(string path)
		{
			var imgSize = GetImageSize(path);

			if (!imgSize.IsEmpty)
			{
				return imgSize.Height;
			}

			return 0;
		}

		/// <summary>
		/// Gets the width of the image.
		/// </summary>
		/// <param name="path">The path of the image.</param>
		/// <returns>Returns the width of the image.</returns>
		public static int GetImageWidth(string path)
		{
			var imgSize = GetImageSize(path);

			if (!imgSize.IsEmpty)
			{
				return imgSize.Width;
			}

			return 0;
		}

		/// <summary>
		/// Gets the height and width of the image.
		/// </summary>
		/// <param name="path">The path of the image.</param>
		/// <param name="height">The height.</param>
		/// <param name="width">The width.</param>
		/// <returns>
		/// Returns the height and width of the image.
		/// </returns>
		private static bool GetImageHeightAndWidth(string path, ref int height, ref int width)
		{
			var imgSize = GetImageSize(path);

			if (!imgSize.IsEmpty)
			{
				height = imgSize.Height;
				width = imgSize.Width;

				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the size of the image.
		/// </summary>
		/// <param name="path">The path of the image.</param>
		/// <returns>
		/// Returns the size (dimensions) of the image.
		/// </returns>
		private static Size GetImageSize(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				var localFile = IO.MapPath(path, true);

				if (File.Exists(localFile))
				{
					using (var img = Image.FromFile(localFile))
					{
						if (img != null)
						{
							return img.Size;
						}
					}
				}
			}

			return Size.Empty;
		}
	}
}