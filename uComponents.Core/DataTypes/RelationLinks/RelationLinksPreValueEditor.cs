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
    /// <summary>
    /// 
    /// </summary>
    public class RelationLinksPreValueEditor : AbstractJsonPrevalueEditor
    {
        /// <summary>
        /// drop down list of all relation types
        /// </summary>
        private DropDownList relationTypeDropDownList = new DropDownList();

        /// <summary>
        /// 
        /// </summary>
        private RelationLinksOptions options = null;

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationLinksPreValueEditor"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        public RelationLinksPreValueEditor(BaseDataType dataType)
            : base(dataType, DBTypes.Ntext)
        {
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.relationTypeDropDownList.ID = "relationTypeDropDownList";
            ////this.relationTypeDropDownList.AutoPostBack = true;
            this.relationTypeDropDownList.DataSource = RelationType.GetAll().OrderBy(x => x.Name);
            this.relationTypeDropDownList.DataTextField = "Name";
            this.relationTypeDropDownList.DataValueField = "Id";
            this.relationTypeDropDownList.DataBind();

            this.Controls.Add(this.relationTypeDropDownList);
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
                if (this.relationTypeDropDownList.Items.FindByValue(this.Options.RelationTypeId.ToString()) != null)
                {
                    this.relationTypeDropDownList.SelectedValue = this.Options.RelationTypeId.ToString();
                }
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public override void Save()
        {
            base.Save();

            this.Options.RelationTypeId = int.Parse(this.relationTypeDropDownList.SelectedValue);

            this.SaveAsJson(this.Options);
        }

        /// <summary>
        /// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("Relation Type", this.relationTypeDropDownList);
        }
    }
}
