using System;
using System.Linq;
using System.IO;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml.Linq;
using uComponents.Core;
using umbraco;

namespace uComponents.DataTypes.UrlPicker.Services
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
            // Github Issue #30 - A 500 server error is thrown when a local Umbraco media file is attempted to be loaded/added in Umbraco 6.2.5
            // Following code is taken from Governor Technology at https://our.umbraco.org/projects/backoffice-extensions/ucomponents/questionssuggestions/61972-URL-picker-just-spinning,-generating-500-server-error?p=0#comment211285
            // So, looking at the source for uComponents, it seems it is down to the following method: uComponents.DataTypes.UrlPicker.Services.MediaNodeUrl. 
            // It extracts the media item XML by ID and then uses XElement.Parse on it, however it seems that 
            // in Umbraco 6.2.5, the XML fragment being parsed here has multiple root elements, which causes this method to fail. 

            Authorize();

            var media = XElement.Parse(string.Format("<root>{0}</root>", library.GetMedia((int)id, false).Current.InnerXml));

            var umbracoFile = media.Descendants(Constants.Umbraco.Media.File);

            if (umbracoFile.Any())
            {
                return umbracoFile.First().Value;
            }
            else
            {
                // Cycle through other properties
                foreach (var element in media.Descendants())
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
            var serviceFile = Path.Combine(Settings.BaseDir.FullName, "UrlPicker/UrlPickerService.asmx");

            if (!File.Exists(serviceFile))
            {
                lock (m_Locker)
                {
                    // double check locking
                    if (!File.Exists(serviceFile))
                    {
                        // now create our new local web service
                        var wServiceFile = new FileInfo(serviceFile);
                        if (!wServiceFile.Exists)
                        {
                            var wServiceTxt = UrlPickerServiceResource.UrlPickerService;

                            if (!wServiceFile.Directory.Exists)
                            {
                                wServiceFile.Directory.Create();
                            }

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