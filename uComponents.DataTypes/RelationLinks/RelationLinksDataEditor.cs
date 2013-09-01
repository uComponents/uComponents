using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using umbraco;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.relation;
using umbraco.interfaces;
using umbraco.editorControls;
using umbraco.presentation.templateControls;

[assembly: WebResource("uComponents.DataTypes.RelationLinks.RelationLinks.js", Constants.MediaTypeNames.Application.JavaScript)]

namespace uComponents.DataTypes.RelationLinks
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;

    using uComponents.Core.Extensions;

    using umbraco.NodeFactory;

    /// <summary>
	/// Related Links dataeditor
	/// </summary>
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

		/// <summary>
		/// Gets a value indicating whether [treat as rich text editor].
		/// </summary>
		public virtual bool TreatAsRichTextEditor
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [show label].
		/// </summary>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="RelationLinksDataEditor"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="options">The options.</param>
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
				return ((umbraco.cms.businesslogic.datatype.DefaultData)this.data).NodeId;
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

		/// <summary>
		/// Builds the link to the related item
		/// </summary>
		/// <param name="relatedCMSNode">The related CMS node.</param>
		/// <returns></returns>
		private HtmlGenericControl BuildLinkToRelated(CMSNode relatedCMSNode)
		{
			HtmlGenericControl li = new HtmlGenericControl("li");
			HtmlAnchor a = new HtmlAnchor();            

            string img = string.Empty;

			switch (uQuery.GetUmbracoObjectType(relatedCMSNode.nodeObjectType))
			{
				case uQuery.UmbracoObjectType.Document:

					a.HRef = "javascript:jumpToEditContent(" + relatedCMSNode.Id + ");";

					// WARNING - getting the content icon cia the document api may potentially be slow
					img = "/umbraco/images/umbraco/" + uQuery.GetDocument(relatedCMSNode.Id).ContentTypeIcon;

					break;

				case uQuery.UmbracoObjectType.Media:

					a.HRef = "javascript:jumpToEditMedia(" + relatedCMSNode.Id + ");";
					img = "/umbraco/images/umbraco/" + uQuery.GetMedia(relatedCMSNode.Id).ContentTypeIcon;

					break;

				case uQuery.UmbracoObjectType.Member:

					a.HRef = "javascript:jumpToEditMember(" + relatedCMSNode.Id + ");";
					img = "/umbraco/images/umbraco/" + uQuery.GetMember(relatedCMSNode.Id).ContentTypeIcon;

					break;
			}
		  		
            // is there a macro ?
            if (string.IsNullOrWhiteSpace(this.options.MacroAlias))
            {
                // default - no macro set
                a.Controls.Add(new HtmlImage() { Src = img });
                a.Controls.Add(new LiteralControl(relatedCMSNode.Text));
            }
            else
            {
                // use macro for markup - to execute a macro, at least one item must be published - as context needed to execute macro ?

                // get the node for the current page
                Node contextNode = uQuery.GetCurrentNode();

                // if the node is null (either document is unpublished, or rendering from outside the content section)
                if (contextNode == null)
                {
                    // then get the first child node from the XML content root
                    contextNode = uQuery.GetNodesByXPath(string.Concat("descendant::*[@parentID = ", uQuery.RootNodeId, "]")).FirstOrDefault();
                }

                if (contextNode != null)
                {
                    // load the page reference
                    HttpContext.Current.Items["pageID"] = contextNode.Id;
                    //HttpContext.Current.Items["pageElements"] = new page(contextNode.Id, contextNode.Version).Elements;                                                
                }
                else
                {
                    // nothing published ! so can't run a macro
                }

                Macro macro = new Macro() { Alias = this.options.MacroAlias };
                macro.MacroAttributes.Add("id", relatedCMSNode.Id);
                a.Controls.Add(new LiteralControl(macro.RenderToString()));
            }

			li.Controls.Add(a);

			return li;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(System.EventArgs e)
		{
			base.OnLoad(e);

			this.RegisterEmbeddedClientResource("uComponents.DataTypes.RelationLinks.RelationLinks.js", ClientDependencyType.Javascript);
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public void Save()
		{
			// This datatype doesn't save any data
		}
	}
}
