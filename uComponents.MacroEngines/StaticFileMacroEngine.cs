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
		/// Gets the supported extensions.
		/// </summary>
		/// <value>The supported extensions.</value>
		public IEnumerable<string> SupportedExtensions
		{
			get
			{
				return new string[] { "text", "txt", "htm", "html" };
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
				return new string[] { "txt", "html" };
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
		/// <returns>Always returns <c>true</c>, as no validation is required.</returns>
		public bool Validate(string code, string tempFileName, INode currentPage, out string errorMessage)
		{
			errorMessage = string.Empty;
			return true;
		}

		/// <summary>
		/// Executes the specified macro.
		/// </summary>
		/// <param name="macro">The macro.</param>
		/// <param name="currentPage">The current page.</param>
		/// <returns>Returns a string of the executed macro text/HTML.</returns>
		public string Execute(MacroModel macro, INode currentPage)
		{
			if (!string.IsNullOrEmpty(macro.ScriptName))
			{
				var fileLocation = macro.ScriptName.StartsWith("~") ? macro.ScriptName : Path.Combine(SystemDirectories.MacroScripts, macro.ScriptName);
				return File.ReadAllText(IOHelper.MapPath(fileLocation));
			}

			return macro.ScriptCode;
		}
	}
}
