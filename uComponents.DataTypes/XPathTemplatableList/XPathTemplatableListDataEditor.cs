using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;

using uComponents.Core;

using umbraco;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using umbraco.editorControls;
using umbraco.interfaces;
using umbraco.NodeFactory;
using umbraco.presentation.templateControls;

using DefaultData = umbraco.cms.businesslogic.datatype.DefaultData;

[assembly: WebResource("uComponents.DataTypes.XPathTemplatableList.XPathTemplatableList.css", Constants.MediaTypeNames.Text.Css)]
[assembly: WebResource("uComponents.DataTypes.XPathTemplatableList.XPathTemplatableList.js", Constants.MediaTypeNames.Application.JavaScript)]

namespace uComponents.DataTypes.XPathTemplatableList
{
    using System.Web.Hosting;

    using ClientDependency.Core.Controls;

    using uComponents.Core.Extensions;

    /// <summary>
    /// DataEditor for the XPath Templatable List data-type.
    /// </summary>
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Javascript, "ui/jqueryui.js", "UmbracoClient")]
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Css, "ui/ui-lightness/jquery-ui.custom.css", "UmbracoClient")]
    public class XPathTemplatableListDataEditor : CompositeControl, IDataEditor
    {
        /// <summary>
        /// Field for the data.
        /// </summary>
        private IData data;

        /// <summary>
        /// local for the SourceData property
        /// </summary>
        private Dictionary<int, string> sourceData = null;

        /// <summary>
        /// Field for the options.
        /// </summary>
        private XPathTemplatableListOptions options;

        /// <summary>
        /// Control used to render an inline style for this control (sets dataype instance specific css, eg. item height)
        /// </summary>
        private HtmlGenericControl style = new HtmlGenericControl("style");

        /// <summary>
        /// Wrapping div
        /// </summary>
        private HtmlGenericControl div = new HtmlGenericControl("div");

        /// <summary>
        /// ul containing the source data in li attributes
        /// </summary>
        private HtmlGenericControl sourceListUl = new HtmlGenericControl("ul");

        /// <summary>
        /// Stores the selected values
        /// </summary>
        private HiddenField selectedItemsHiddenField = new HiddenField();

        /// <summary>
        /// Ensure number of items selected is within any min and max configuration settings
        /// </summary>
        private CustomValidator customValidator = new CustomValidator();

        /// <summary>
        /// Initializes a new instance of XPathTemplatableListDataEditor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="options"></param>
        internal XPathTemplatableListDataEditor(IData data, XPathTemplatableListOptions options)
        {
            this.data = data;
            this.options = options;
        }

        /// <summary>
        /// Gets a value indicating whether [treat as rich text editor].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [treat as rich text editor]; otherwise, <c>false</c>.
        /// </value>
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
        /// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
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
        /// Lazy loads the source data, a collection of Ids with markup for each id item
        /// </summary>
        private Dictionary<int, string> SourceData
        {
            get
            {
                if (this.sourceData == null)
                {
                    // id, string of markup to render
                    this.sourceData = new Dictionary<int, string>();
                    
                    // to execute a macro, at least one item must be published - as context needed to execute macro ?
                    Macro macro = null;

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

                    // maarkup for each item
                    string markup = string.Empty;

                    switch (this.options.UmbracoObjectType)
                    {
                        case uQuery.UmbracoObjectType.Document:

                            foreach (Node node in this.SetSortDirectionAndTake(uQuery.GetNodesByXPath(this.options.XPath)
                                                                                        .Where(x => x.Id != -1)
                                                                                        .OrderBy(x => !string.IsNullOrWhiteSpace(this.options.SortOn) ?
                                                                                                            this.options.SortOn == "Name" ? x.Name :
                                                                                                            this.options.SortOn == "CreateDate" ? x.CreateDate.ToString() :
                                                                                                            this.options.SortOn == "UpdateDate" ? x.UpdateDate.ToString() :
                                                                                                            x.GetProperty<string>(this.options.SortOn)
                                                                                                            : x.SortOrder.ToString())))
                            {
                                if (string.IsNullOrWhiteSpace(this.options.MacroAlias))
                                {
                                    markup = "<img src=\"/umbraco/images/umbraco/" + uQuery.GetDocument(node.Id).ContentTypeIcon + "\" /> " + node.Name;
                                }
                                else
                                {
                                    macro = new Macro() { Alias = this.options.MacroAlias };
                                    macro.MacroAttributes.Add("id", node.Id);

                                    markup = macro.RenderToString();
                                }

                                if (!string.IsNullOrWhiteSpace(markup))
                                {
                                    this.sourceData.Add(node.Id, WebUtility.HtmlDecode(markup));
                                }
                            }

                            break;

                        case uQuery.UmbracoObjectType.Media:

                            foreach(Media mediaItem in this.SetSortDirectionAndTake(uQuery.GetMediaByXPath(this.options.XPath)
                                                                                            .Where(x => x.Id != -1)
                                                                                            .OrderBy(x => !string.IsNullOrWhiteSpace(this.options.SortOn) ?
                                                                                                                (this.options.SortOn == "Name") ? x.Text :
                                                                                                                (this.options.SortOn == "CreateDate") ? x.CreateDateTime.ToString() :
                                                                                                                (this.options.SortOn == "UpdateDate") ? x.VersionDate.ToString() :
                                                                                                                x.GetProperty<string>(this.options.SortOn)
                                                                                                                : x.sortOrder.ToString())))
                            {
                                if (string.IsNullOrWhiteSpace(this.options.MacroAlias))
                                {
                                    markup = "<img src=\"/umbraco/images/umbraco/" + mediaItem.ContentTypeIcon + "\" /> " + mediaItem.Text;
                                }
                                else
                                {
                                    macro = new Macro() { Alias = this.options.MacroAlias };
                                    macro.MacroAttributes.Add("id", mediaItem.Id);

                                    markup = macro.RenderToString();
                                }

                                if (!string.IsNullOrWhiteSpace(markup))
                                {
                                    this.sourceData.Add(mediaItem.Id, WebUtility.HtmlDecode(markup));
                                }
                            }

                            break;

                        case uQuery.UmbracoObjectType.Member:

                            foreach(Member member in this.SetSortDirectionAndTake(uQuery.GetMembersByXPath(this.options.XPath)
                                                                                        .OrderBy(x => !string.IsNullOrWhiteSpace(this.options.SortOn) ?
                                                                                                        (this.options.SortOn == "Name") ? x.Text :
                                                                                                        (this.options.SortOn == "CreateDate") ? x.CreateDateTime.ToString() :
                                                                                                        (this.options.SortOn == "UpdateDate") ? x.VersionDate.ToString() :
                                                                                                        x.GetProperty<string>(this.options.SortOn)
                                                                                                        : x.Text.ToString())))
                            {
                                if (string.IsNullOrWhiteSpace(this.options.MacroAlias))
                                {
                                    markup = "<img src=\"/umbraco/images/umbraco/" + member.ContentTypeIcon + "\" /> " + member.Text;
                                }
                                else
                                {
                                    macro = new Macro() { Alias = this.options.MacroAlias };
                                    macro.MacroAttributes.Add("id", member.Id);

                                    markup = macro.RenderToString();
                                }

                                if (!string.IsNullOrWhiteSpace(markup))
                                {
                                    this.sourceData.Add(member.Id, WebUtility.HtmlDecode(markup));
                                }
                            }

                            break;
                    }
                }

                return this.sourceData;
            }
        }

        /// <summary>
        /// TODO: consider converting this into an extension method to make the calling code a little more readable ?
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private IEnumerable<object> SetSortDirectionAndTake(IEnumerable<object> collection)
        {
            if (this.options.SortDirection == ListSortDirection.Descending)
            {
                collection = collection.Reverse();
            }

            if (this.options.LimitTo > 0)
            {
                collection = collection.Take(this.options.LimitTo);
            }

            return collection;
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            // inline css to set item height (todo: use same pattern for list height ?)

            this.Controls.Add(this.style);

            this.div.Attributes.Add(
                    "class",
                    "xpath-templatable-list " + 
                    "datatype-id-" + ((DefaultData)this.data).DataTypeDefinitionId + " " +
                    "property-alias-" + this.Editor.ID.Replace("prop_", string.Empty)); // http://our.umbraco.org/forum/developers/extending-umbraco/7452-Document-type-alias-in-custom-datatype
                    
            this.div.Attributes.Add("data-type", this.options.Type);
            this.div.Attributes.Add("data-list-height", this.options.ListHeight.ToString());
            this.div.Attributes.Add("data-min-items", this.options.MinItems.ToString());
            this.div.Attributes.Add("data-max-items", this.options.MaxItems.ToString());
            this.div.Attributes.Add("data-allow-duplicates", this.options.AllowDuplicates.ToString());

            this.sourceListUl.Attributes.Add("class", "source-list propertypane");
            this.div.Controls.Add(this.sourceListUl);

            HtmlGenericControl sortableListUl = new HtmlGenericControl("ul");
            sortableListUl.Attributes.Add("class", "sortable-list propertypane");
            this.div.Controls.Add(sortableListUl);

            this.div.Controls.Add(this.selectedItemsHiddenField);
           
            this.Controls.Add(this.div);
            this.Controls.Add(this.customValidator);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.EnsureChildControls();

            if (!this.Page.IsPostBack)
            {
                // if item height has been set, then set inline style
                if (this.options.ItemHeight > 0)
                {
                    this.style.Controls.Add(new Literal()
                    {
                        Text = @"
                                #" + this.div.ClientID + @" > ul > li {
                                    height: " + this.options.ItemHeight + @"px;
                                }"
                    });
                }

                this.selectedItemsHiddenField.Value = this.data.Value.ToString();
            }

            this.PopulateSourceList();

            // add custom css from options
            if (!string.IsNullOrWhiteSpace(this.options.CssFile))
            {
                ClientDependencyLoader clientDependencyLoader = ClientDependencyLoader.GetInstance(new HttpContextWrapper(HttpContext.Current));
                clientDependencyLoader.RegisterDependency(this.options.CssFile, ClientDependency.Core.ClientDependencyType.Css);
            }

            // add datatype css / js
            this.RegisterEmbeddedClientResource("uComponents.DataTypes.XPathTemplatableList.XPathTemplatableList.css", ClientDependencyType.Css);
            this.RegisterEmbeddedClientResource("uComponents.DataTypes.XPathTemplatableList.XPathTemplatableList.js", ClientDependencyType.Javascript);

            // if selecting a js file, it'll read in the contents of that file server side, and pass that string as a callback to the datatype init function
            string customCallbackScript = null;
            if (!string.IsNullOrWhiteSpace(this.options.ScriptFile))
            {
                string scriptFile = HostingEnvironment.MapPath("~" + this.options.ScriptFile);
                if (scriptFile != null)
                {
                    customCallbackScript = File.ReadAllText(scriptFile);
                }                
            }

            if (string.IsNullOrWhiteSpace(customCallbackScript))
            {
                customCallbackScript = "null"; // ensure a clean js method call
            }

            string startupScript = @"
                <script language='javascript' type='text/javascript'>
                    $(document).ready(function () {
                        XPathTemplatableList.init(jQuery('div#" + this.div.ClientID + "'), " + customCallbackScript + @");
                    });
                </script>";

            ScriptManager.RegisterStartupScript(this, typeof(XPathTemplatableListDataEditor), this.ClientID + "_init", startupScript, false);
        }

        /// <summary>
        /// Called by Umbraco when saving the node
        /// </summary>
        public void Save()
        {
            var xml = this.selectedItemsHiddenField.Value;

            // TODO: fail safe validation ?

            this.data.Value = xml;
        }

        /// <summary>
        /// generates the source list markup
        /// </summary>
        private void PopulateSourceList()
        {
            foreach (KeyValuePair<int, string> dataItem in this.SourceData)
            {
                HtmlGenericControl li = new HtmlGenericControl("li");

                if (!(!this.options.AllowDuplicates && this.GetSelectedValues().Contains(dataItem.Key)))
                {
                    li.Attributes.Add("class", "active");
                }

                //li.Attributes.Add("data-text", dataItem.Value);
                li.Attributes.Add("data-value", dataItem.Key.ToString());

                HtmlGenericControl a = new HtmlGenericControl("a");
                a.Attributes.Add("class", "add");
                a.Attributes.Add("title", "add");
                a.Attributes.Add("href", "javascript:void(0);");
                a.Attributes.Add("onclick", "XPathTemplatableList.addItem(this);");

                a.Controls.Add(new Literal() { Text = dataItem.Value });

                li.Controls.Add(a);
                this.sourceListUl.Controls.Add(li);
            }
        }

        /// <summary>
        /// Gets ids for the selected items from the hidden field (in order)
        /// </summary>
        /// <returns></returns>
        private IEnumerable<int> GetSelectedValues()
        {
            //<XPathTemplatableList Type="c66ba18e-eaf3-4cff-8a22-41b16d66a972">
            //    <Item Value="1" />
            //    <Item Value="9" />
            //</XPathTemplatableList>

            if (!string.IsNullOrWhiteSpace(this.selectedItemsHiddenField.Value))
            {
                return XDocument.Parse(this.selectedItemsHiddenField.Value)
                                .Descendants("Item")
                                .Select(x => int.Parse(x.Attribute("Value").Value));
            }
            else
            {
                return new int[] { };
            }
        }
    }
}
