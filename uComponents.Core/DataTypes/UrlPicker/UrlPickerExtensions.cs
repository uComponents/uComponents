using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ClientDependency.Core.Controls;
using ClientDependency.Core;
using uComponents.Core.Shared;


[assembly: WebResource("uComponents.Core.DataTypes.UrlPicker.UrlPickerScripts.js", MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.Core.DataTypes.UrlPicker.UrlPickerStyles.css", MediaTypeNames.Text.Css, PerformSubstitution = true)]
namespace uComponents.Core.DataTypes.UrlPicker
{
    /// <summary>
    /// Extension methods for the URL picker
    /// </summary>
    public static class UrlPickerExtensions
    {
        /// <summary>
        /// Adds the JS/CSS required for the UrlPicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddAllUrlPickerClientDependencies(this Control ctl)
        {
            //get the urls for the embedded resources
            AddCssUrlPickerClientDependencies(ctl);
            AddJsUrlPickerClientDependencies(ctl);
        }

        /// <summary>
        /// Adds the CSS required for the UrlPicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddCssUrlPickerClientDependencies(this Control ctl)
        {
            var cssName = "uComponents.Core.DataTypes.UrlPicker.UrlPickerStyles.css";
            ctl.AddResourceToClientDependency(cssName, ClientDependencyType.Css);
        }

        /// <summary>
        /// Adds the JS required for the UrlPicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddJsUrlPickerClientDependencies(this Control ctl)
        {
            ctl.AddResourceToClientDependency("uComponents.Core.Shared.Resources.Scripts.json2.js", ClientDependencyType.Javascript);
            ctl.AddResourceToClientDependency("uComponents.Core.Shared.Resources.Scripts.jquery.form.js", ClientDependencyType.Javascript);
            ctl.AddResourceToClientDependency("uComponents.Core.DataTypes.UrlPicker.UrlPickerScripts.js", ClientDependencyType.Javascript);
        }
    }
}
