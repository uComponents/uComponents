using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using uComponents.Core.Extensions;
using uComponents.DataTypes.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using System.Web.UI.HtmlControls;

[assembly: WebResource("uComponents.DataTypes.SubTabs.SubTabs.js", Constants.MediaTypeNames.Application.JavaScript)]

namespace uComponents.DataTypes.SubTabs
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

                        int counter = 0;

                        HtmlButton subTabButton;
                        foreach (var tab in tabs)
                        {
                            subTabButton = new HtmlButton();
                            subTabButton.InnerText = tab.Caption;                            
                            subTabButton.Attributes.Add("data-tab", tab.Caption); // added an attribute to identify which tab this button relates to, as haven't yet calculated the other parms to pass into activateSubTab()
                            if (counter == 0)
                            {
                                // if it's the first button, then disable it (this means it's active tab)
                                subTabButton.Disabled = true;
                            }
                            counter++;
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

                            // used to identify the containing element for the subTabs dropdown or button controls
                            var subTabsPanel = $('div#" + subTabsPanel.ClientID + @"');

                            // the main tab anchor in which the sub tabs datatype has been placed
                            var hostTabAnchor = $('li#' + $(subTabsPanel).parentsUntil('div.tabpagescrollinglayer', 'div.tabpageContent').parent().attr('id').replace('layer_contentlayer', '') + ' > a');
                            
                            // init the first sub tab, and if it's already lit (by Umbraco) then pass in true on the last param so that it's toggled into action
                            activateSubTab(hostTabAnchor, subTabsPanel, '" + this.options.SubTabType.ToString() + @"', $(hostTabAnchor).parent('li').hasClass('tabOn')); 

                            // TODO: loop though subtabs, and if any have 'tabOn' then init with that tab caption

                            // when the defult host tab is clicked - true passed as final param to indicate that the tab is to be activated
                            $(hostTabAnchor).click(function() { activateSubTab(this, subTabsPanel, '" + this.options.SubTabType.ToString() + @"', true); });
                    ");

                switch (this.options.SubTabType)
                {
                    case SubTabType.Buttons:

                        stringBuilder.Append(@"

                            // look for all buttons that are children of the subTabsPanel, and update their click event
                            $(subTabsPanel).children('button').click(function() {
                        
                                // enable all buttons
                                $(subTabsPanel).children('button').removeAttr('disabled');

                                // disable the button being clicked (this allows us to also identify which tab should be activated within the activateSubTab method - or set a flag ?)
                                $(this).attr('disabled', true);

                                // activate the sub tab
                                activateSubTab(hostTabAnchor, subTabsPanel, '" + SubTabType.Buttons.ToString() + @"', true);

                                // prevent the button from causing a submit
                                return false;
                            });

                        ");

                        break;

                    case SubTabType.DropDownList:

                        stringBuilder.Append(@"

                            // find the drop down as a child element of the subTabsPanel (there should only be one select element)
                            $(subTabsPanel).children('select').eq(0).change(function() { 

                                activateSubTab(hostTabAnchor, subTabsPanel, '" + SubTabType.DropDownList.ToString() + @"', true); 

                            });

                        ");

                        break;
                }
                            
                foreach (var tab in tabs)
                {
                    // Hide the regular Umbraco tabs - TODO: is there a safer way by finding tabs that exactly contain that text ?
                    stringBuilder.Append(@"
                               $('span > nobr:contains(""" + tab.Caption + @""")').parentsUntil('li', 'a').parent().hide();
                        ");
                }

                // Close the script string
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

            this.AddResourceToClientDependency("uComponents.DataTypes.SubTabs.SubTabs.js", ClientDependencyType.Javascript);
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