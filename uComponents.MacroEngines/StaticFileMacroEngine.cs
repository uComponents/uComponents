using System;
using System.Collections.Generic;
using System.IO;
using umbraco;
using umbraco.cms.businesslogic.macro;
using umbraco.interfaces;
using Umbraco.Core.IO;

namespace uComponents.MacroEngines
{
	/// <summary>
	/// Static File Macro Engine
	/// </summary>
	public class StaticFileMacroEngine : IMacroEngine, IMacroEngineResultStatus
	{
		/// <summary>
		/// Gets the name of the macro engine.
		/// </summary>
		/// <value>The name of the macro engine.</value>
		public string Name
		{
			get
			{
				return "uComponents: Static File Macro Engine";
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
				return new[] { "text", "txt", "htm", "html" };
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
				return new[] { "txt", "html" };
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
			try
			{
				this.Success = true;

				if (!string.IsNullOrEmpty(macro.ScriptName))
				{
					var fileLocation = macro.ScriptName.StartsWith("~") ? macro.ScriptName : Path.Combine(SystemDirectories.MacroScripts, macro.ScriptName);
					return File.ReadAllText(IOHelper.MapPath(fileLocation));
				}

				return macro.ScriptCode;

			}
			catch (Exception ex)
			{
				this.ResultException = ex;
				this.Success = false;

				return string.Format("<div style=\"border: 1px solid #990000\">Error loading file {0}<br />{1}</div>", macro.ScriptName, GlobalSettings.DebugMode ? ex.Message : string.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the result exception.
		/// </summary>
		/// <value>
		/// The result exception.
		/// </value>
		public Exception ResultException { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="StaticFileMacroEngine"/> is success.
		/// </summary>
		/// <value>
		///   <c>true</c> if success; otherwise, <c>false</c>.
		/// </value>
		public bool Success { get; set; }
	}
}