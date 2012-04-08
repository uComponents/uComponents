using System.Web.UI;
using ClientDependency.Core;
using uComponents.Core;
using uComponents.Core.Extensions;

[assembly: WebResource("uComponents.DataTypes.MultiNodeTreePicker.MultiNodePickerScripts.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.MultiNodeTreePicker.MultiNodePickerStyles.css", Constants.MediaTypeNames.Text.Css, PerformSubstitution = true)]

namespace uComponents.DataTypes.MultiNodeTreePicker
{
	/// <summary>
	/// Extension methods for this namespace
	/// </summary>
	public static class MultiNodeTreePickerExtensions
	{
		/// <summary>
		/// Adds the JS/CSS required for the MultiNodeTreePicker
		/// </summary>
		/// <param name="ctl"></param>
		public static void AddAllMNTPClientDependencies(this Control ctl)
		{
			//get the urls for the embedded resources
			AddCssMNTPClientDependencies(ctl);
			AddJsMNTPClientDependencies(ctl);
		}

		/// <summary>
		/// Adds the CSS required for the MultiNodeTreePicker
		/// </summary>
		/// <param name="ctl"></param>
		public static void AddCssMNTPClientDependencies(this Control ctl)
		{
			ctl.AddResourceToClientDependency("uComponents.DataTypes.MultiNodeTreePicker.MultiNodePickerStyles.css", ClientDependencyType.Css);
		}

		/// <summary>
		/// Adds the JS required for the MultiNodeTreePicker
		/// </summary>
		/// <param name="ctl"></param>
		public static void AddJsMNTPClientDependencies(this Control ctl)
		{
			ctl.AddResourceToClientDependency("uComponents.DataTypes.MultiNodeTreePicker.MultiNodePickerScripts.js", ClientDependencyType.Javascript);
			ctl.AddResourceToClientDependency("uComponents.DataTypes.Shared.Resources.Scripts.jquery.tooltip.min.js", ClientDependencyType.Javascript);
		}
	}
}