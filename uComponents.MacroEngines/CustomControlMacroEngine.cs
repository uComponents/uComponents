using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using umbraco;
using umbraco.cms.businesslogic.macro;
using umbraco.interfaces;

namespace uComponents.MacroEngines
{
	/// <summary>
	/// Custom Control Macro Engine
	/// </summary>
	public class CustomControlMacroEngine : IMacroEngine
	{
		/// <summary>
		/// Gets the name of the macro engine.
		/// </summary>
		/// <value>The name of the macro engine.</value>
		public string Name
		{
			get
			{
				return "uComponents: Custom Control Macro Engine";
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
				return new string[] { "control", "dll" };
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
		/// <returns>Returns a string of the executed macro text/HTML.</returns>
		public string Execute(MacroModel macro, INode currentPage)
		{
			var typeAssembly = "uComponents.Controls";
			var typeName = "uComponents.Controls.RenderTemplate";
			
			var tempMacro = new macro() { };
			var ctrl = tempMacro.loadControl(typeAssembly, typeName, macro, (Hashtable)HttpContext.Current.Items["pageElements"]);

			return this.RenderControl(ctrl);
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