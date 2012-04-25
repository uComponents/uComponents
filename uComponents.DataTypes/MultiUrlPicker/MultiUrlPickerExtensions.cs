using System.Web.UI;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;

[assembly: WebResource("uComponents.DataTypes.MultiUrlPicker.MultiUrlPickerScripts.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.MultiUrlPicker.MultiUrlPickerStyles.css", Constants.MediaTypeNames.Text.Css, PerformSubstitution = true)]

namespace uComponents.DataTypes.MultiUrlPicker
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
            var cssName = "uComponents.DataTypes.MultiUrlPicker.MultiUrlPickerStyles.css";
            ctl.AddResourceToClientDependency(cssName, ClientDependencyType.Css);
        }

        /// <summary>
        /// Adds the JS required for the MultiUrlPicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddJsMultiUrlPickerClientDependencies(this Control ctl)
        {
			ctl.AddResourceToClientDependency("uComponents.DataTypes.Shared.Resources.Scripts.json2.js", ClientDependencyType.Javascript);
            ctl.AddResourceToClientDependency("uComponents.DataTypes.MultiUrlPicker.MultiUrlPickerScripts.js", ClientDependencyType.Javascript);
        }
    }
}
