using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.relation;
using umbraco.macroRenderings;
using umbraco.editorControls;

namespace uComponents.DataTypes.MultiPickerRelations
{
    using uComponents.Core;

    /// <summary>
    /// This PreValueEditor will require an XPath expression to define the nodes to pick as CheckBox options,
	/// TODO: [HR] min / max selections ?
    /// Uses the shared JsonPreValueEditor as nice way of lightweight serializing a config data class object into a single DB field
    /// </summary>
	public class MultiPickerRelationsPreValueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
    {
        /// <summary>
        /// Prepopulated Umbraco Propery Picker, lists all aliases (could refine this by asking for the context in which this relation wire-up will
        /// be used, and then only listing the aliases for that context)
        /// </summary>
        private propertyTypePicker multiPickerPropertyAliasPicker = new propertyTypePicker();

        /// <summary>
        /// RequiredFieldValidator for the ProperyAliasPicker
        /// </summary>
        private RequiredFieldValidator multiPickerPropertyAliasRequiredFieldValidator = new RequiredFieldValidator();

        /// <summary>
        /// drop down list of all relation types
        /// </summary>
        private DropDownList relationTypeDropDownList = new DropDownList();

        /// <summary>
        /// RequiredFieldValidator for the RelationType DropDownList
        /// </summary>
        private RequiredFieldValidator relationTypeRequiredFieldValidator = new RequiredFieldValidator();

        /// <summary>
        /// If a parent to child relation type is selected, then this checkbox will indicate the direction to use,
        /// with reverse indexing the parents are the nodes selected via the multi-picker, and the nodeID on which the
        /// this datatype is used, become the child nodes
        /// </summary>
        private CheckBox reverseIndexingCheckBox = new CheckBox();

		/// <summary>
		/// if selected, then the property on the data editor is hidden (it's only used as a label)
		/// </summary>
		private CheckBox hideDataEditorCheckBox = new CheckBox();

        /// <summary>
        /// Data object used to define the configuration status of this PreValueEditor
        /// </summary>
        private MultiPickerRelationsOptions options = null;

        /// <summary>
        /// Currently selected RelationType
        /// </summary>
        private RelationType relationType = null;

        /// <summary>
        /// Lazy load the options data object that represents the current state of this datatypes configuration
        /// </summary>
        internal MultiPickerRelationsOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    // Deserialize any stored settings for this PreValueEditor instance
                    this.options = this.GetPreValueOptions<MultiPickerRelationsOptions>();

