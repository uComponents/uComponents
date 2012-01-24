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
namespace uComponents.Core.DataTypes.RadiobuttonListWithImages
{
	/// <summary>
	/// Client extensions for RadioButtonList (with Images).
	/// </summary>
    public static class ClientExtensions
    {
        /// <summary>
        /// Adds the JS/CSS required for the DropdownCheckList
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddAllClientDependencies(this Control ctl)
        {
            //get the urls for the embedded resources
            AddCssClientDependencies(ctl);            
        }

        /// <summary>
        /// Adds the CSS required for the DropdownCheckList
        /// </summary>
        /// <param name="ctl"></param>
        public static void AddCssClientDependencies(this Control ctl)
        {
            var cssName = "uComponents.Core.DataTypes.DropdownCheckList.css.uiDropdownchecklist.css";
            ctl.AddResourceToClientDependency(cssName, ClientDependency.Core.ClientDependencyType.Css);
        }
    }
}
