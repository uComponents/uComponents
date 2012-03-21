using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core.Shared.Extensions;
using uComponents.Core.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.Core.DataTypes.TabsToDropDownPanel
{
    /// <summary>
    /// Control rendered in the cms when configuring the datatype.
    /// </summary>
    public class TabsToDropDownPreValueEditor : AbstractJsonPrevalueEditor
    {
        private CheckBoxList tabsCheckBoxList = new CheckBoxList();

        private CheckBox showLabelCheckBox = new CheckBox();

        private TabsToDropDownOptions options = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabsToDropDownPreValueEditor"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        public TabsToDropDownPreValueEditor(BaseDataType dataType)
            : base(dataType, DBTypes.Ntext)
        {
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
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

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
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

            this.showLabelCheckBox.ID = "showLabelCheckBox";

            this.Controls.Add(this.tabsCheckBoxList);
            this.Controls.Add(this.showLabelCheckBox);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
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

                this.showLabelCheckBox.Checked = this.Options.ShowLabel;
            }

        }

        /// <summary>
        /// Saves the options
        /// </summary>
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

                this.Options.ShowLabel = this.showLabelCheckBox.Checked;

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
            writer.AddPrevalueRow("Show Label", this.showLabelCheckBox);
        }
    }
}
