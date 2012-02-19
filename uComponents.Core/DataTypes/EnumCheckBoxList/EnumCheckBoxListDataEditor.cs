using System;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using System.Collections.Generic;
using uComponents.Core.Shared;

namespace uComponents.Core.DataTypes.EnumCheckBoxList
{
	/// <summary>
	/// Enum CheckBoxList Data Type
	/// </summary>
	public class EnumCheckBoxListDataEditor : CompositeControl, IDataEditor
	{
		/// <summary>
		/// Field for the data.
		/// </summary>
		private IData data;

		/// <summary>
		/// Field for the options.
		/// </summary>
		private EnumCheckBoxListOptions options;

		/// <summary>
		/// Field for the CustomValidator.
		/// </summary>
		private CustomValidator customValidator = new CustomValidator();

		/// <summary>
		/// Field for the CheckBoxList.
		/// </summary>
		private CheckBoxList checkBoxList = new CheckBoxList();

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
		/// Initializes a new instance of EnumCheckBoxListDataEditor
		/// </summary>
		/// <param name="data"></param>
		/// <param name="options"></param>
		internal EnumCheckBoxListDataEditor(IData data, EnumCheckBoxListOptions options)
		{
			this.data = data;
			this.options = options;
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(this.CurrentDomain_ReflectionOnlyAssemblyResolve);

			FieldInfo fieldInfo;
			ListItem checkBoxListItem;

			try
			{
				Assembly assembly;
				if (string.Equals(this.options.Assembly, "App_Code", StringComparison.InvariantCultureIgnoreCase))
				{
					assembly = Assembly.Load(this.options.Assembly);
				}
				else
				{
					assembly = Assembly.LoadFile(this.MapPathSecure(string.Concat("~/bin/", this.options.Assembly)));
				}

				var type = assembly.GetType(this.options.Enum);

				// Loop though enum to create drop down list items
				foreach (string name in Enum.GetNames(type))
				{
					checkBoxListItem = new ListItem(name, name); // Default to the enum item name

					fieldInfo = type.GetField(name);

					// Loop though any custom attributes that may have been applied the the curent enum item
					foreach (CustomAttributeData customAttributeData in CustomAttributeData.GetCustomAttributes(fieldInfo))
					{
						if (customAttributeData.Constructor.DeclaringType.Name == "EnumCheckBoxListAttribute")
						{
							// Loop though each property on the EnumCheckBoxListAttribute
							foreach (CustomAttributeNamedArgument customAttributeNamedArguement in customAttributeData.NamedArguments)
							{
								switch (customAttributeNamedArguement.MemberInfo.Name)
								{
									case "Text":
										checkBoxListItem.Text = customAttributeNamedArguement.TypedValue.Value.ToString();
										break;

									case "Value":
										checkBoxListItem.Value = customAttributeNamedArguement.TypedValue.Value.ToString();
										break;

									case "Enabled":
										checkBoxListItem.Enabled = (bool)customAttributeNamedArguement.TypedValue.Value;
										break;
								}
							}
						}
					}

					this.checkBoxList.Items.Add(checkBoxListItem);
				}
			}
			catch
			{
			}

			this.Controls.Add(this.customValidator);
			this.Controls.Add(this.checkBoxList);
		}

		private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
		{
			return Assembly.ReflectionOnlyLoad(args.Name);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.EnsureChildControls();

			if (!this.Page.IsPostBack && this.data.Value != null)
			{
				string value = this.data.Value.ToString();
				List<string> selectedValues = new List<string>();

				if (uComponents.Core.XsltExtensions.Xml.CouldItBeXml(value))
				{
					// build selected values from XML fragment
					foreach (XElement nodeXElement in XElement.Parse(value).Elements())
					{
						selectedValues.Add(nodeXElement.Value);
					}
				}
				else
				{
					// Assume a CSV source
					selectedValues = value.Split(Settings.COMMA).ToList();
				}

				// Find checkboxes where values match the stored values and set to selected
				ListItem checkBoxListItem = null;
				foreach (string selectedValue in selectedValues)
				{
					checkBoxListItem = this.checkBoxList.Items.FindByValue(selectedValue);
					if (checkBoxListItem != null)
					{
						checkBoxListItem.Selected = true;
					}
				}
			}
		}

		/// <summary>
		/// Called by Umbraco when saving the node
		/// </summary>
		public void Save()
		{
			// Get all checked item values
			IEnumerable<string> selectedOptions = from ListItem item in this.checkBoxList.Items
												  where item.Selected
												  select item.Value;

			if (this.options.UseXml)
			{
				this.data.Value = new XElement("EnumCheckBoxList",
					selectedOptions.Select(x => new XElement("enumName", x.ToString()))).ToString();
			}
			else
			{
				// Save the CSV
				this.data.Value = string.Join(",", selectedOptions.ToArray());
			}
		}
	}
}
