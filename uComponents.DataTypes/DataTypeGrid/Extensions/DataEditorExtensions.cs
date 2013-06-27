// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 22.02.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Extensions
{
    using System;
    using System.Web.UI;

    using umbraco.cms.businesslogic.datatype;
    using umbraco.editorControls;

    /// <summary>
    /// Extension methods for the DataType Grid DataType
    /// </summary>
    internal static class DtgDataEditorExtensions
    {
        /// <summary>
        /// Adds the JS/CSS required for the DataType Grid
        /// </summary>
        /// <param name="ctl">The grid control.</param>
        public static void AddAllDtgClientDependencies(this DataEditor ctl)
        {
            //get the urls for the embedded resources
            AddCssDtgClientDependencies(ctl);
            AddJsDtgClientDependencies(ctl);
        }

        /// <summary>
        /// Adds the CSS required for the DataType Grid
        /// </summary>
        /// <param name="ctl">The grid control.</param>
        public static void AddCssDtgClientDependencies(this DataEditor ctl)
        {
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.DataTypeGrid.Css.DTG_DataEditor.css", ClientDependencyType.Css);
        }

        /// <summary>
        /// Adds the JS required for the DataType Grid
        /// </summary>
        /// <param name="ctl">The grid control.</param>
        public static void AddJsDtgClientDependencies(this DataEditor ctl)
        {
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.DataTypeGrid.Scripts.jquery.dataTables.min.js", ClientDependencyType.Javascript);
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.Shared.Resources.Scripts.jquery.ucomponents.js", ClientDependencyType.Javascript);
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.Shared.Resources.Scripts.jquery.ucomponents.dictionary.js", ClientDependencyType.Javascript);
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.DataTypeGrid.Scripts.DTG_DataEditor.js", ClientDependencyType.Javascript);
        }

        /// <summary>
        /// Searches the current naming container for a server control matching the function call.
        /// </summary>
        /// <typeparam name="T">The type of the control to find.</typeparam>
        /// <param name="control">The parent control.</param>
        /// <param name="func">The function.</param>
        /// <returns>The specified control, or null if the specified control does not exist.</returns>
        public static T FindControl<T>(this Control control, Func<T, bool> func) where T : Control
        {
            var result = false;
            var foundControl = control as T;

            if (foundControl != null)
            {
                result = func(foundControl);
            }

            if (control != null)
            {
                if (result)
                {
                    return control as T;
                }

                if (control.HasControls())
                {
                    foreach (Control c in control.Controls)
                    {
                        foundControl = c.FindControl(func);
                        
                        if (foundControl != null)
                        {
                            return foundControl;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Searches the current naming container for a server control with the specified <paramref name="id" /> parameter.
        /// </summary>
        /// <typeparam name="T">The type of the control to find.</typeparam>
        /// <param name="control">The parent control.</param>
        /// <param name="id">The identifier for the control to be found.</param>
        /// <returns>The specified control, or null if the specified control does not exist.</returns>
        public static T FindControl<T>(this Control control, string id) where T : Control
        {
            return control.FindControl<T>(x => x.ID == id);
        }
    }
}
