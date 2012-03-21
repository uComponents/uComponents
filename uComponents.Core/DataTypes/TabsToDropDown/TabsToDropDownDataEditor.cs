using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

[assembly: WebResource("uComponents.Core.DataTypes.TabsToDropDown.TabsToDropDown.js", Constants.MediaTypeNames.Application.JavaScript)]
namespace uComponents.Core.DataTypes.TabsToDropDownPanel
{
    /// <summary>
    /// Hides configured tabs, and toggles the hidden tab forms via a drop down
    /// </summary>
    public class TabsToDropDownDataEditor : CompositeControl, IDataEditor
    {
        private IData data;

        private TabsToDropDownOptions options;

        private DropDownList dropDownList = new DropDownList();

        private Literal literal = new Literal();

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
        /// Initializes a new instance of the <see cref="TabsToDropDownDataEditor"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="options">The options.</param>
        internal TabsToDropDownDataEditor(IData data, TabsToDropDownOptions options)
        {
            this.data = data;
            this.options = options;
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            this.dropDownList.ID = "dropDownList";

            // NOTE: uQuery.GetCurrentDocument doens't work here, when item unpublished!
            var tabs = uQuery.GetDocument(uQuery.GetIdFromQueryString()).ContentType.getVirtualTabs.Where(x => this.options.TabIds.Contains(x.Id));

            // if the tab this datatype is on is in the collection, then report an error

            if (tabs.Count() > 0)
            {
                foreach (var tab in tabs)
                {
                    this.dropDownList.Items.Add(new ListItem(tab.Caption));
                }

                this.Controls.Add(this.dropDownList);

                // Build the startup js
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(@"
                
                    <script language='javascript' type='text/javascript'>

                        $(document).ready(function () {

                            var dropDown = $('select#" + this.dropDownList.ClientID + @"');

                            var hostTabAnchor = $('li#' + $(dropDown).parentsUntil('div.tabpagescrollinglayer', 'div.tabpageContent').parent().attr('id').replace('layer_contentlayer', '') + ' > a');

                            // init the first tab - if the host tab is the first (ie lit, then pass in true on last param, so that the tab being rendered is toggled into action)
                            changeTabToDropDownView(hostTabAnchor, dropDown, $(hostTabAnchor).parent('li').hasClass('tabOn'));

                            //TODO: loop though tabs, and if any have 'tabOn' then init with that tab caption


                            $(hostTabAnchor).click(function() { changeTabToDropDownView(this, dropDown, true); });
                            
                            $(dropDown).change(function() { changeTabToDropDownView(hostTabAnchor, this, true); });
                ");
                
                foreach (var tab in tabs)
                {
                    // hide tab
                    stringBuilder.Append(@"
                           $('span > nobr:contains(""" + tab.Caption + @""")').parentsUntil('li', 'a').parent().hide();
                    ");
                }

                stringBuilder.Append(@"
                        });                    
                    </script>
                ");

                ScriptManager.RegisterStartupScript(this, typeof(TabsToDropDownDataEditor), this.ClientID + "_init", stringBuilder.ToString(), false);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.AddResourceToClientDependency("uComponents.Core.DataTypes.TabsToDropDown.TabsToDropDown.js", ClientDependencyType.Javascript);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
        }
    }
}