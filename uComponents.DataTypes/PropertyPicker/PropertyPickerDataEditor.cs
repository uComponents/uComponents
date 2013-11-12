using System;
using System.Collections.Generic;
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

namespace uComponents.DataTypes.PropertyPicker
{
	using Umbraco.Web;

	/// <summary>
	/// Property Picker Data Type
	/// </summary>
	public class PropertyPickerDataEditor : CompositeControl, IDataEditor
	{
		/// <summary>
		/// Field for the data.
		/// </summary>
		private IData data;

		/// <summary>
		/// Field for the options.
		/// </summary>
		private PropertyPickerOptions options;

		/// <summary>
		/// Field for the CustomValidator.
		/// </summary>
		private CustomValidator customValidator = new CustomValidator();

		/// <summary>
		/// Field for the DropDownList.
		/// </summary>
		private DropDownList dropDownList = new DropDownList();

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyPickerDataEditor"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="options">The options.</param>
		public PropertyPickerDataEditor(IData data, PropertyPickerOptions options)
		{
			this.data = data;
			this.options = options;
		}

		/// <summary>
		/// Gets a value indicating whether [treat as rich text editor].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [treat as rich text editor]; otherwise, <c>false</c>.
		/// </value>
		public virtual bool TreatAsRichTextEditor
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [show label].
		/// </summary>
		/// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public void Save()
		{
			var property = new Property(((DefaultData)this.data).PropertyId);
			if (property.PropertyType.Mandatory && this.dropDownList.SelectedValue == string.Empty)
			{
				// Property is mandatory, but no value selected in the DropDownList
				this.customValidator.IsValid = false;
				
				var documentType = UmbracoContext.Current.Application.Services.ContentTypeService.GetContentType(property.PropertyType.ContentTypeId);
				var tab = documentType.PropertyGroups.FirstOrDefault(x => x.Id == property.PropertyType.PropertyTypeGroup);

				if (tab != null)
				{
					this.customValidator.ErrorMessage = ui.Text("errorHandling", "errorMandatory", new[] { property.PropertyType.Alias, tab.Name }, User.GetCurrent());
				}
			}

			this.data.Value = this.dropDownList.SelectedValue;
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			// initialise the dropdownlist
			this.dropDownList.Items.Clear();
			this.dropDownList.Items.Add(new ListItem(string.Concat(ui.Text("general", "choose"), "..."), string.Empty));

			int contentTypeId;
			if (int.TryParse(this.options.ContentTypeId, out contentTypeId))
			{
				var contentType = UmbracoContext.Current.Application.Services.ContentTypeService.GetContentType(contentTypeId);

				if (contentType != null)
				{
					foreach (var propertyType in contentType.PropertyTypes)
					{
						this.dropDownList.Items.Add(new ListItem(propertyType.Name, propertyType.Alias));
					}

					if (this.options.IncludeDefaultAttributes)
					{
						var defaultAttributes = this.GetDefaultAttributes();
						foreach (var attribute in defaultAttributes)
						{
							this.dropDownList.Items.Add(new ListItem(attribute, attribute));
						}
					}
				}
			}

			this.Controls.Add(this.customValidator);
			this.Controls.Add(this.dropDownList);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.EnsureChildControls();

			if (!this.Page.IsPostBack)
			{
				// Get selected items from Node Name or Node Id
				var dropDownListItem = this.dropDownList.Items.FindByValue(this.data.Value.ToString());
				if (dropDownListItem != null)
				{
					dropDownListItem.Selected = true;
				}
			}
		}

		/// <summary>
		/// Gets the default attributes for a content type.
		/// </summary>
		/// <returns></returns>
		private List<string> GetDefaultAttributes()
		{
			var defaultAttributes = new List<string>() { "@id", "@version", "@parentID", "@level", "@writerID", "@nodeType", "@template", "@sortOrder", "@createDate", "@updateDate", "@nodeName", "@writerName", "@nodeTypeAlias", "@path", "@urlName" };
			var guid = Guid.Empty;

			if (Guid.TryParse(this.options.ObjectTypeId, out guid))
			{
				var umbracoType = uQuery.GetUmbracoObjectType(guid);

				if (umbracoType == uQuery.UmbracoObjectType.DocumentType)
				{
					defaultAttributes.AddRange(new[] { "@createDate", "@creatorName" });
				}

				if (umbracoType == uQuery.UmbracoObjectType.MemberType)
				{
					defaultAttributes.AddRange(new[] { "@loginName", "@email", "@password" });
				}
			}

			return defaultAttributes;
		}
	}
}