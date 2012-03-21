using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;
using System.Web.UI.HtmlControls;
using umbraco.cms.businesslogic.relation;
using uComponents.Core.uQueryExtensions;
using uComponents.Core;
using umbraco.cms.businesslogic.datatype; // DefaultData
using umbraco.cms.businesslogic;
using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;
using umbraco.NodeFactory;
using umbraco.cms.businesslogic.web;

[assembly: WebResource("uComponents.Core.DataTypes.RelationLinks.RelationLinks.js", Constants.MediaTypeNames.Application.JavaScript)]
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

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            HtmlGenericControl ul = new HtmlGenericControl("ul");

            ul.Attributes.Add("style", "list-style-type:none");

            RelationType relationType = new RelationType(this.options.RelationTypeId);
            if (relationType != null)
            {                               
                foreach (Relation relation in relationType.GetRelations(this.CurrentContentId))
                {
                    // this id could be a parent or child (or only the parent if is a one way relation)                                     
                    if (relation.Parent.Id == this.CurrentContentId)
                    {
                        ul.Controls.Add(BuildLinkToRelated(relation.Child));
                    }
                    else
                    {
                        ul.Controls.Add(BuildLinkToRelated(relation.Parent));
                    }
                }
            }

            this.Controls.Add(ul);
        }

        private HtmlGenericControl BuildLinkToRelated(CMSNode relatedCMSNode)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            HtmlAnchor a = new HtmlAnchor();
            HtmlImage img = new HtmlImage();

            // Currently supports only Documents (items in the content tree) & Media
            // TODO: add Members next and then all the other object types

            switch (uQuery.GetUmbracoObjectType(relatedCMSNode.nodeObjectType))
            {
                case uQuery.UmbracoObjectType.Document:

                    a.HRef = "javascript:jumpToEditContent(" + relatedCMSNode.Id + ");";
                    img.Src = "/umbraco/images/umbraco/" + uQuery.GetDocument(relatedCMSNode.Id).ContentTypeIcon; /// WARNING - Potentially SLOW !

                    break;

                case uQuery.UmbracoObjectType.Media:

                    a.HRef = "javascript:jumpToEditMedia(" + relatedCMSNode.Id + ");";
                    img.Src = "/umbraco/images/umbraco/" + uQuery.GetMedia(relatedCMSNode.Id).ContentTypeIcon;

                    break;
            }
          
            a.Controls.Add(img);
            a.Controls.Add(new LiteralControl(relatedCMSNode.Text));

            li.Controls.Add(a);

            return li;
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            this.AddResourceToClientDependency("uComponents.Core.DataTypes.RelationLinks.RelationLinks.js", ClientDependencyType.Javascript);
        }

        public void Save()
        {

        }
    }
}
