using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;

namespace uComponents.Core.Shared.AjaxUpload
{
    /// <summary>
    /// Allows uploading of files to the media folder via a simple http handler.
    /// Indirectly, this allows AJAX uploading of files.
    /// 
    /// Response is a JSON serialized response, e.g:
    /// -----------------------------------
    /// {
    ///     statusCode: 200,
    ///     statusDescription: All posted files were saved successfully,
    ///     filesSaved: {
    ///         0: ~/media/116/myImage.jpg
    ///     }
    /// }
    /// -----------------------------------
    /// 
    /// Other response codes returned are 400 (bad request) &amp; 500 (server file errors).
    /// </summary>
    /// <param>
    /// The integer ID of the upload - i.e, the subfolder of media which the file(s) will be
    /// saved in.
    /// 
    /// Must be nonnegative.
    /// </param>
    /// <param>
    /// Include any files via the "multipart/form-data" encoding type.  They will all be saved with their
    /// original filename.
    /// </param>
    public class AjaxUploadHandler : IHttpHandler
    {
        /// <summary>
        /// Used to filter out form fields which do no apply to this handler
        /// </summary>
        public static readonly string Guid = "7EEE876C-47BF-4390-97BA-CEDE69F68CD9";

        private static readonly object m_Locker = new object();

        // Constants
        private string m_SavePath
        {
            get
            {
                // Default media path
                return umbraco.IO.IOHelper.ResolveUrl(umbraco.IO.SystemDirectories.Media).TrimEnd('/');
            }
        }

        private static readonly string m_IdParam = Guid + "_Id";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            Authorize();

            int id;
            if (!int.TryParse(context.Request[m_IdParam], out id) || id < 0)
            {
                context.Response.StatusCode = 400;
                context.Response.StatusDescription = string.Format("You must include a parameter named '{0}', which is a non-negative integer describing the subfolder under '{1}/' to save to.", m_IdParam, m_SavePath);
                WriteResponseBody(context);
                return;
            }

            var path = string.Format("{0}/{1}/", m_SavePath, id.ToString());

            // Save all relevant files which are posted
            var savedFiles = new List<string>();
            foreach (string key in context.Request.Files)
            {
                // Get the HttpPostedFile
                var file = context.Request.Files[key];

                // Check if this param is a file, and that the file is meant for this
                // handler (via the GUID).  Also check it isn't a null entry.
                if (key.Contains(Guid) && !string.IsNullOrEmpty(file.FileName.Trim()) && file.ContentLength > 0)
                {
                    // Create the directory for this transaction
                    var directory = HttpContext.Current.Server.MapPath(path);

                    var shortFileName = Path.GetFileName(file.FileName);
                    var fullFileName = directory + shortFileName;

                    // Save file
                    lock (m_Locker)
                    {
                        Directory.CreateDirectory(directory);

                        //if (File.Exists(fullFileName))
                        //{
                        //    context.Response.StatusCode = 500;
                        //    context.Response.StatusDescription = string.Format("File '{0}/{1}/{2}' already exists", m_SavePath, id, shortFileName);
                        //    WriteResponseBody(context);
                        //    return;
                        //}

                        try
                        {
                            file.SaveAs(fullFileName);
                        }
                        catch (Exception)
                        {
                            context.Response.StatusCode = 500;
                            context.Response.StatusDescription = string.Format("File '{0}/{1}/{2}' could not be saved", m_SavePath, id, shortFileName);
                            WriteResponseBody(context);
                            return;
                        }

                        // Log file
                        savedFiles.Add(string.Format("{0}/{1}/{2}", m_SavePath.TrimStart('~'), id, shortFileName));
                    }
                }
            }

            if (savedFiles.Count == 0)
            {
                context.Response.StatusCode = 400;
                context.Response.StatusDescription = "You must post at least one file";
                WriteResponseBody(context);
                return;
            }

            // Log saved files
            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "All posted files were saved successfully";
            WriteResponseBody(context, savedFiles);
        }

		private void WriteResponseBody(HttpContext context)
		{
			WriteResponseBody(context, null);
		}

        private void WriteResponseBody(HttpContext context, List<string> filesSaved)
        {
            var jss = new JavaScriptSerializer();

            context.Response.ContentType = "text/plain";
            context.Response.Write(jss.Serialize(new
            {
                statusCode = context.Response.StatusCode,
                statusDescription = context.Response.StatusDescription,
                filesSaved = filesSaved
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        internal static void Authorize()
        {
            if (!umbraco.BasePages.BasePage.ValidateUserContextID(umbraco.BasePages.BasePage.umbracoUserContextID))
                throw new Exception("Client authorization failed. User is not logged in");

        }

        /// <summary>
        /// Ensures that the handler is available on the file system
        /// </summary>
        internal static void Ensure()
        {
            var handlerPath = Path.Combine(uComponents.Core.Settings.BaseDir.FullName, "Shared/AjaxUpload");

            if (!Directory.Exists(handlerPath))
            {
                lock (m_Locker)
                {
                    //double check locking
                    if (!Directory.Exists(handlerPath))
                    {
                        //now create our new local web service
                        var wHandlerTxt = AjaxUploadHandlerResource.AjaxUploadHandler_ashx;
                        var dirUrlPicker = new DirectoryInfo(handlerPath);
                        if (!dirUrlPicker.Exists)
                        {
                            dirUrlPicker.Create();
                        }
                        var wHandlerFile = new FileInfo(Path.Combine(dirUrlPicker.FullName, "AjaxUploadHandler.ashx"));
                        if (!wHandlerFile.Exists)
                        {
                            using (var sw = new StreamWriter(wHandlerFile.Create()))
                            {
                                sw.Write(wHandlerTxt);
                            }
                        }
                    }
                }
            }
        }
    }
}