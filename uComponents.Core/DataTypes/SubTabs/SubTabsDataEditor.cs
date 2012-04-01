using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using System.Web.UI.HtmlControls;

[assembly: WebResource("uComponents.Core.DataTypes.SubTabs.SubTabs.js", Constants.MediaTypeNames.Application.JavaScript)]
namespace uComponents.Core.DataTypes.SubTabs
{
    /// <summary>
    /// Hides configured tabs, and toggles the hidden tab forms via a drop down
    /// </summary>
    public class SubTabsDataEditor : CompositeControl, IDataEditor
    {
        private IData data;

        private SubTabsOptions options;

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
                return this.options.ShowLabel;
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
        /// Initializes a new instance of the <see cref="SubTabsDataEditor"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="options">The options.</param>
        internal SubTabsDataEditor(IData data, SubTabsOptions options)
        {
            this.data = data;
            this.options = options;
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            Panel subTabsPanel = new Panel();
                        
            var tabs = uQuery.GetDocument(uQuery.GetIdFromQueryString()).ContentType.getVirtualTabs.Where(x => this.options.TabIds.Contains(x.Id));

            if (tabs.Count() > 0)
            {
                switch (this.options.SubTabType)
                {
                    case SubTabType.Buttons:

                        HtmlButton subTabButton;
                        foreach (var tab in tabs)
                        {
                            subTabButton = new HtmlButton();
                            subTabButton.InnerText = tab.Caption;
                            subTabButton.Attributes.Add("onclick", "javascript:alert('" + tab.Caption + "'); return false;");
                            subTabsPanel.Controls.Add(subTabButton);
                        }

                        break;
                    
                    case SubTabType.DropDownList:

                        DropDownList subTabDropDownList = new DropDownList();
                        foreach (var tab in tabs)
                        {
                            subTabDropDownList.Items.Add(new ListItem(tab.Caption));
                        }
                        subTabsPanel.Controls.Add(subTabDropDownList);

                        break;
                }

                subTabsPanel.ID = "subTabsPanel";
                this.Controls.Add(subTabsPanel);




                // Build the startup js
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(@"
                
                    <script language='javascript' type='text/javascript'>

                        $(document).ready(function () {

////                            var subTabsPanel = $('" + subTabsPanel.ClientID + @"');
////                            var hostTabAnchor = $('li#' + $(subTabsPanel).parentsUntil('div.tabpagescrollinglayer', 'div.tabpageContent').parent().attr('id').replace('layer_contentlayer', '') + ' > a');
////
////                            // init the first tab
////                            // if the host tab is already lit, then pass in true on last param, so that it's toggled into action                            
////
////                            activateSubTab(hostTabAnchor, subTabsPanel, " + "'undefined'" + @", $(hostTabAnchor).parent('li').hasClass('tabOn')); 
////
//////                            //TODO: loop though subtabs, and if any have 'tabOn' then init with that tab caption
//////
//////                            // when the defult host tab is clicked
//////                            //$(hostTabAnchor).click(function() { activateSubTab(this, subTabsPanel, + " + "null" + @", true); });
//////                            
//////                            // which is being used - a dropdown or buttons ?
//////                            //$(dropDown).change(function() { changeTabToDropDownView(hostTabAnchor, this, true); });
                    ");

                foreach (var tab in tabs)
                {
                    // hide the regular Umbraco tabs - TODO: is there a safer way by finding tabs that exactly contain that text ?
                    stringBuilder.Append(@"
                               $('span > nobr:contains(""" + tab.Caption + @""")').parentsUntil('li', 'a').parent().hide();
                        ");
                }

                stringBuilder.Append(@"
                            });                    
                        </script>
                    ");

                ScriptManager.RegisterStartupScript(this, typeof(SubTabsDataEditor), this.ClientID + "_init", stringBuilder.ToString(), false);
            }


        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.AddResourceToClientDependency("uComponents.Core.DataTypes.SubTabs.SubTabs.js", ClientDependencyType.Javascript);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
        }
    }
}