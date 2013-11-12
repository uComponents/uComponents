using System.Collections.Generic;
using System.Web;
using uComponents.Core.Extensions;
using Umbraco.Web;
using Umbraco.Web.Templates;

namespace uComponents.MacroEngines.Extensions
{
	/// <summary>
	/// Extension methods for <c>Umbraco.Web.UmbracoHelper</c>.
	/// </summary>
	public static class UmbracoHelperExtensions
	{
		/// <summary>
		/// Renders the macro script.
		/// </summary>
		/// <param name="helper">The helper.</param>
		/// <param name="language">The language.</param>
		/// <param name="fileLocation">The file location.</param>
		/// <returns></returns>
		public static IHtmlString RenderMacroScript(this UmbracoHelper helper, string language, string fileLocation)
		{
			return RenderMacroScript(helper, language, fileLocation, new { });
		}

		/// <summary>
		/// Renders the macro script.
		/// </summary>
		/// <param name="helper">The helper.</param>
		/// <param name="language">The language.</param>
		/// <param name="fileLocation">The file location.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public static IHtmlString RenderMacroScript(this UmbracoHelper helper, string language, string fileLocation, object parameters)
		{
			return RenderMacroScript(helper, language, fileLocation, parameters.ToDictionary<object>());
		}

		/// <summary>
		/// Renders the macro script.
		/// </summary>
		/// <param name="helper">The helper.</param>
		/// <param name="language">The language.</param>
		/// <param name="fileLocation">The file location.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public static IHtmlString RenderMacroScript(this UmbracoHelper helper, string language, string fileLocation, IDictionary<string, object> parameters)
		{
			var ctrl = new umbraco.presentation.templateControls.Macro()
			{
				Language = language,
				FileLocation = fileLocation,
			};

			foreach (var parameter in parameters)
			{
				ctrl.Attributes.Add(parameter.Key, parameter.Value.ToString());
			}

			return new HtmlString(TemplateUtilities.ParseInternalLinks(ctrl.RenderToString()));
		}
	}
}