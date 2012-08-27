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
    public static class MediaHelper
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

        #endregion

        #region Public Methods

        /// <summary>
        ///   Saves the text image.
        /// </summary>
        /// <param name = "parameters">The parameters.</param>
        /// <param name = "imageName">Name of the image.</param>
        /// <returns></returns>
        public static string SaveTextImage(TextImageParameters parameters, string imageName)
        {
            imageName = LegalizeString(imageName.Replace(" ", "-"));
            Bitmap textImage;
            try
            {
                textImage = ImageGenerator.GenerateTextImage(parameters);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("Error: " + ex.Message);
                textImage =
                    ImageGenerator.GenerateTextImage(new TextImageParameters(ex.Message, parameters.OutputFormat,
                                                                             string.Empty, "ARIAL", 12,
                                                                             new[] {new FontStyle()}, "000", "FFF",
                                                                             "transparent", HorizontalAlignment.Left,
                                                                             VerticalAlignment.Top, -1, -1, null));
            }

            var textImageClientFolder = string.Format("~/media/TextImages");
            var textImageServerFolder = IOHelper.MapPath(textImageClientFolder);

            if (!Directory.Exists(textImageServerFolder))
            {
                Directory.CreateDirectory(textImageServerFolder);
            }

            var textImageFileName = string.Format("{0}.{1}", imageName, parameters.OutputFormat);
            var textImageClientFile = string.Format("{0}/{1}", textImageClientFolder, textImageFileName);
            var textImageServerFile = IOHelper.MapPath(textImageClientFile);
            textImage.Save(textImageServerFile, ConvertToImageFormat(parameters.OutputFormat));
            return textImageClientFile;
        }

        /// <summary>
        ///   Converts to image format.
        /// </summary>
        /// <param name = "outputFormat">The output format.</param>
        /// <returns></returns>
        private static ImageFormat ConvertToImageFormat(OutputFormat outputFormat)
        {
            switch (outputFormat)
            {
                case OutputFormat.Gif:
                    {
                        return ImageFormat.Gif;
                    }
                case OutputFormat.Jpg:
                    {
                        return ImageFormat.Jpeg;
                    }
                case OutputFormat.Png:
                    {
                        return ImageFormat.Png;
                    }
                case OutputFormat.Bmp:
                    {
                        return ImageFormat.Bmp;
                    }
                case OutputFormat.Tif:
                    {
                        return ImageFormat.Tiff;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        /// <summary>
        ///   Creates an image media.
        /// </summary>
        /// <param name = "parentId">The parent media id.</param>
        /// <param name = "mediaName">Name of the media.</param>
        /// <param name = "image">The image.</param>
        /// <param name = "outputFormat">The output format.</param>
        /// <returns>Media Url</returns>
        public static int CreateImageMedia(int parentId, string mediaName, Image image, OutputFormat outputFormat)
        {
            // Create media item
            mediaName = LegalizeString(mediaName.Replace(" ", "-"));
            var mediaItem = Media.MakeNew(mediaName, MediaType.GetByAlias("image"), new User(0), parentId);

            // Filename
            // i.e. 1234.jpg
            var mediaFileName = string.Format("{0}.{1}", mediaItem.Id, outputFormat);
            var mediaThumbName = string.Format("{0}_thumb.{1}", mediaItem.Id, outputFormat);

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
            var url = "";
            var member = currentNavigator.Clone();
            //var nodeIterator = member.Select("data[@alias='umbracoFile']");
            var nodeIterator = member.Select(Constants.Umbraco.Media.File);
            
            while (nodeIterator.MoveNext())
            {
                url = nodeIterator.Current.Value;
            }

            return url;
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