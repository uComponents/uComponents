using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;

namespace uComponents.DataTypes.EnumDropDownList
{
	/// <summary>
	/// Enum DropDownList Data Type
	/// </summary>
	public class EnumDropDownListDataEditor : CompositeControl, IDataEditor
	{
		/// <summary>
		/// Field for the data.
		/// </summary>
		private IData data;

		/// <summary>
		/// Field for the options.
		/// </summary>
		private EnumDropDownListOptions options;

		/// <summary>
		/// Field for the CustomValidator.
		/// </summary>
		private CustomValidator customValidator = new CustomValidator();

		/// <summary>
		/// Field for the DropDownList.
		/// </summary>
		private DropDownList dropDownList = new DropDownList();

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumDropDownListDataEditor"/> class. 
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="options">The options.</param>
		internal EnumDropDownListDataEditor(IData data, EnumDropDownListOptions options)
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
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			try
			{
				var assembly = Helper.IO.GetAssembly(this.options.Assembly);
				var type = assembly.GetType(this.options.Enum);

				// Loop though enum to create drop down list items
				foreach (var name in Enum.GetNames(type))
				{
					var dropDownListItem = new ListItem(name, name);
					var fieldInfo = type.GetField(name);

					// Loop though any custom attributes that may have been applied the the curent enum item
					foreach (var customAttributeData in CustomAttributeData.GetCustomAttributes(fieldInfo))
					{
						if (customAttributeData.Constructor.DeclaringType != null && customAttributeData.Constructor.DeclaringType.Name == "EnumDropDownListAttribute" && customAttributeData.NamedArguments != null)
						{
							// Loop though each property on the EnumDropDownListAttribute
							foreach (var customAttributeNamedArguement in customAttributeData.NamedArguments)
							{
								switch (customAttributeNamedArguement.MemberInfo.Name)
								{
									case "Text":
										dropDownListItem.Text = customAttributeNamedArguement.TypedValue.Value.ToString();
										break;

									case "Value":
										dropDownListItem.Value = customAttributeNamedArguement.TypedValue.Value.ToString();
										break;

									case "Enabled":
										dropDownListItem.Enabled = (bool)customAttributeNamedArguement.TypedValue.Value;
										break;
								}
							}
						}
					}

					this.dropDownList.Items.Add(dropDownListItem);
				}
			}
			catch
			{
			}

			if (!this.options.DefaultToFirstItem)
			{
				// Add a default please select value
				this.dropDownList.Items.Insert(0, new ListItem(string.Concat(ui.Text("choose"), "..."), "-1"));
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

            // OA: Need to check if this.data.Value is not null to prevent YSODs in some cases where it is actually null, like when the editor is not directly on a document type.
			if (!this.Page.IsPostBack && this.data.Value != null)
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
		/// Called by Umbraco when saving the node
		/// </summary>
		public void Save()
		{
		    var defaultPropertyId = ((DefaultData)this.data).PropertyId;

            // OA: the property id will be 0 if the editor is rendered without being directly on a document type, so we need to check to prevent YSODs here.
            if (defaultPropertyId > 0) 
            {
			    var property = new Property(defaultPropertyId);
			    if (property.PropertyType.Mandatory && this.dropDownList.SelectedValue == "-1")
			    {
				    // Property is mandatory, but no value selected in the DropDownList
				    this.customValidator.IsValid = false;

				    DocumentType documentType = new DocumentType(property.PropertyType.ContentTypeId);
				    ContentType.TabI tab = documentType.getVirtualTabs.Where(x => x.Id == property.PropertyType.TabId).FirstOrDefault();

				    if (tab != null)
				    {
					    this.customValidator.ErrorMessage = ui.Text("errorHandling", "errorMandatory", new string[] { property.PropertyType.Alias, tab.Caption }, User.GetCurrent());
				    }
			    }
            }

			this.data.Value = this.dropDownList.SelectedValue;
		}
	}
}
