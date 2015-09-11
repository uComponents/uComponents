using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.PrevalueEditors;

namespace uComponents.DataTypes.RadiobuttonListWithImages
{
	/// <summary>
	/// The user interface to display to the content editor
	/// </summary>
	public class RLWI_DataEditor : CompositeControl
	{
		/// <summary>
		/// 
		/// </summary>
		protected HiddenField SelectedValues;

		/// <summary>
		/// Gets or sets the m_prevalues.
		/// </summary>
		/// <value>The m_prevalues.</value>
		public List<ImageAndAlias> m_prevalues { get; set; }

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
		/// Creates the child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			
			this.Controls.Add(this.SelectedValues);
		}

		/// <summary>
		/// Override render to control the exact output of what is rendered this includes instantiating the jquery plugin
		/// </summary>
		/// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Table);

			foreach (ImageAndAlias val in this.m_prevalues)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.AddAttribute("type", "radio");
				writer.AddAttribute("name", this.ClientID);
				writer.AddAttribute("value", val.Alias);
				writer.AddAttribute("onclick", "jQuery('#{1}').val(jQuery(this).val());".Replace("{0}", this.ClientID).Replace("{1}", this.SelectedValues.ClientID));
				if (this.PickedValues.Equals(val.Alias))
				{
					writer.AddAttribute("checked", "checked");
				}
				writer.RenderBeginTag(HtmlTextWriterTag.Input);
				writer.RenderEndTag();

				writer.RenderBeginTag(HtmlTextWriterTag.Label);
				writer.WriteLine(val.Alias);
				writer.RenderEndTag();
				writer.RenderEndTag();
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.AddAttribute(HtmlTextWriterAttribute.Src, val.ImageUrl);
				writer.RenderBeginTag(HtmlTextWriterTag.Img);
				writer.RenderEndTag();
				writer.RenderEndTag();
				writer.RenderEndTag();
			}

			writer.RenderEndTag();

			this.SelectedValues.RenderControl(writer);
		}
	}
}