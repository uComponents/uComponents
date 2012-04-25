using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;

[assembly: WebResource("uComponents.Legacy.DataTypes.ImageDropdown.Styles.ImageDropdown.css", Constants.MediaTypeNames.Text.Css, PerformSubstitution = true)]
[assembly: WebResource("uComponents.Legacy.DataTypes.ImageDropdown.Scripts.jquery.dd.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.Legacy.DataTypes.ImageDropdown.Images.dd_arrow.gif", Constants.MediaTypeNames.Image.Gif)]

namespace uComponents.DataTypes.ImageDropdown
{
	/// <summary>
	/// The DataEditor for the ImageDropdown data-type.
	/// </summary>
	[ValidationProperty("IsValid")]
	public class ID_DataEditor : CompositeControl
	{
		/// <summary>
		/// The DropDownList control.
		/// </summary>
		public DropDownList m_DropDownList;

		/// <summary>
		/// The HiddenField control.
		/// </summary>
		public HiddenField m_HiddenValue;

		/// <summary>
		/// Gets the value of IsValid.
		/// </summary>
		/// <value>Returns 'Valid' if valid, otherwise an empty string.</value>
		public string IsValid
		{
			get
			{
				if (!string.IsNullOrEmpty(this.m_HiddenValue.Value))
				{
					return "Valid";
				}

				return string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the PreValue options.
		/// </summary>
		/// <value>The PreValue options.</value>
		public List<ImageAndAlias> Options { get; set; }

		/// <summary>
		/// Gets or sets the picked value.
		/// </summary>
		/// <value>The picked value.</value>
		public string PickedValue { get; set; }

		/// <summary>
		/// Stores this instance.
		/// </summary>
		/// <remarks>
		/// NOTE: Added for compatibility with DTG [azzlack]
		/// </remarks>
		public void Store()
		{
			this.PickedValue = this.m_HiddenValue.Value;
		}

		/// <summary>
		/// Initialize the control, make sure children are created
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();
		}

		/// <summary>
		/// Add the resources (sytles/scripts)
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// get the urls for the embedded resources
			this.AddResourceToClientDependency("uComponents.Legacy.DataTypes.ImageDropdown.Scripts.jquery.dd.js", ClientDependencyType.Javascript);
			this.AddResourceToClientDependency("uComponents.Legacy.DataTypes.ImageDropdown.Styles.ImageDropdown.css", ClientDependencyType.Css);
		}

		/// <summary>
		/// Creates the child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.EnsureChildControls();

			this.m_DropDownList = new DropDownList { ID = "dropdownList" };
			this.m_HiddenValue = new HiddenField { ID = "hiddenValue", Value = this.PickedValue };

			this.Controls.Add(this.m_HiddenValue);
			this.Controls.Add(this.m_DropDownList);
		}

		/// <summary>
		/// Override render to control the exact output of what is rendered this includes instantiating the jquery plugin
		/// </summary>
		/// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			foreach (ImageAndAlias val in this.Options)
			{
				var l = new ListItem(val.Alias);
				l.Attributes.Add("title", val.ImageUrl);

				if (val.Alias == this.PickedValue)
				{
					l.Selected = true;
				}

				this.m_DropDownList.Items.Add(l);
			}

			this.m_DropDownList.Attributes["onchange"] = "javascript:changeHiddenValue('" + this.m_DropDownList.ClientID + "', '" + this.m_HiddenValue.ClientID + "');";

			this.m_HiddenValue.RenderControl(writer);
			this.m_DropDownList.RenderControl(writer);

			string javascript = string.Concat(@"
			<script type='text/javascript'>
				$(function(e) {
					try {
						$('#", this.m_DropDownList.ClientID, @"').msDropDown({visibleRows:5, rowHeight:23});
					} catch(e) {
						alert(e.message);
					}
				});
				function changeHiddenValue(dropdownlistid, hiddenvalueid) {
					$(function() {
						$('#' + hiddenvalueid).val($('#' + dropdownlistid).val());
					});
				};
			</script>");

			writer.WriteLine(javascript);
		}
	}
}