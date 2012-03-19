using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using umbraco.DataLayer;
//using ClientDependency.Core;

using System.Data.SqlClient;
using System.Data;
using System.Text;

using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;

[assembly: WebResource("uComponents.Core.DataTypes.TabsToDropDown.TabsToDropDown.js", Constants.MediaTypeNames.Application.JavaScript)]
namespace uComponents.Core.DataTypes.TabsToDropDownPanel
{

    public class TabsToDropDownDataEditor : CompositeControl, IDataEditor
    {
        /// <summary>
        /// Field for the data.
        /// </summary>
        private IData data;

        /// <summary>
        /// Field for the options.
        /// </summary>
        private TabsToDropDownOptions options;


        private DropDownList dropDownList = new DropDownList();

        private Literal literal = new Literal();



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

        internal TabsToDropDownDataEditor(IData data, TabsToDropDownOptions options)
        {
            this.data = data;
            this.options = options;
        }

        protected override void CreateChildControls()
        {
            this.dropDownList.ID = "dropDownList";

            // NOTE: uQuery.GetCurrentDocument doens't work here, when item unpublished!
            var tabs = uQuery.GetDocument(uQuery.GetIdFromQueryString()).ContentType.getVirtualTabs.Where(x => this.options.TabIds.Contains(x.Id));

            // TODO: make sure the tab this property is on isn't added to the drop down list...

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
                            changeTabToDropDownView(hostTabAnchor, dropDown, '" + tabs.First().Caption + @"', $(hostTabAnchor).parent('li').hasClass('tabOn'));

                            //TODO: loop though tabs, and if any have 'tabOn' then init with that tab caption


                            $(hostTabAnchor).click(function() { changeTabToDropDownView(this, dropDown, $(dropDown).val(), true); });
                            
                            $(dropDown).change(function() { changeTabToDropDownView(hostTabAnchor, this, this.value, true); });                   
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




        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Adds the client dependencies.
            this.AddResourceToClientDependency("uComponents.Core.DataTypes.TabsToDropDown.TabsToDropDown.js", ClientDependencyType.Javascript);
        }

        public void Save()
        {

        }
    }
}

