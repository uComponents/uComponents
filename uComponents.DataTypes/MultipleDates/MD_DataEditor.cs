using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClientDependency.Core;
using uComponents.Core;
using uComponents.Core.Extensions;
using uComponents.DataTypes.Shared.Extensions;
using umbraco.uicontrols.DatePicker;

[assembly: WebResource("uComponents.Core.DataTypes.MultipleDates.css.MultipleDates.css", Constants.MediaTypeNames.Text.Css)]
[assembly: WebResource("uComponents.Core.DataTypes.MultipleDates.scripts.main.js", Constants.MediaTypeNames.Application.JavaScript)]

namespace uComponents.DataTypes.MultipleDates
{
	/// <summary>
	/// The DataEditor for the MultipleDates data-type.
	/// </summary>
	public class MD_DataEditor : CompositeControl
	{
		/// <summary>
		/// The DateTimePicker for the control.
		/// </summary>
		protected DateTimePicker DatePicker;

		/// <summary>
		/// The HiddenField to store the selected values.
		/// </summary>
		protected HiddenField SelectedValues;

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
			this.AddResourceToClientDependency("uComponents.Core.DataTypes.MultipleDates.scripts.main.js", ClientDependencyType.Javascript);
			this.AddResourceToClientDependency("uComponents.Core.DataTypes.MultipleDates.css.MultipleDates.css", ClientDependencyType.Css);
		}

		/// <summary>
		/// Creates the child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.DatePicker = new DateTimePicker();
			this.DatePicker.ID = string.Concat(this.ID, "multipleDates");
			this.Controls.Add(this.DatePicker);

			this.Controls.Add(this.SelectedValues);
		}

		/// <summary>
		/// Override render to control the exact output of what is rendered this includes instantiating the jquery plugin
		/// </summary>
		/// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			string multilpeDateContainer = this.ClientID + "multilpeDateContainer";
			string dateListId = this.ClientID + "dateList";
			writer.AddAttribute(HtmlTextWriterAttribute.Id, multilpeDateContainer);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			this.DatePicker.RenderControl(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.Br);
			writer.RenderEndTag();
			writer.RenderBeginTag(HtmlTextWriterTag.Br);
			writer.RenderEndTag();
			writer.RenderBeginTag(HtmlTextWriterTag.Br);
			writer.RenderEndTag();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "row clearfix");
			writer.RenderBeginTag(HtmlTextWriterTag.Div); // start 'row'

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "field");
			writer.RenderBeginTag(HtmlTextWriterTag.Div); // start 'field'


			writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "addItem('{0}' ,jQuery('#{1} input.hasDatepicker').val(),'{2}');".Replace("{0}", dateListId).Replace("{1}", multilpeDateContainer).Replace("{2}", this.SelectedValues.ClientID));
			writer.AddAttribute(HtmlTextWriterAttribute.Style, "cursor:pointer;");
			writer.RenderBeginTag(HtmlTextWriterTag.A);
			writer.WriteLine("ADD DATE");
			writer.RenderEndTag();

			writer.RenderEndTag(); // end 'field'
			writer.RenderEndTag(); // end 'row'

			if (string.IsNullOrEmpty(this.SelectedValues.Value))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Style, "display: none");
			}

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "right propertypane");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, dateListId);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "date-list");
			writer.RenderBeginTag(HtmlTextWriterTag.Ul);

			foreach (string dateValue in this.SelectedValues.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Id, dateValue);
				writer.RenderBeginTag(HtmlTextWriterTag.Li);
				
				writer.WriteLine(dateValue);
				
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "deleteButton");
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "removeItem(this,'" + this.SelectedValues.ClientID + "','" + dateValue + "');");
				writer.RenderBeginTag(HtmlTextWriterTag.Div);
				
				writer.WriteLine("<img src='images/delete_button.png' height='15' width='15' />");
				
				writer.RenderEndTag(); // div.deleteButton

				writer.RenderEndTag(); // li
			}

			writer.RenderEndTag();
			writer.RenderEndTag();
			writer.RenderEndTag();

			this.SelectedValues.RenderControl(writer);

			string javascript = @"<script type='text/javascript'>jQuery(document).ready(function(){ jQuery('#{0} input.hasDatepicker').attr('disabled','disabled');  });</script>".Replace("{0}", multilpeDateContainer);
			writer.WriteLine(javascript);
		}
	}
}
