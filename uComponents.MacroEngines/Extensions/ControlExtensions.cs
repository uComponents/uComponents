using System.IO;
using System.Text;
using System.Web.UI;

namespace uComponents.MacroEngines.Extensions
{
    using System;

    /// <summary>
	/// Extension methods for <c>System.Web.UI.Control</c>.
	/// </summary>
	[Obsolete("use:  uComponents.Core.Extensions")]
	internal static class ControlExtensions
	{
		/// <summary>
		/// Renders the control.
		/// </summary>
		/// <param name="ctrl">The control to render.</param>
		/// <returns>
		/// Returns a string of the rendered control.
		/// </returns>
		public static string RenderControlToString(this Control ctrl)
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
