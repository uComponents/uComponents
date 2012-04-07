using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ClientDependency.Core.Controls;
using ClientDependency.Core;
using uComponents.Core;
using uComponents.Core.Extensions;
using uComponents.DataTypes.Shared.Extensions;

// embed resources
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
            var cssName = "uComponents.Core.DataTypes.MultiNodeTreePicker.MultiNodePickerStyles.css";
            ctl.AddResourceToClientDependency(cssName, ClientDependencyType.Css);
        }

        /// <summary>
        /// Adds the JS required for the MultiNodeTreePicker
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddJsMNTPClientDependencies(this Control ctl)
        {
            var jsMultiNodePicker = "uComponents.Core.DataTypes.MultiNodeTreePicker.MultiNodePickerScripts.js";
            ctl.AddResourceToClientDependency(jsMultiNodePicker, ClientDependencyType.Javascript);
            var tooltip = "uComponents.Core.Shared.Resources.Scripts.jquery.tooltip.min.js";
            ctl.AddResourceToClientDependency(tooltip, ClientDependencyType.Javascript);
        }

    }
}
