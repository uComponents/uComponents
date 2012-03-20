using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core.Shared.Extensions;
using uComponents.Core.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.Core.DataTypes.RelationLinks
{

    public class RelationLinksPreValueEditor : AbstractJsonPrevalueEditor
    {

        private DropDownList relationTypesDropDownList = new DropDownList();

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


        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.relationTypesDropDownList.ID = "relationTypesDropDownList";



            this.Controls.Add(this.relationTypesDropDownList);

        }

        public override void Save()
        {
            base.Save();

            this.SaveAsJson(this.Options);
        }



        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);

            writer.AddPrevalueRow("Relation Type", this.relationTypesDropDownList);
        }
    }
}
