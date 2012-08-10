using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using umbraco;
using umbraco.cms.businesslogic.macro;
using umbraco.interfaces;
using umbraco.IO;

namespace uComponents.Core.MacroEngines
{
	/// <summary>
	/// XSLT Macro Engine
	/// </summary>
	public class XsltMacroEngine : IMacroEngine
	{
		/// <summary>
		/// Gets the name of the macro engine.
		/// </summary>
		/// <value>The name of the macro engine.</value>
		public string Name
		{
			get
			{
				return "XSLT Macro Engine";
			}
		}

		/// <summary>
		/// Gets the XSLT temp directory.
		/// </summary>
		/// <value>The XSLT temp directory.</value>
		public string XsltTempDirectory
		{
			get
			{
				return string.Concat(GlobalSettings.StorageDirectory, "/TEMP/MacroEngines/Xslt/");
			}
		}

		/// <summary>
		/// Gets the supported extensions.
		/// </summary>
		/// <value>The supported extensions.</value>
		public IEnumerable<string> SupportedExtensions
		{
			get
			{
				return new string[] { "xslt" };
			}
		}

		/// <summary>
		/// Gets the supported UI extensions.
		/// </summary>
		/// <value>The supported UI extensions.</value>
		public IEnumerable<string> SupportedUIExtensions
		{
			get
			{
				// intentionally returns an empty array,
				// (as to not display as macro engine in the back-office).
				return new string[] { };
			}
		}

		/// <summary>
		/// Gets the supported properties.
		/// </summary>
		/// <value>The supported properties.</value>
		/// <remarks>Not Implemented.</remarks>
		public Dictionary<string, IMacroGuiRendering> SupportedProperties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Validates the specified code.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="tempFileName">Name of the temp file.</param>
		/// <param name="currentPage">The current page.</param>
		/// <param name="errorMessage">The error message.</param>
		/// <returns></returns>
		/// <remarks>Not Implemented.</remarks>
		public bool Validate(string code, string tempFileName, INode currentPage, out string errorMessage)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Executes the specified macro.
		/// </summary>
		/// <param name="macro">The macro.</param>
		/// <param name="currentPage">The current page.</param>
		/// <returns>Returns a string of the executed macro XSLT.</returns>
		public string Execute(MacroModel macro, INode currentPage)
		{
			string fileLocation = null;

			if (!string.IsNullOrEmpty(macro.ScriptName))
			{
				fileLocation = string.Concat("../", macro.ScriptName);
			}
			else if (!string.IsNullOrEmpty(macro.ScriptCode))
			{
				var xslt = this.CheckXsltFragment(macro.ScriptCode.Trim());
				var md5 = library.md5(xslt);
				var filename = string.Concat("inline-", md5, ".xslt");
				fileLocation = this.CreateTemporaryFile(xslt, filename, true).Replace("~", "..");
			}

			if (string.IsNullOrEmpty(fileLocation))
			{
				return string.Empty;
			}

			var tempMacro = new macro() { };
			tempMacro.Model.Xslt = fileLocation;

			// copy the macro properties across
			foreach (var property in macro.Properties)
			{
				tempMacro.Model.Properties.Add(new MacroPropertyModel(property.Key, property.Value));
			}

			var ctrl = tempMacro.loadMacroXSLT(tempMacro, macro, (Hashtable)HttpContext.Current.Items["pageElements"]);

			return this.RenderControl(ctrl);
		}

		/// <summary>
		/// Checks if the XSLT is a fragment, if so, then builds up the full XSLT document (with headers).
		/// </summary>
		/// <param name="xslt">The contents of the XSLT.</param>
		/// <returns>Returns a full XSLT document.</returns>
		private string CheckXsltFragment(string xslt)
		{
			if (!xslt.Contains("<xsl:stylesheet"))
			{
				using (var cleanXslt = File.OpenText(IOHelper.MapPath(SystemDirectories.Umbraco + "/xslt/templates/clean.xslt")))
				{
					string tempXslt = cleanXslt.ReadToEnd();
					xslt = tempXslt.Replace("<!-- start writing XSLT -->", xslt);
					xslt = macro.AddXsltExtensionsToHeader(xslt);
				}
			}

			return xslt;
		}

		/// <summary>
		/// Creates the temporary file.
		/// </summary>
		/// <param name="xslt">The contents of the XSLT.</param>
		/// <param name="filename">The filename.</param>
		/// <param name="skipExists">if set to <c>true</c> [skip exists].</param>
		/// <returns>Returns the filepath of the temporary file.</returns>
		private string CreateTemporaryFile(string xslt, string filename, bool skipExists)
		{
			var relativePath = string.Concat(this.XsltTempDirectory, filename);
			var physicalPath = IOHelper.MapPath(relativePath);
			var physicalDirectoryPath = IOHelper.MapPath(this.XsltTempDirectory);

			if (skipExists && File.Exists(physicalPath))
			{
				return relativePath;
			}

			if (File.Exists(physicalPath))
			{
				File.Delete(physicalPath);
			}

			if (!Directory.Exists(physicalDirectoryPath))
			{
				Directory.CreateDirectory(physicalDirectoryPath);
			}

			using (var file = new StreamWriter(physicalPath, false, Encoding.UTF8))
			{
				file.Write(xslt);
			}

			return relativePath;
		}

		/// <summary>
		/// Renders the control.
		/// </summary>
		/// <param name="ctrl">The control to render.</param>
		/// <returns>Returns a string of the rendered control.</returns>
		private string RenderControl(Control ctrl)
		{
			var sb = new StringBuilder();

			using (var tw = new StringWriter(sb))
			using (var hw = new HtmlTextWriter(tw))
			{
				ctrl.RenderControl(hw);
			}

			return sb.ToString();
		}
	}
}