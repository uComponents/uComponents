using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using uComponents.Core;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.media;
using umbraco.NodeFactory;
using umbraco.IO;

namespace uComponents.DataTypes.TextImage
{
    /// <summary>
    ///   Helper Class
    ///   Wraps frequently used functionality from Umbraco
    /// </summary>
    public static class Helper
    {
        #region Public Properties

        /// <summary>
        ///   Gets the current node.
        /// </summary>
        /// <value>The current node.</value>
        public static Node CurrentNode
        {
            get { return new Node(Convert.ToInt32(HttpContext.Current.Request["id"])); }
        }

/*
        /// <summary>
        /// Gets the current document.
        /// </summary>
        /// <value>The current document.</value>
        public static Document CurrentDocument
        {
            get { return new Document(Convert.ToInt32(HttpContext.Current.Request["id"])); }
        }
*/

        #endregion

        #region Public Methods

        /// <summary>
        ///   Creates an image media.
        /// </summary>
        /// <param name = "parentFolderId">The parent folder id.</param>
        /// <param name = "mediaName">Name of the media.</param>
        /// <param name = "image">The image.</param>
        /// <param name = "imageFormat">The image format.</param>
        /// <returns>Media Url</returns>
        public static int CreateImageMedia(int parentFolderId, string mediaName, Image image, ImageFormat imageFormat)
        {
            // Create media item
            mediaName = LegalizeString(mediaName.Replace(" ", "-"));
            var mediaItem = Media.MakeNew(mediaName, MediaType.GetByAlias("image"), new User(0), parentFolderId);

            // Filename
            // i.e. 1234.jpg
            var mediaFileName = string.Format("{0}.{1}", mediaItem.Id, GetDefaultExtension(imageFormat));
            var mediaThumbName = string.Format("{0}_thumb.{1}", mediaItem.Id, GetDefaultExtension(imageFormat));

            // Client side Paths
            //
            //      sample folder : /media/1234/
            //                    : /media/1234/1234.jpg
            //
            var clientMediaFolder = string.Format("{0}/../media/{1}", GlobalSettings.Path, mediaItem.Id);
            var clientMediaFile = string.Format("{0}/{1}", clientMediaFolder, mediaFileName);
            var clientMediaThumb = string.Format("{0}/{1}", clientMediaFolder, mediaThumbName);

            // Server side Paths
            var serverMediaFolder = IOHelper.MapPath(clientMediaFolder);
            var serverMediaFile = IOHelper.MapPath(clientMediaFile);
            var serverMediaThumb = IOHelper.MapPath(clientMediaThumb);

            var extension = Path.GetExtension(serverMediaFile).ToLower();

            // Create media folder if it doesn't exist
            if (!Directory.Exists(serverMediaFolder))
                Directory.CreateDirectory(serverMediaFolder);

            // Save Image and thumb for new media item
            image.Save(serverMediaFile);
            var savedImage = Image.FromFile(serverMediaFile);
            savedImage.GetThumbnailImage((int) (image.Size.Width*.3), (int) (image.Size.Height*.3), null, new IntPtr()).
                Save(serverMediaThumb);

            mediaItem.getProperty(Constants.Umbraco.Media.Width).Value = savedImage.Size.Width.ToString();
            mediaItem.getProperty(Constants.Umbraco.Media.Height).Value = savedImage.Size.Height.ToString();
            mediaItem.getProperty(Constants.Umbraco.Media.File).Value = clientMediaFile;
            mediaItem.getProperty(Constants.Umbraco.Media.Extension).Value = extension;
            mediaItem.getProperty(Constants.Umbraco.Media.Bytes).Value = File.Exists(serverMediaFile)
                                                              ? new FileInfo(serverMediaFile).Length.ToString()
                                                              : "0";

            mediaItem.Save();
            mediaItem.XmlGenerate(new XmlDocument());

            image.Dispose();
            savedImage.Dispose();

            // assign output parameters
            return mediaItem.Id;
        }

        /// <summary>
        ///   Updates the image media.
        /// </summary>
        /// <param name = "mediaId">The media id.</param>
        /// <param name = "image">The image.</param>
        public static void UpdateImageMedia(int mediaId, Image image)
        {
            var mediaFilePath = IOHelper.MapPath(GetMediaUrl(mediaId));
            var mediaFolderPath = Path.GetDirectoryName(mediaFilePath);

            if (!Directory.Exists(mediaFolderPath))
                Directory.CreateDirectory(mediaFolderPath);

            image.Save(mediaFilePath);
        }

        /// <summary>
        ///   Gets the media URL.
        /// </summary>
        /// <param name = "mediaId">The media id.</param>
        /// <returns></returns>
        public static string GetMediaUrl(int mediaId)
        {
            var url = library.NiceUrl(mediaId);

            //assume its media
            if (url == string.Empty)
            {
                var nodeIterator = library.GetMedia(mediaId, false);
                while (nodeIterator.MoveNext())
                {
                    var currentNavigator = nodeIterator.Current;
                    url = GetUrl(currentNavigator);
                }
            }

            var file = IOHelper.MapPath(url);
            return File.Exists(file) ? url : string.Empty;
        }

        /*
        /// <summary>
        /// Gets the current property value.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static string GetCurrentPropertyValue(string alias)
        {
            return CurrentNode.GetProperty(alias).Value;
        }
*/

        #endregion

        #region Private Methods

        /// <summary>
        ///   for media node get url by traversing the data node
        /// </summary>
        /// <param name = "currentNavigator">The current navigator.</param>
        /// <returns></returns>
        private static string GetUrl(XPathNavigator currentNavigator)
        {
            // Clone navigator to move off axis. 
            var url = string.Empty;
            var member = currentNavigator.Clone();
            var nodeIterator = member.Select(string.Concat("data[@alias='", Constants.Umbraco.Media.File, "']"));
            
            while (nodeIterator.MoveNext())
            {
                url = nodeIterator.Current.Value;
            }

            return url;
        }

        /// <summary>
        ///   Gets the default extension.
        /// </summary>
        /// <param name = "imageFormat">The image format.</param>
        /// <returns></returns>
        private static string GetDefaultExtension(ImageFormat imageFormat)
        {
            if (imageFormat == ImageFormat.Jpeg)
                return "jpg";
            if (imageFormat == ImageFormat.Png)
                return "png";
            if (imageFormat == ImageFormat.Gif)
                return "gif";
            if (imageFormat == ImageFormat.Wmf | imageFormat == ImageFormat.Emf)
                return "wmf";
            if (imageFormat == ImageFormat.Tiff)
                return "tif";
            if (imageFormat == ImageFormat.Bmp | imageFormat == ImageFormat.MemoryBmp)
                return "bmp";
            return string.Empty;
        }

        /// <summary>
        ///   Legalizes the string.
        /// </summary>
        /// <param name = "text">The text.</param>
        /// <returns></returns>
        private static string LegalizeString(string text)
        {
            var illegalCharacters = string.Format("{0}{1}",
                                                  new string(Path.GetInvalidFileNameChars()),
                                                  new string(Path.GetInvalidPathChars()));
            var regex = new Regex(string.Format("[{0}]", Regex.Escape(illegalCharacters)));

            return regex.Replace(text, "");
        }

        #endregion
    }
}