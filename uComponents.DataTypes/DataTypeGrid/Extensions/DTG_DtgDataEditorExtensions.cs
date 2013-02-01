// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 22.02.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Extensions
{
    using uComponents.DataTypes.Shared.Extensions;
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
        /// <param name="ctl">The CTL.</param>
        public static void AddAllDtgClientDependencies(this DataEditor ctl)
        {
            //get the urls for the embedded resources
            AddCssDtgClientDependencies(ctl);
            AddJsDtgClientDependencies(ctl);
        }

        /// <summary>
        /// Adds the CSS required for the DataType Grid
        /// </summary>
        /// <param name="ctl">The CTL.</param>
        public static void AddCssDtgClientDependencies(this DataEditor ctl)
        {
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.DataTypeGrid.Css.DTG_DataEditor.css", ClientDependencyType.Css);
        }

        /// <summary>
        /// Adds the JS required for the DataType Grid
        /// </summary>
        /// <param name="ctl">The CTL.</param>
        public static void AddJsDtgClientDependencies(this DataEditor ctl)
        {
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.DataTypeGrid.Scripts.jquery.dataTables.min.js", ClientDependencyType.Javascript);            
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.Shared.Resources.Scripts.jquery.ucomponents.js", ClientDependencyType.Javascript);
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.Shared.Resources.Scripts.jquery.ucomponents.dictionary.js", ClientDependencyType.Javascript);
            ctl.RegisterEmbeddedClientResource("uComponents.DataTypes.DataTypeGrid.Scripts.DTG_DataEditor.js", ClientDependencyType.Javascript);
        }
    }
}
