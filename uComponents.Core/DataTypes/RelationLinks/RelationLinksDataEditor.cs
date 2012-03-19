using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;

namespace uComponents.Core.DataTypes.RelationLinks
{

    public class RelationLinksDataEditor : CompositeControl, IDataEditor
    {
        /// <summary>
        /// Field for the data.
        /// </summary>
        private IData data;

        /// <summary>
        /// Field for the options.
        /// </summary>
        private RelationLinksOptions options;

        public virtual bool TreatAsRichTextEditor
        {
            get
            {
                return false;
            }
        }


        public virtual bool ShowLabel
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the editor.
        /// </summary>
        /// <value>The editor.</value>
        public Control Editor
        {
            get
            {
                return this;
            }
        }


        internal RelationLinksDataEditor(IData data, RelationLinksOptions options)
        {
            this.data = data;
            this.options = options;
        }


        protected override void CreateChildControls()
        {
        }


        public void Save()
        {

        }
    }
}
