using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using ClientDependency.Core;
using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;

[assembly: WebResource("uComponents.Core.DataTypes.DropdownCheckList.css.uiDropdownchecklist.css", MediaTypeNames.Text.Css, PerformSubstitution = true)]
[assembly: WebResource("uComponents.Core.DataTypes.DropdownCheckList.scripts.ui.dropdownchecklist.js", MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.Core.Shared.Resources.Scripts.ui.core.js", MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.Core.DataTypes.DropdownCheckList.images.dropdown.png", MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.Core.DataTypes.DropdownCheckList.images.dropdown_hover.png", MediaTypeNames.Image.Png)]

namespace uComponents.Core.DataTypes.DropdownCheckList
{
	/// <summary>
	/// The DataEditor for the DropdownCheckList.
	/// </summary>
	public class DDCList_DataEditor : CompositeControl
	{
		/// <summary>
		/// The HiddenField for the selected values.
		/// </summary>
		protected HiddenField SelectedValues;

		/// <summary>
		/// Gets or sets the pre-values.
		/// </summary>
		/// <value>The pre-values.</value>
		public SortedList PreValues { get; set; }

		/// <summary>
		/// The DropDownList control.
		/// </summary>
		protected DropDownList DropDownList;

		/// <summary>
		/// Gets or sets the picked values.
		/// </summary>
		/// <value>The picked values.</value>
		public string PickedValues
		{
			get
			{
				return this.SelectedValues.Value;
			}
			set
			{
				if (this.SelectedValues == null)
				{
					this.SelectedValues = new HiddenField();
					this.SelectedValues.ID = string.Concat(this.ID, "SelectedValuesHiddenField");
				}

				this.SelectedValues.Value = value;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// get the urls for the embedded resources
			this.AddResourceToClientDependency("uComponents.Core.DataTypes.DropdownCheckList.css.uiDropdownchecklist.css", ClientDependencyType.Css);
			this.AddResourceToClientDependency("uComponents.Core.DataTypes.DropdownCheckList.scripts.ui.dropdownchecklist.js", ClientDependencyType.Javascript);
			this.AddResourceToClientDependency("uComponents.Core.Shared.Resources.Scripts.ui.core.js", ClientDependencyType.Javascript);
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.EnsureChildControls();

			this.DropDownList = new DropDownList();
			this.DropDownList.Attributes.Add("multiple", string.Empty);
			this.DropDownList.ID = string.Concat(this.ClientID, "DropdownList");

			this.Controls.Add(this.SelectedValues);
			this.Controls.Add(this.DropDownList);
		}

		/// <summary>
		/// Writes the <see cref="T:System.Web.UI.WebControls.CompositeControl"/> content to the specified <see cref="T:System.Web.UI.HtmlTextWriter"/> object, for display on the client.
		/// </summary>
		/// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			var pickedValues = this.PickedValues.Split(new[] { Settings.COMMA }, StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToList();

			foreach (object key in this.PreValues.Keys)
			{
				var value = this.PreValues[key].ToString();
				var item = new ListItem(value);

				if (pickedValues.Contains(value))
				{
					item.Attributes.Add("selected", "selected");
				}

				this.DropDownList.Items.Add(item);
			}

			this.SelectedValues.RenderControl(writer);
			this.DropDownList.RenderControl(writer);

			var functions = @"
				$('#{0}').dropdownchecklist();
				$('#{0}').change(function() {
					var title = $('#{0}').parent().find('span.ui-dropdownchecklist-text').attr('title');
					var items = $('#{0}').parent().find('span.ui-dropdownchecklist-text');
					$('#{1}').val(title);
				});
				".Replace("{0}", this.DropDownList.ClientID).Replace("{1}", this.SelectedValues.ClientID);
			var javascript = string.Concat("<script type='text/javascript'>jQuery(document).ready(function() { ", functions, "});</script>");
			writer.WriteLine(javascript);
		}
	}
}