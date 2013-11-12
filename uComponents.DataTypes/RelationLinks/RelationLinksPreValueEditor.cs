using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.relation;
using umbraco.editorControls;

namespace uComponents.DataTypes.RelationLinks
{
    using umbraco.cms.businesslogic.macro;

    /// <summary>
    /// 
    /// </summary>
	public class RelationLinksPreValueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
    {
        /// <summary>
        /// drop down list of all relation types
        /// </summary>
        private DropDownList relationTypeDropDownList = new DropDownList();

        /// <summary>
        /// drop down list to select the repeat direction
        /// </summary>
        private DropDownList repeatDirectionDropDownList = new DropDownList();

        /// <summary>
        /// drop down list to select an optional macro
        /// </summary>
        private DropDownList macroDropDownList = new DropDownList();

        /// <summary>
        /// strongly typed options
        /// </summary>
        private RelationLinksOptions options = null;

        /// <summary>
        /// Gets the documentation URL.
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return string.Concat(base.DocumentationUrl, "/data-types/relation-links/");
            }
        }

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
        public RelationLinksPreValueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
            : base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
        {
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.relationTypeDropDownList.ID = "relationTypeDropDownList";
            this.relationTypeDropDownList.DataSource = RelationType.GetAll().OrderBy(x => x.Name);
            this.relationTypeDropDownList.DataTextField = "Name";
            this.relationTypeDropDownList.DataValueField = "Id";
            this.relationTypeDropDownList.DataBind();

            this.repeatDirectionDropDownList.ID = "repeatDirectionDropDownList";
            this.repeatDirectionDropDownList.Items.Add(RepeatDirection.Vertical.ToString());
            this.repeatDirectionDropDownList.Items.Add(RepeatDirection.Horizontal.ToString());

            this.macroDropDownList.ID = "macroDropDownList";
            this.macroDropDownList.DataValueField = "Alias"; // key
            this.macroDropDownList.DataTextField = "Name";
            this.macroDropDownList.DataSource = Macro.GetAll();
            this.macroDropDownList.DataBind();
            this.macroDropDownList.Items.Insert(0, string.Empty);
            

            this.Controls.AddPrevalueControls(
                this.relationTypeDropDownList,
                this.repeatDirectionDropDownList,
                this.macroDropDownList);
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
                this.relationTypeDropDownList.SetSelectedValue(this.Options.RelationTypeId.ToString());
                this.repeatDirectionDropDownList.SetSelectedValue(this.Options.RepeatDirection.ToString());
                this.macroDropDownList.SetSelectedValue(this.Options.MacroAlias);
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public override void Save()
        {
            base.Save();

            this.Options.RelationTypeId = int.Parse(this.relationTypeDropDownList.SelectedValue);
            RepeatDirection repeatDirection;
            if (!RepeatDirection.TryParse(this.repeatDirectionDropDownList.SelectedValue, true, out repeatDirection))
            {
                repeatDirection = RepeatDirection.Vertical;
            }
            this.Options.RepeatDirection = repeatDirection;
            this.Options.MacroAlias = this.macroDropDownList.SelectedValue;

            this.SaveAsJson(this.Options);
        }

        /// <summary>
        /// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("Relation Type", "queries this relation type using the id of the current document / media or member", this.relationTypeDropDownList);
            writer.AddPrevalueRow("Repeat Direction", "list rendering direction", this.repeatDirectionDropDownList);
            writer.AddPrevalueRow("Macro Alias", "(optional) for custom rendering - expects a number parameter named 'id'", this.macroDropDownList);
        }
    }
}
