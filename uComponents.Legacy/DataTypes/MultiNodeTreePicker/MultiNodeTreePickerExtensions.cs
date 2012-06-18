using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ClientDependency.Core;
using ClientDependency.Core.Controls;
using uComponents.Core;
using umbraco.cms.businesslogic.datatype;

[assembly: WebResource("uComponents.Core.DataTypes.MultiNodeTreePicker.MultiNodePickerScripts.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.Core.DataTypes.MultiNodeTreePicker.MultiNodePickerStyles.css", Constants.MediaTypeNames.Text.Css, PerformSubstitution = true)]

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
			ctl.AddResourceToClientDependency("uComponents.Legacy.DataTypes.MultiNodeTreePicker.MultiNodePickerStyles.css", ClientDependency.Core.ClientDependencyType.Css);
		}

		/// <summary>
		/// Adds the JS required for the MultiNodeTreePicker
		/// </summary>
		/// <param name="ctl"></param>
		public static void AddJsMNTPClientDependencies(this Control ctl)
		{
			ctl.AddResourceToClientDependency("uComponents.Legacy.DataTypes.MultiNodeTreePicker.MultiNodePickerScripts.js", ClientDependency.Core.ClientDependencyType.Javascript);
			ctl.AddResourceToClientDependency("uComponents.Core.Shared.Resources.Scripts.jquery.tooltip.min.js", ClientDependency.Core.ClientDependencyType.Javascript);
		}
	}
}