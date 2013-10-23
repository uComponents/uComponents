using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;

namespace uComponents.DataTypes.PropertyPicker
{
	/// <summary>
	/// Prevalue Editor for the Property Picker data-type.
	/// </summary>
	public class PropertyPickerPrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// DropDownList to list the object types.
		/// </summary>
		private DropDownList objectTypesDropDownList = new DropDownList() { AutoPostBack = true, ID = "objectTypesDropDownList" };

		/// <summary>
		/// DropDownList for the content types.
		/// </summary>
		private DropDownList contentTypesDropDownList = new DropDownList() { ID = "contentTypesDropDownList" };

		/// <summary>
		/// CheckBox for whether to include the default attributes for a content type.
		/// </summary>
		private CheckBox includeDefaultAttributesCheckBox = new CheckBox() { ID = "includeDefaultAttributesCheckBox" };

		/// <summary>
		/// Data object used to define the configuration status of this Prevalue Editor.
		/// </summary>
		private PropertyPickerOptions options = null;

        /// <summary>
        /// Gets the documentation URL.
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return string.Concat(base.DocumentationUrl, "/data-types/property-picker/");
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyPickerPrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public PropertyPickerPrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
            : base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
		{
		}

		/// <summary>
		/// Gets the options data object that represents the current state of this datatypes configuration
		/// </summary>
		internal PropertyPickerOptions Options
		{
			get
			{
				if (this.options == null)
				{
					// Deserialize any stored settings for this Prevalue Editor instance
					this.options = this.GetPreValueOptions<PropertyPickerOptions>();

					// If still null, ie, object couldn't be de-serialized from string value.
					if (this.options == null)
					{
						// Create a new Options data object with the default values
						this.options = new PropertyPickerOptions();
					}
				}

				return this.options;
			}
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public override void Save()
		{
			if (this.Page.IsValid)
			{
				this.Options.ObjectTypeId = this.objectTypesDropDownList.SelectedValue;
				this.Options.ContentTypeId = this.contentTypesDropDownList.SelectedValue;
				this.Options.IncludeDefaultAttributes = this.includeDefaultAttributesCheckBox.Checked;

				this.SaveAsJson(this.Options);
			}
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			this.objectTypesDropDownList.SelectedIndexChanged += new EventHandler(this.objectTypesDropDownList_SelectedIndexChanged);

			// populate the object types
			this.objectTypesDropDownList.Items.Clear();
			this.objectTypesDropDownList.Items.Add(new ListItem(string.Concat(ui.Text("general", "choose"), "..."), string.Empty));
			this.objectTypesDropDownList.Items.Add(new ListItem(uQuery.UmbracoObjectType.DocumentType.GetFriendlyName(), uQuery.UmbracoObjectType.DocumentType.GetGuid().ToString()));
			this.objectTypesDropDownList.Items.Add(new ListItem(uQuery.UmbracoObjectType.MediaType.GetFriendlyName(), uQuery.UmbracoObjectType.MediaType.GetGuid().ToString()));
			this.objectTypesDropDownList.Items.Add(new ListItem(uQuery.UmbracoObjectType.MemberType.GetFriendlyName(), uQuery.UmbracoObjectType.MemberType.GetGuid().ToString()));

			this.Controls.AddPrevalueControls(this.objectTypesDropDownList, this.contentTypesDropDownList, this.includeDefaultAttributesCheckBox);
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
				// Read in stored configuration values
				this.objectTypesDropDownList.SelectedValue = this.Options.ObjectTypeId;
				this.SetSourceContentTypes();
				this.contentTypesDropDownList.SelectedValue = this.Options.ContentTypeId.ToString();
				this.includeDefaultAttributesCheckBox.Checked = this.Options.IncludeDefaultAttributes;
			}
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow("Object Type", this.objectTypesDropDownList);
			writer.AddPrevalueRow("Content Type", this.contentTypesDropDownList);
			writer.AddPrevalueRow("Include default attributes?", "This option includes all the default attribute properties for the given content type; e.g. @id, @nodeName, etc.", this.includeDefaultAttributesCheckBox);
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the objectTypesDropDownList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void objectTypesDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SetSourceContentTypes();
		}

		/// <summary>
		/// Sets the source content types DropDownList.
		/// </summary>
		private void SetSourceContentTypes()
		{
			this.contentTypesDropDownList.Items.Clear();
			this.contentTypesDropDownList.Items.Add(new ListItem(string.Concat(ui.Text("general", "choose"), "..."), string.Empty));

			var value = this.objectTypesDropDownList.SelectedValue;
			var objectType = Guid.Empty;

			if (Guid.TryParse(value, out objectType))
			{
				foreach (var guid in CMSNode.getAllUniquesFromObjectType(objectType))
				{
					var node = new CMSNode(guid);
					this.contentTypesDropDownList.Items.Add(new ListItem(node.Text, node.Id.ToString()));
				}
			}
		}
	}
}