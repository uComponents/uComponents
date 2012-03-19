using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.XPath;
using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;
using uComponents.Core.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.DataLayer;
using System.Linq;
using System.Collections.Generic;

namespace uComponents.Core.DataTypes.TabsToDropDownPanel
{

    public class TabsToDropDownPreValueEditor : AbstractJsonPrevalueEditor
    {

        private CheckBoxList tabsCheckBoxList = new CheckBoxList();

        private TabsToDropDownOptions options = null;


        internal TabsToDropDownOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    this.options = this.GetPreValueOptions<TabsToDropDownOptions>();

                    if (this.options == null)
                    {
                        this.options = new TabsToDropDownOptions();
                    }
                }

                return this.options;
            }
        }


        public TabsToDropDownPreValueEditor(BaseDataType dataType)
            : base(dataType, DBTypes.Ntext)
        {
        }


        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            
            this.tabsCheckBoxList.ID = "tabsCheckBoxList";
            this.tabsCheckBoxList.DataSource = uQuery.SqlHelper.ExecuteReader(@"

                SELECT id, text 
                FROM cmsTab
                ORDER BY text ASC

            ");

            this.tabsCheckBoxList.DataTextField = "text";
            this.tabsCheckBoxList.DataValueField = "id";
            this.tabsCheckBoxList.DataBind();
            this.tabsCheckBoxList.RepeatColumns = 5;
            this.tabsCheckBoxList.RepeatDirection = RepeatDirection.Horizontal;

            this.Controls.Add(this.tabsCheckBoxList);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.Page.IsPostBack)
            {
                ListItem checkBoxListItem;

                foreach (int tabId in this.Options.TabIds)
                {
                    checkBoxListItem = this.tabsCheckBoxList.Items.FindByValue(tabId.ToString());
                    if (checkBoxListItem != null)
                    {
                        checkBoxListItem.Selected = true;
                    }
                }
            }

        }


        public override void Save()
        {
            if (this.Page.IsValid)
            {
                this.Options.TabIds.Clear();

                foreach (ListItem checkBoxListItem in this.tabsCheckBoxList.Items)
                {
                    if (checkBoxListItem.Selected)
                    {
                        this.Options.TabIds.Add(int.Parse(checkBoxListItem.Value));
                    }
                }

                this.SaveAsJson(this.Options);
            }
        }

        /// <summary>
        /// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("Tabs", this.tabsCheckBoxList);
        }

    }
}
