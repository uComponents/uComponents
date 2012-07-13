using System;
using System.Collections.Generic;
using System.IO;
using umbraco.cms.businesslogic.macro;
using umbraco.interfaces;
using umbraco.IO;

namespace uComponents.MacroEngines
{
	/// <summary>
	/// Static File Macro Engine
	/// </summary>
	public class StaticFileMacroEngine : IMacroEngine
	{
		/// <summary>
		/// Gets the name of the macro engine.
		/// </summary>
		/// <value>The name of the macro engine.</value>
		public string Name
		{
			get
			{
				return "Static File Macro Engine";
			}
		}

		/// <summary>
		/// Gets the temp directory.
		/// </summary>
		/// <value>The temp directory.</value>
		public string StaticFileTempDirectory
		{
			get
			{
				return "~/App_Data/TEMP/StaticFiles/";
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
				return new string[] { "txt", "htm", "html" };
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
				return new string[] { "txt", "htm", "html" };
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
			if (!string.IsNullOrEmpty(macro.ScriptName))
			{
				return File.ReadAllText(IOHelper.MapPath(macro.ScriptName));
			}

			return macro.ScriptCode;
		}
	}
}
