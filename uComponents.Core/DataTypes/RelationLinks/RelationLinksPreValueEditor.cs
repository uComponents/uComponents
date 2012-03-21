using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core.Shared.Extensions;
using uComponents.Core.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.relation;
using System.Linq;

namespace uComponents.Core.DataTypes.RelationLinks
{

    public class RelationLinksPreValueEditor : AbstractJsonPrevalueEditor
    {
        /// <summary>
        /// drop down list of all relation types
        /// </summary>
        private DropDownList relationTypeDropDownList = new DropDownList();

        private RelationLinksOptions options = null;

        internal RelationLinksOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    this.options = this.GetPreValueOptions<RelationLinksOptions>();

                    if (this.options == null)
                    {
                        this.options = new RelationLinksOptions();
                    }
                }

                return this.options;
            }
        }

        public RelationLinksPreValueEditor(BaseDataType dataType)
            : base(dataType, DBTypes.Ntext)
        {
        }


        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.relationTypeDropDownList.ID = "relationTypeDropDownList";
            //this.relationTypeDropDownList.AutoPostBack = true;
            this.relationTypeDropDownList.DataSource = RelationType.GetAll().OrderBy(x => x.Name);
            this.relationTypeDropDownList.DataTextField = "Name";
            this.relationTypeDropDownList.DataValueField = "Id";
            this.relationTypeDropDownList.DataBind();

            this.Controls.Add(this.relationTypeDropDownList);
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.Page.IsPostBack)
            {
                if (this.relationTypeDropDownList.Items.FindByValue(this.Options.RelationTypeId.ToString()) != null)
                {
                    this.relationTypeDropDownList.SelectedValue = this.Options.RelationTypeId.ToString();
                }
            }
        }

        public override void Save()
        {
            base.Save();

            this.Options.RelationTypeId = int.Parse(this.relationTypeDropDownList.SelectedValue);

            this.SaveAsJson(this.Options);
        }



        protected override void RenderContents(HtmlTextWriter writer)
        {           
            writer.AddPrevalueRow("Relation Type", this.relationTypeDropDownList);
        }
    }
}
