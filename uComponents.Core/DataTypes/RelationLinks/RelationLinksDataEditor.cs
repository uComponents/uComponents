using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;
using System.Web.UI.HtmlControls;
using umbraco.cms.businesslogic.relation;
using uComponents.Core.uQueryExtensions;
using uComponents.Core;
using umbraco.cms.businesslogic.datatype; // DefaultData
using umbraco.cms.businesslogic;

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

        /// <summary>
        /// Gets the id of the current (content || media || member) node on which this datatype is a property
        /// </summary>
        private int CurrentContentId
        {
            get
            {
                return ((DefaultData)this.data).NodeId;
            }
        }

        protected override void CreateChildControls()
        {
            HtmlGenericControl ul = new HtmlGenericControl("ul");
            ul.Attributes.Add("list-style-type", "none");

            RelationType relationType = new RelationType(this.options.RelationTypeId);
            if (relationType != null)
            {                
                uQuery.UmbracoObjectType parentUmbracoObjectType = relationType.GetParentUmbracoObjectType();
                uQuery.UmbracoObjectType childUmbracoObjectType = relationType.GetChildUmbracoObjectType();
               
                CMSNode relatedCMSNode;

                HtmlGenericControl li;
                HtmlImage img;
                HtmlAnchor a;

                foreach (Relation relation in relationType.GetRelations(this.CurrentContentId))
                {
                    // this id could be a parent or child (or only the parent if is a one way relation)                                     
                    if (relation.Parent.Id == this.CurrentContentId)
                    {
                        relatedCMSNode = relation.Child;                        
                    }
                    else
                    {
                        relatedCMSNode = relation.Parent;
                    }


                    li = new HtmlGenericControl("li");
                    li.InnerText = relatedCMSNode.Text;

                    ul.Controls.Add(li);
                }
            }

            this.Controls.Add(ul);
        }




        public void Save()
        {

        }
    }
}
