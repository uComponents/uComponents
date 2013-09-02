namespace uComponents.Core.Extensions
{
	using System.IO;
	using System.Text;
	using System.Web.UI;

	/// <summary>
	/// Extension methods for System.Web.UI.Control
	/// </summary>
	internal static class ControlExtensions
	{
		/// <summary>
		/// Renders the control.
		/// </summary>
		/// <param name="ctrl">The control to render.</param>
		/// <returns>
		/// Returns a string of the rendered control.
		/// </returns>
		public static string RenderToString(this Control ctrl)
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