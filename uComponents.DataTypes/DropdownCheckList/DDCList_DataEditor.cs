using System;
using System.Collections;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClientDependency.Core;
using uComponents.Core;
using uComponents.Core.Extensions;

[assembly: WebResource("uComponents.DataTypes.DropdownCheckList.Styles.uiDropdownchecklist.css", Constants.MediaTypeNames.Text.Css, PerformSubstitution = true)]
[assembly: WebResource("uComponents.DataTypes.DropdownCheckList.Scripts.ui.dropdownchecklist.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.DropdownCheckList.Images.dropdown.png", Constants.MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.DataTypes.DropdownCheckList.Images.dropdown_hover.png", Constants.MediaTypeNames.Image.Png)]

namespace uComponents.DataTypes.DropdownCheckList
{
	/// <summary>
	/// The DataEditor for the DropdownCheckList.
	/// </summary>
	[ClientDependency(ClientDependencyType.Javascript, "ui/jqueryui.js", "UmbracoClient")]
	[ValidationProperty("PickedValues")]
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
			this.AddResourceToClientDependency("uComponents.DataTypes.DropdownCheckList.Styles.uiDropdownchecklist.css", ClientDependencyType.Css);
			this.AddResourceToClientDependency("uComponents.DataTypes.DropdownCheckList.Scripts.ui.dropdownchecklist.js", ClientDependencyType.Javascript);
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
			var pickedValues = this.PickedValues.Split(new[] { Constants.Common.COMMA }, StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToList();

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