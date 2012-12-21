using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.web;
using umbraco.editorControls;
using Umbraco.Core.IO;

[assembly: WebResource("uComponents.DataTypes.Similarity.SimilarityScripts.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.Similarity.SimilarityStyles.css", Constants.MediaTypeNames.Text.Css, PerformSubstitution = true)]

namespace uComponents.DataTypes.Similarity
{
    /// <summary>
    /// button to pull items, and second pane to show what is already picked
    /// </summary>
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Javascript, "ui/jqueryui.js", "UmbracoClient")]
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Javascript, "ui/jquery.tooltip.min.js", "UmbracoClient")]
    public class SimilarityDataEditor : Panel
    {

        #region public properties

        /// <summary>
        /// Gets or sets the index to search.
        /// </summary>
        /// <value>The index to search.</value>
        public string IndexToSearch { get; set; }

        /// <summary>
        /// Gets or sets the max results.
        /// </summary>
        /// <value>The max results.</value>
        public int MaxResults { get; set; }

        /// <summary>
        /// Gets or sets the properties to search.
        /// </summary>
        /// <value>The properties to search.</value>
        public IEnumerable<string> PropertiesToSearch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string _selectedNodes = string.Empty;

        /// <summary>
        /// Gets or sets the selected nodes.
        /// </summary>
        /// <value>The selected nodes.</value>
        public string SelectedNodes
        {
            get
            {
                return GetSelectedItems();
            }
            set
            {
                _selectedNodes = value;
                if (_selectedNodes != string.Empty)
                {
                    //split out and build up new list 
                    AddExistingToSelectedList();
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns the Id of the current node (if found on the querystring id value)
        /// </summary>
        private int CurrentDocId
        {
            get
            {
                int currentDocId;
                var id = uQuery.GetIdFromQueryString();

                if (int.TryParse(id, out currentDocId))
                {
                    return currentDocId;
                }

                return uQuery.RootNodeId;
            }
        }

        /// <summary>
        /// A reference path to where the icons are actually stored as compared to where the tree themes folder is
        /// </summary>
        private static string IconPath = IOHelper.ResolveUrl(SystemDirectories.Umbraco) + "/images/umbraco/";

        /// <summary>
        /// 
        /// </summary>
        protected HtmlGenericControl LeftColumn;

        /// <summary>
        /// 
        /// </summary>
        protected HtmlGenericControl RightColumn;

        /// <summary>
        /// 
        /// </summary>
        protected HtmlGenericControl UlListSelected;

        /// <summary>
        /// 
        /// </summary>
        protected Button Searchbtn;

        /// <summary>
        /// 
        /// </summary>
        protected Repeater SearchResults;

        /// <summary>
        /// 
        /// </summary>
        protected HiddenField PickedValues;

        /// <summary>
        /// 
        /// </summary>
        private HtmlGenericControl ptag = new HtmlGenericControl("p") { ID = "foundHeader", Visible = false };

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            bindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void bindData()
        {
            IEnumerable<SearchResultItem> results = FindSimilar();
            var foundHeader = LeftColumn.FindControl("foundHeader") as HtmlGenericControl;
            foundHeader.Visible = true;
            if (results.Count() != 0)
            {
                foundHeader.InnerText = string.Format(SimilarityResources.lbl_items_found, results.Count());
            }
            else
            {
                foundHeader.InnerText = SimilarityResources.lbl_no_items_found;
            }

            SearchResults.DataSource = results;
            SearchResults.DataBind();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Adds the client dependencies
            this.RegisterEmbeddedClientResource("uComponents.DataTypes.Similarity.SimilarityStyles.css", ClientDependencyType.Css);
            this.RegisterEmbeddedClientResource("uComponents.DataTypes.Similarity.SimilarityScripts.js", ClientDependencyType.Javascript);
            //this.RegisterEmbeddedClientResource("uComponents.DataTypes.Shared.Resources.Scripts.jquery.tooltip.min.js", ClientDependencyType.Javascript);
        }

        /// <summary>
        /// Adds the existing to selected list.
        /// </summary>
        private void AddExistingToSelectedList()
        {
            string[] items = _selectedNodes.Split(new[] { Constants.Common.COMMA });
            PickedValues.Value = _selectedNodes;

            // TODO: [IM] need check to see if doc still exists
            var results = (from item in items
                           where (item != string.Empty && item != ",")
                           select int.Parse(item)
                           into id let d = new Document(id)
                           select new SearchResultItem { NodeId = id, PageName = d.Text }).ToList();

            // TODO: [IM] add to list
            foreach (var searchResultItem in results)
            {
                var anchor = new HtmlGenericControl("a");
                anchor.Attributes.Add("rel", searchResultItem.NodeId.ToString());
                anchor.Attributes.Add("class", "close");
                anchor.Attributes.Add("href", "javascript:void(0)");
                var li = new HtmlGenericControl("li");
                li.Attributes.Add("rel", searchResultItem.NodeId.ToString());
                var div = new HtmlGenericControl("div") { InnerText = searchResultItem.PageName };
                var outera = new HtmlGenericControl("a");
                outera.Attributes.Add("href", "javascript:void(0)");
                outera.Attributes.Add("class", "noSpr");
                outera.Controls.Add(div);
                li.Controls.Add(anchor);
                li.Controls.Add(outera);
                UlListSelected.Controls.Add(li);
            }
        }

        /// <summary>
        /// Initialize the control, make sure children are created
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();

        }

        /// <summary>
        /// Creates the child controls for this control
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            this.EnsureChildControls();

            Searchbtn = new Button
                { ID = "FindSimilar", Text = SimilarityResources.SimilarityDataEditor_CreateChildControls_Find_similar };

            Searchbtn.Click += Searchbtn_Click;

            LeftColumn = new HtmlGenericControl("div") { ID = "LeftColumn" };
            LeftColumn.Attributes.Add("class", "left propertypane");

            RightColumn = new HtmlGenericControl("div") { ID = "RightColumn" };
            RightColumn.Attributes.Add("class", "left propertypane");

            //create the hidden field
            PickedValues = new HiddenField { ID = "PickedValue" };

            SearchResults = new Repeater { ID = "SearchResults", ItemTemplate = new SimilarItemsTemplate() };
            SearchResults.ItemDataBound += SearchResults_ItemDataBound;

            UlListSelected = new HtmlGenericControl("ul") { ID = "selectedItems" };
            UlListSelected.Attributes.Add("class", "ulSelected");

            var strongTag = new HtmlGenericControl("strong") { InnerText = SimilarityResources.lbl_items_found };
            ptag.Controls.Add(strongTag);
            LeftColumn.Controls.Add(ptag);
            LeftColumn.Controls.Add(SearchResults);
            RightColumn.Controls.Add(PickedValues);

            this.Controls.Add(Searchbtn);
            this.Controls.Add(LeftColumn);
            this.Controls.Add(PickedValues);
            this.Controls.Add(UlListSelected);
        }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            //<div class="multiTreePicker">
            //    <div class="header propertypane">
            //        <div><input type="button" text="search"/></div>
            //    </div>
            //    <div class="left propertypane">        
            //          <ul><li>node 1</li></ul>
            //    </div>
            //    <div class="right propertypane">
            //          <ul><li>node 1</li></ul>
            //    </div>
            //</div>

            RenderTooltip(writer);

            writer.AddAttribute("class", "similarity clearfix");
            writer.AddAttribute("id", this.ClientID);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute("class", "header propertypane");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            Searchbtn.RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderEndTag(); // .header

            writer.AddAttribute("class", "left propertypane");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            // add the tree control here
            ptag.RenderControl(writer);
            SearchResults.RenderControl(writer);
            writer.RenderEndTag(); // .left

            writer.AddAttribute("class", "right propertypane");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.RenderBeginTag(HtmlTextWriterTag.Strong);
            writer.Write(SimilarityResources.lbl_CurrentlySelected);
            writer.RenderEndTag(); // </strong>
            writer.RenderEndTag(); // </p>
            PickedValues.RenderControl(writer);
            UlListSelected.RenderControl(writer);
            writer.RenderEndTag(); // .right

            writer.RenderEndTag(); // .similarity
        }

        private void Searchbtn_Click(object sender, EventArgs e)
        {
            bindData();
        }

        private IEnumerable<SearchResultItem> FindSimilar()
        {
            //add 1 to max results as we will ignore first item becuase it will be 
            //current document itself
            var more = new MoreUmbracoDocsLikeThis(CurrentDocId, IndexToSearch, MaxResults + 1, PropertiesToSearch);

            List<SearchResultItem> itemsFound = more.FindMoreLikeThis().ToList();
            more.Dispose();

            return itemsFound;
        }

        private void SearchResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var liSelectNode = (HtmlGenericControl)e.Item.FindControl("SelectedNodeListItem");
            var lnkSelectNode = (HtmlAnchor)e.Item.FindControl("SelectedNodeLink");
            var litSelectNodeName = (LiteralControl)e.Item.FindControl("SelectedNodeText");

            var sr = (SearchResultItem)e.Item.DataItem;

            if (sr.NodeId != 0)
            {
                umbraco.cms.businesslogic.Content loadedNode;

                try
                {
                    loadedNode = new umbraco.cms.businesslogic.Content(sr.NodeId);

                    //add the node id
                    liSelectNode.Attributes["rel"] = sr.NodeId.ToString();

                    lnkSelectNode.HRef = "javascript:void(0);";
                    litSelectNodeName.Text = loadedNode.Text;

                    if (loadedNode.IsTrashed)
                    {
                        //need to flag this to be removed which will be done after all items are data bound
                        liSelectNode.Attributes["rel"] = "trashed";
                    }
                    else
                    {
                        //we need to set the icon
                        if (loadedNode.ContentTypeIcon.StartsWith(".spr")) lnkSelectNode.Attributes["class"] += " " + loadedNode.ContentTypeIcon.TrimStart('.');
                        else
                        {
                            //it's a real icon, so make it a background image
                            lnkSelectNode.Style.Add(
                                HtmlTextWriterStyle.BackgroundImage,
                                string.Format("url('{0}')", IconPath + loadedNode.ContentTypeIcon));
                            //set the nospr class since it's not a sprite
                            lnkSelectNode.Attributes["class"] += " noSpr";
                        }


                    }

                }
                catch (ArgumentException)
                {
                    //the node no longer exists, so we display a msg
                    litSelectNodeName.Text = string.Format("<i>{0}</i>", Helper.Dictionary.GetDictionaryItem("NodeNoLongerExists", "NODE NO LONGER EXISTS"));
                }
            }
        }

        /// <summary>
        /// this will render the tooltip object on the page so long as another 
        /// one hasn't already been registered. There should only be one tooltip.
        /// </summary>
        private void RenderTooltip(HtmlTextWriter writer)
        {
            //render the tooltip holder
            //<div class="tooltip">
            //  <div class="throbber"></div>
            //  <div class="tooltipInfo"></div>
            //</div>
            //this.Page.Controls.AddAt(0, new LiteralControl("<div id='MNTPTooltip'><div class='throbber'></div><div class='tooltipInfo'></div></div>"));
            writer.AddAttribute("id", "SimilarityTooltip");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute("class", "throbber");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag(); //end throbber
            writer.AddAttribute("class", "tooltipInfo");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag(); //end tooltipInfo
            writer.RenderEndTag(); //end tooltipo

            //ensure we add this to our page items so it's not duplicated
            this.Page.Items.Add("SimilarityTooltip", true);
        }

        private string GetSelectedItems()
        {
            return PickedValues.Value.TrimEnd(new[] { Constants.Common.COMMA });
        }
    }

    /// <summary>
    /// dto class
    /// </summary>
    public class SearchResultItem
    {
        /// <summary>
        /// Gets or sets the name of the page.
        /// </summary>
        /// <value>The name of the page.</value>
        public string PageName { get; set; }

        /// <summary>
        /// Gets or sets the node id.
        /// </summary>
        /// <value>The node id.</value>
        public int NodeId { get; set; }
    }
}
