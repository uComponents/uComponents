using System;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml.Linq;
using umbraco;
using System.Text.RegularExpressions;
using System.IO;

namespace uComponents.Core.DataTypes.UrlPicker.Services
{
    /// <summary>
    /// Client side ajax utlities for the URL Picker
    /// </summary>
    [ScriptService]
    [WebService]
    public class UrlPickerService : WebService
    {
        /// <summary>
        /// Returns the NiceUrl of a content node
        /// </summary>
        /// <param name="id">The content node's ID</param>
        /// <returns>The NiceUrl of that content node</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ContentNodeUrl(int id)
        {
            Authorize();

            return library.NiceUrl(id);
        }

        /// <summary>
        /// Returns the URL of the default file in a media node (i.e. the "umbracoFile" property)
        /// else cycles through media node properties looking for "/media/" URLs
        /// </summary>
        /// <param name="id">The media node's ID</param>
        /// <returns>The URL of the file</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MediaNodeUrl(int id)
        {
            Authorize();

            var media = XElement.Parse(library.GetMedia((int)id, false).Current.InnerXml);

            var umbracoFile = media.Element("umbracoFile");

            if (umbracoFile != null)
            {
                return umbracoFile.Value;
            }
            else
            {
                // Cycle through other properties
                foreach (var element in media.Elements())
                {
                    // Check if the property looks like a URL to the media folder
                    if (element != null &&
                        !string.IsNullOrEmpty(element.Value) &&
                        element.Value.Contains("/media/"))
                    {
                        return element.Value;
                    }
                }
            }

            return null;
        }

        internal static void Authorize()
        {
            if (!umbraco.BasePages.BasePage.ValidateUserContextID(umbraco.BasePages.BasePage.umbracoUserContextID))
                throw new Exception("Client authorization failed. User is not logged in");

        }

        /// <summary>
        /// File locker
        /// </summary>
        private static readonly object m_Locker = new object();

        /// <summary>
        /// Ensures that the web service is available on the file system of the
        /// web service
        /// </summary>
        internal static void Ensure()
        {
            var servicePath = Path.Combine(uComponents.Core.Shared.Settings.BaseDir.FullName, "UrlPicker");

            if (!Directory.Exists(servicePath))
            {
                lock (m_Locker)
                {
                    //double check locking
                    if (!Directory.Exists(servicePath))
                    {
                        //now create our new local web service
                        var wServiceTxt = UrlPickerServiceResource.UrlPickerService;
                        var dirUrlPicker = new DirectoryInfo(servicePath);
                        if (!dirUrlPicker.Exists)
                        {
                            dirUrlPicker.Create();
                        }
                        var wServiceFile = new FileInfo(Path.Combine(dirUrlPicker.FullName, "UrlPickerService.asmx"));
                        if (!wServiceFile.Exists)
                        {
                            using (var sw = new StreamWriter(wServiceFile.Create()))
                            {
                                sw.Write(wServiceTxt);
                            }
                        }
                    }
                }
            }
        }

    }
}
