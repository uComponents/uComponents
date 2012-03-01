using System.Web.UI;
using ClientDependency.Core;
using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;

[assembly: WebResource("uComponents.Core.DataTypes.MultiUrlPicker.MultiUrlPickerScripts.js", MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.Core.DataTypes.MultiUrlPicker.MultiUrlPickerStyles.css", MediaTypeNames.Text.Css, PerformSubstitution = true)]
namespace uComponents.Core.DataTypes.MultiUrlPicker
{
    /// <summary>
    /// Extension methods for the Multi URL picker
    /// </summary>
    public static class MultiUrlPickerExtensions
    {
        /// <summary>
        /// Adds the JS/CSS required for the MultiUrlPicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddAllMultiUrlPickerClientDependencies(this Control ctl)
        {
            //get the urls for the embedded resources
            AddCssMultiUrlPickerClientDependencies(ctl);
            AddJsMultiUrlPickerClientDependencies(ctl);
        }

        /// <summary>
        /// Adds the CSS required for the MultiUrlPicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddCssMultiUrlPickerClientDependencies(this Control ctl)
        {
            var cssName = "uComponents.Core.DataTypes.MultiUrlPicker.MultiUrlPickerStyles.css";
            ctl.AddResourceToClientDependency(cssName, ClientDependencyType.Css);
        }

        /// <summary>
        /// Adds the JS required for the MultiUrlPicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddJsMultiUrlPickerClientDependencies(this Control ctl)
        {
			ctl.AddResourceToClientDependency("uComponents.Core.Shared.Resources.Scripts.json2.js", ClientDependencyType.Javascript);
            ctl.AddResourceToClientDependency("uComponents.Core.DataTypes.MultiUrlPicker.MultiUrlPickerScripts.js", ClientDependencyType.Javascript);
        }
    }
}
