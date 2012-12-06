using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using uComponents.Core;
using umbraco;
using umbraco.IO;

namespace uComponents.XsltExtensions
{
	/// <summary>
	/// The IO class exposes XSLT extensions to access data from System.IO.
	/// </summary>
	[XsltExtension("ucomponents.io")]
	public class IO
	{
		/// <summary>
		/// Directories the exists.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>Returns true if the directory exists, otherwise false.</returns>
		public static bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		/// <summary>
		/// Files the exists.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>Returns true if the file exists, otherwise false.</returns>
		public static bool FileExists(string path)
		{
			return File.Exists(path);
		}

		/// <summary>
		/// Gets the directories.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="allDirectories">if set to <c>true</c> [all directories].</param>
		/// <returns>Returns a node-set of the directories.</returns>
		public static XPathNodeIterator GetDirectories(string path, string searchPattern, bool allDirectories)
		{
			try
			{
				// create the XML document
				var xd = new XmlDocument();
				xd.LoadXml("<Directories/>");

				// set the search options
				var searchOption = allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

				// get the directories
				var directories = Directory.GetDirectories(path, searchPattern, searchOption);

				// check there are directories
				if (directories != null && directories.Length > 0)
				{
					// loop through each of the directories
					foreach (var directory in directories)
					{
						// create an XML node for the directory
						var directoryNode = umbraco.xmlHelper.addTextNode(xd, "Directory", directory);

						// add the node to the XML document
						xd.DocumentElement.AppendChild(directoryNode);
					}
				}

				// return the XML document
				return xd.CreateNavigator().Select("/Directories");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="allDirectories">if set to <c>true</c> [all directories].</param>
		/// <returns>Returns a node-set of the files.</returns>
		public static XPathNodeIterator GetFiles(string path, string searchPattern, bool allDirectories)
		{
			try
			{
				// create the XML document
				var xd = new XmlDocument();
				xd.LoadXml("<Files/>");

				// set the search options
				var searchOption = allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

				// get the files
				var files = Directory.GetFiles(path, searchPattern, searchOption);

				// check there are files
				if (files != null && files.Length > 0)
				{
					// loop through each of the files
					foreach (var file in files)
					{
						// create an XML node for the file
						var fileNode = umbraco.xmlHelper.addTextNode(xd, "File", file);

						// add the node to the XML document
						xd.DocumentElement.AppendChild(fileNode);
					}
				}

				// return the XML document
				return xd.CreateNavigator().Select("/Files");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the extension.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <returns>Returns the extension of the specified path string.</returns>
		public static string GetExtension(string path)
		{
			if (path.StartsWith(Constants.Common.HTTP))
			{
				path = GetLocalFilePath(path);
			}

			return Path.GetExtension(path);
		}

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <returns>Returns the file name and extension of the specified path string.</returns>
		public static string GetFileName(string path)
		{
			if (path.StartsWith(Constants.Common.HTTP))
			{
				path = GetLocalFilePath(path);
			}

			return Path.GetFileName(path);
		}

		/// <summary>
		/// Gets the file name without extension.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <returns>Returns the file name of the specified path string without the extension.</returns>
		public static string GetFileNameWithoutExtension(string path)
		{
			if (path.StartsWith(Constants.Common.HTTP))
			{
				path = GetLocalFilePath(path);
			}

			return Path.GetFileNameWithoutExtension(path);
		}

		/// <summary>
		/// Gets the name of the directory.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <returns>Returns the directory name for the specified path string.</returns>
		public static string GetDirectoryName(string path)
		{
			if (path.StartsWith(Constants.Common.HTTP))
			{
				path = GetLocalFilePath(path);
			}

			return Path.GetDirectoryName(path);
		}

		/// <summary>
		/// Gets the size of the file.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>
		/// Returns the filesize for the specified filepath.
		/// </returns>
		public static long GetFileSize(string path)
		{
			var file = GetFileInfo(path);

			if (file != null)
			{
				return file.Length;
			}

			return 0;
		}

		/// <summary>
		/// Gets the mapped path to the server.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>Returns the physical file path that corresponds to the specified virtual path on the web server.</returns>
		[Obsolete("Please use uComponents.XsltExtensions.IO.MapPath")]
		public static string GetServerMapPath(string path)
		{
			return MapPath(path, true);
		}

		/// <summary>
		/// Gets the mapped path to the server.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="useHttpContext">if set to <c>true</c> [use HTTP context].</param>
		/// <returns>Returns the physical file path that corresponds to the specified virtual path on the web server.</returns>
		public static string MapPath(string path, bool useHttpContext)
		{
			return IOHelper.MapPath(path, useHttpContext);
		}

		/// <summary>
		/// Truncates the middle section of a string, this is ideal for long filepaths or URLs.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns a shortened path of the string.</returns>
		public static string PathShortener(string input)
		{
			string pattern = @"^(\w+:|\\)(\\[^\\]+\\[^\\]+\\).*(\\[^\\]+\\[^\\]+)$";
			string replacement = "$1$2...$3";

			if (!Regex.Match(input, pattern).Success)
			{
				return input;
			}

			return Regex.Replace(input, pattern, replacement);
		}

		/// <summary>
		/// Formats the size of the file.
		/// </summary>
		/// <param name="filesize">The filesize.</param>
		/// <returns></returns>
		public static string FormatFileSize(long filesize)
		{
			var suffix = new[] { "bytes", "KB", "MB", "GB", "TB", "PB", "EB" };
			var i = 0;
			var kilobyte = 1024;

			// while the filesize is over 1KB and index is less than the length of the suffix array.
			while (filesize >= kilobyte && i < suffix.Length)
			{
				// divide the filesize by 1024 - to give the next SI unit suffix.
				filesize = filesize / kilobyte;

				// increment the index position.
				i++;
			}

			// return the filesize with suffix.
			return string.Concat(filesize, " ", suffix[i]);
		}

		/// <summary>
		/// Loads and reads the contents of a file.
		/// </summary>
		/// <param name="filepath">The filepath.</param>
		/// <returns>
		/// Returns the contents of the specified file.
		/// </returns>
		public static string LoadFile(string filepath)
		{
			string path = MapPath(filepath, true);

			using (var reader = new StreamReader(path))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Gets the FileInfo.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>
		/// Returns the <c>System.IO.FileInfo</c> for the specified filepath.
		/// </returns>
		private static FileInfo GetFileInfo(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				var localFile = MapPath(path, true);

				if (File.Exists(localFile))
				{
					return new FileInfo(localFile);
				}
			}

			return null;
		}

		/// <summary>
		/// Checks if the file path is a Uri, then returns the local file path.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <returns>Returns the local file path.</returns>
		private static string GetLocalFilePath(string path)
		{
			Uri uri;
			if (Uri.TryCreate(path, UriKind.Absolute, out uri))
			{
				return uri.LocalPath;
			}

			return path;
		}
	}
}