using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ClientDependency.Core.Controls;
using ClientDependency.Core;
using uComponents.Core.Shared;

//embed resources
[assembly: WebResource("uComponents.Core.DataTypes.DropdownCheckList.css.uiDropdownchecklist.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("uComponents.Core.DataTypes.DropdownCheckList.scripts.ui.dropdownchecklist.js", "text/javascript")]
[assembly: WebResource("uComponents.Core.Shared.Resources.Scripts.ui.core.js", "text/javascript")]
[assembly: WebResource("uComponents.Core.DataTypes.DropdownCheckList.images.dropdown.png", "img/png")]
[assembly: WebResource("uComponents.Core.DataTypes.DropdownCheckList.images.dropdown_hover.png", "img/png")]
namespace uComponents.Core.DataTypes.DropdownCheckList
{
	/// <summary>
	/// Client extensions for the Dropdown CheckList data-type.
	/// </summary>
    public static class ClientExtensions
    {
        /// <summary>
        /// Adds the JS/CSS required for the DropdownCheckList
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddAllDDCListClientDependencies(this Control ctl)
        {
            //get the urls for the embedded resources
            var cssName = "uComponents.Core.DataTypes.DropdownCheckList.css.uiDropdownchecklist.css";
            ctl.AddResourceToClientDependency(cssName, ClientDependency.Core.ClientDependencyType.Css);

            var dropdownchecklist = "uComponents.Core.DataTypes.DropdownCheckList.scripts.ui.dropdownchecklist.js";
            ctl.AddResourceToClientDependency(dropdownchecklist, ClientDependency.Core.ClientDependencyType.Javascript);

            var uiCore = "uComponents.Core.Shared.Resources.Scripts.ui.core.js";
            ctl.AddResourceToClientDependency(uiCore, ClientDependency.Core.ClientDependencyType.Javascript);

            
        }


    }
}
