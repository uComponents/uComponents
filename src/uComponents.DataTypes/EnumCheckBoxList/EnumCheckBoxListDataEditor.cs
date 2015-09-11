using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using uComponents.Core;
using uComponents.DataTypes.Shared.Enums;
using umbraco.interfaces;

namespace uComponents.DataTypes.EnumCheckBoxList
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
		private CustomValidator customValidator = new CustomValidator() { ID = "CustomValidator" };

		/// <summary>
		/// Field for the CheckBoxList.
		/// </summary>
		private CheckBoxList checkBoxList = new CheckBoxList() { ID = "CheckBoxList" };

		/// <summary>
		/// Gets the drop down list.
		/// </summary>
		/// <value>The drop down list.</value>
		public CheckBoxList CheckBoxList
		{
			get
			{
				return this.checkBoxList;
			}
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
			try
			{
				var assembly = Helper.IO.GetAssembly(this.options.Assembly);
				var type = assembly.GetType(this.options.Enum);

				var items = EnumHelper.GetEnumListAttributeValues(type).ToArray();
				this.CheckBoxList.Items.AddRange(items);
			}
			catch
			{
			}

			this.Controls.Add(this.customValidator);
			this.Controls.Add(this.CheckBoxList);
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
				var value = this.data.Value.ToString();
				
				this.SetSelectedValues(value);
			}
		}

		/// <summary>
		/// Sets the selected values.
		/// </summary>
		/// <param name="data">The data.</param>
		public void SetSelectedValues(string data)
		{
			var selectedValues = new List<string>();

			if (Helper.Xml.CouldItBeXml(data))
			{
				// build selected values from XML fragment
				foreach (XElement nodeXElement in XElement.Parse(data).Elements())
				{
					selectedValues.Add(nodeXElement.Value);
				}
			}
			else
			{
				// Assume a CSV source
				selectedValues = data.Split(Constants.Common.COMMA).ToList();
			}

			// Find checkboxes where values match the stored values and set to selected
			foreach (var selectedValue in selectedValues)
			{
				var checkBoxListItem = this.CheckBoxList.Items.FindByValue(selectedValue);

				if (checkBoxListItem != null)
				{
					checkBoxListItem.Selected = true;
				}
			}
		}

		/// <summary>
		/// Called by Umbraco when saving the node
		/// </summary>
		public void Save()
		{
			// Get all checked item values
			IEnumerable<string> selectedOptions = from ListItem item in this.CheckBoxList.Items
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
