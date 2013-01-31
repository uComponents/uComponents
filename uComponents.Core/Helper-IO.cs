using System.Collections.Generic;
using System.IO;
using System.Linq;
using umbraco.IO;

namespace uComponents.Core
{
    /// <summary>
    /// Generic helper methods
    /// </summary>
    internal static partial class Helper
    {
        /// <summary>
        /// IO helpers
        /// </summary>
        public static class IO
        {
            /// <summary>
            /// The locker object
            /// </summary>
            private static readonly object locker = new object();

            /// <summary>
            /// Gets the assemblies.
            /// </summary>
            /// <returns>Returns a list of assembly names.</returns>
            public static string[] GetAssemblies()
            {
                var assemblies = new List<string>();

                // check if the App_Code directory exists and has any files
                var appCode = new DirectoryInfo(IOHelper.MapPath("~/App_Code"));
                if (appCode != null && appCode.Exists && appCode.GetFiles().Length > 0)
                {
                    assemblies.Add(appCode.Name);
                }

                // add assemblies from the /bin directory
                assemblies.AddRange(Directory.GetFiles(IOHelper.MapPath("~/bin"), "*.dll").Select(fileName => fileName.Substring(fileName.LastIndexOf('\\') + 1)));

                return assemblies.ToArray();
            }

            /// <summary>
            /// Ensures the folder exists.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>The folder info.</returns>
            public static DirectoryInfo EnsureFolderExists(string path)
            {
                if (!Directory.Exists(path))
                {
                    lock (locker)
                    {
                        if (!Directory.Exists(path))
                        {
                            var dir = new DirectoryInfo(path);
                            dir.Create();
                        }
                    }
                }

                return new DirectoryInfo(path);
            }

            /// <summary>
            /// Ensures the file exists.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="content">The content.</param>
            /// <returns>The file info.</returns>
            public static FileInfo EnsureFileExists(string path, string content)
            {
                if (!File.Exists(path))
                {
                    lock (locker)
                    {
                        if (!File.Exists(path))
                        {
                            using (var writer = new StreamWriter(File.Create(path)))
                            {
                                writer.Write(content);
                            }
                        }
                    }
                }

                return new FileInfo(path);
            }
        }
    }
}