                    // If still null, ie, object couldn't be de-serialized from PreValue[0] string value
                    if (this.options == null)
                    {
                        // Create a new Options data object with the default values
                        this.options = new MultiPickerRelationsOptions();
                    }
                }
                return this.options;
            }
        }

        /// <summary>
        /// Lazy load currently selected RelationType
        /// </summary>
        private RelationType RelationType
        {
            get
            {
                if (this.relationType == null)
                {
                    // Attempt to get RelationType from the selected DropDownList item
                    int relationTypeId;
                    if(int.TryParse(this.relationTypeDropDownList.SelectedValue, out relationTypeId))
                    {
                        if (relationTypeId != -1)
                        {
                            this.relationType = new RelationType(relationTypeId);
                        }
                    }
                }

                return this.relationType;
            }
        }

        /// <summary>
        /// Initialize a new instance of MultiPickerRelationsPreValueEditor
        /// </summary>
        /// <param name="dataType">MultiPickerRelationsDataType</param>
        public MultiPickerRelationsPreValueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
            : base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Nvarchar)
        {
        }

        /// <summary>
        /// Creates all of the controls and assigns all of their properties
        /// </summary>
        protected override void CreateChildControls()
        {
            this.multiPickerPropertyAliasPicker.ID = "multiPickerPropertyAliasPicker";
			
            this.multiPickerPropertyAliasRequiredFieldValidator.Text = " " + Helper.Dictionary.GetDictionaryItem("Required", "Required");
            this.multiPickerPropertyAliasRequiredFieldValidator.InitialValue = string.Empty;
            this.multiPickerPropertyAliasRequiredFieldValidator.ControlToValidate = this.multiPickerPropertyAliasPicker.ID;

            this.relationTypeDropDownList.ID = "relationTypeDropDownList";
            this.relationTypeDropDownList.AutoPostBack = true;
			this.relationTypeDropDownList.DataSource = RelationType.GetAll().OrderBy(x => x.Name);
            this.relationTypeDropDownList.DataTextField = "Name";
            this.relationTypeDropDownList.DataValueField = "Id";
            this.relationTypeDropDownList.DataBind();
            this.relationTypeDropDownList.Items.Insert(0, new ListItem(string.Empty, "-1"));

            this.relationTypeRequiredFieldValidator.Text = " " + Helper.Dictionary.GetDictionaryItem("Required", "Required");
            this.relationTypeRequiredFieldValidator.InitialValue = "-1";
            this.relationTypeRequiredFieldValidator.ControlToValidate = this.relationTypeDropDownList.ID;

            this.Controls.Add(this.multiPickerPropertyAliasPicker);
            this.Controls.Add(this.multiPickerPropertyAliasRequiredFieldValidator);
            this.Controls.Add(this.relationTypeDropDownList);
            this.Controls.Add(this.relationTypeRequiredFieldValidator);
            this.Controls.Add(this.reverseIndexingCheckBox);
			this.Controls.Add(this.hideDataEditorCheckBox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.Page.IsPostBack)
            {
                // Read in stored configuration values
                if (this.multiPickerPropertyAliasPicker.Items.Contains(new ListItem(this.Options.PropertyAlias)))
                {
                    this.multiPickerPropertyAliasPicker.SelectedValue = this.Options.PropertyAlias;
                }

                if (this.relationTypeDropDownList.Items.FindByValue(this.Options.RelationTypeId.ToString()) != null)
                {
                    this.relationTypeDropDownList.SelectedValue = this.Options.RelationTypeId.ToString();
                }

                this.reverseIndexingCheckBox.Checked = this.Options.ReverseIndexing;
				
				this.hideDataEditorCheckBox.Checked = this.Options.HideDataEditor;
            }
        }

        /// <summary>
        /// Saves the pre value data to Umbraco
        /// </summary>
        public override void Save()
        {
            if (this.Page.IsValid)
            {
                this.Options.PropertyAlias = this.multiPickerPropertyAliasPicker.SelectedValue;
                if (this.RelationType != null)
                {
                    this.Options.RelationTypeId = this.RelationType.Id;
                }

                this.Options.ReverseIndexing = this.reverseIndexingCheckBox.Checked;

				this.Options.HideDataEditor = this.hideDataEditorCheckBox.Checked;

                // Serialize to Umbraco database field
                this.SaveAsJson(this.Options);
            }
        }

        /// <summary>
        /// Used to remove styling from the built in multiNodePickerProperty alias picker DropDownList 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.multiPickerPropertyAliasPicker.CssClass = string.Empty; // Remove guiInputTextStandard 

			// Sort properties in the built in property picker control
			ListItem[] propertyAliasListItems = this.multiPickerPropertyAliasPicker.Items.Cast<ListItem>().OrderBy(x => x.Text).ToArray();

			this.multiPickerPropertyAliasPicker.Items.Clear();
			this.multiPickerPropertyAliasPicker.Items.AddRange(propertyAliasListItems);
        }

        /// <summary>
        /// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("MultiPicker Alias", this.multiPickerPropertyAliasPicker, this.multiPickerPropertyAliasRequiredFieldValidator);
            writer.AddPrevalueRow("Relation Type", this.relationTypeDropDownList, this.relationTypeRequiredFieldValidator);

            // Only show this field if selected RelationType is of Parent to Child
            if (this.RelationType != null)
            {
                if (!this.RelationType.Dual)
                {
                    writer.AddPrevalueRow("Reverse Indexing", this.reverseIndexingCheckBox);
                }
            }

			writer.AddPrevalueRow("Hide Data Editor", this.hideDataEditorCheckBox);
        }
    }
}
