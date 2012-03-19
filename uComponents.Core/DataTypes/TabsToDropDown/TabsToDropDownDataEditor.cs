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

[assembly: WebResource("uComponents.Core.DataTypes.TabsToDropDown.TabsToDropDOwn.js", MediaTypeNames.Application.JavaScript)]
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
            this.dropDownList.Attributes["onchange"] = "changeTabToDropDownView(this.value, true)";



            //what tab is this datatype on ?

            //make sure the tab this property is on isn't added to the drop down list...
            var tabs = uQuery.GetDocument(uQuery.GetIdFromQueryString()).ContentType.getVirtualTabs.Where(x => this.options.TabIds.Contains(x.Id));

            // NOTE: uQuery.GetCurrentDocument doens't work here, when item unpublished!

            if (tabs.Count() > 0)
            {

                foreach (var tab in tabs)
                {
                    this.dropDownList.Items.Add(new ListItem(tab.Caption));
                }


                this.Controls.Add(this.dropDownList);


                StringBuilder stringBuilder = new StringBuilder();


                stringBuilder.Append(@"
                
                    <script language='javascript' type='text/javascript'>

                        var hostTab;
                    





                        $(document).ready(function () {

                            hostTab = $('li#' + $('select#" + this.dropDownList.ClientID + @"').parentsUntil('div.tabpagescrollinglayer', 'div.tabpageContent').parent().attr('id').replace('layer_contentlayer', '') + ' > a');
                            
                            $(hostTab).click(function() { changeTabToDropDownView('" + tabs.First().Caption + @"', true) });
                   
                        ");


                // hide tabs
                foreach (var tab in tabs)
                {
                    stringBuilder.Append(@"

                            $('span > nobr:contains(""" + tab.Caption + @""")').parentsUntil('li', 'a').parent().hide();

                    ");


                }

                stringBuilder.Append(@"

                        });
                    

                        function changeTabToDropDownView(tabCaption, reClick) {                        


                            var tabsToDropDownProperty = $('select#" + this.dropDownList.ClientID + @"').parentsUntil('div.tabpageContent', 'div.propertypane').parent();

                            var anchor = $('span > nobr:contains(' + tabCaption + ')').parentsUntil('li', 'a');
                            var tab = $(anchor).parent();
                            var area = $('div#' + $(tab).attr('id') + 'layer_contentlayer div.tabpageContent');

                            $(tabsToDropDownProperty).moveTo(area);
    
                            if (reClick) {
                                $(anchor).click();
                            }

                            $('select#" + this.dropDownList.ClientID + @"').val(tabCaption);

                            $(hostTab).parent('li').attr('class', 'tabOn');
                        }

                    </script>
                ");

                this.literal.Text = stringBuilder.ToString();
            }



            this.Controls.Add(this.dropDownList);
            this.Controls.Add(this.literal);
            
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

