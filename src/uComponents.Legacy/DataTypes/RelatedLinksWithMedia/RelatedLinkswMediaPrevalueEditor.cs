using uComponents.Core;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;

namespace uComponents.DataTypes.RelatedLinksWithMedia
{
    /// <summary>
    /// The PreValue Editor for the Related Links with Media data-type.
    /// </summary>
    public class RelatedLinkswMediaPrevalueEditor : NoOptionsPrevalueEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedLinkswMediaPrevalueEditor"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        public RelatedLinkswMediaPrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
            : base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
        {
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            this.RegisterEmbeddedClientResource(typeof(DataTypeConstants), Constants.PrevalueEditorCssResourcePath, ClientDependencyType.Css);
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public string Configuration
        {
            get
            {
                return string.Empty;
            }
        }
    }
}
