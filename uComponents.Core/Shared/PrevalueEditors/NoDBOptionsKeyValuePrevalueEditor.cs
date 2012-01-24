using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using umbraco;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.Core.Shared.PrevalueEditors
{
	/// <summary>
	/// A PreValue Editor with no database options.
	/// </summary>
	public class NoDBOptionsKeyValuePrevalueEditor : AbstractPrevalueEditor
	{
		/// <summary>
		/// The underlying base data-type.
		/// </summary>
		private readonly BaseDataType m_DataType;

		/// <summary>
		/// An object to temporarily lock writing to the database.
		/// </summary>
		private static readonly object m_Locker = new object();

		/// <summary>
		/// A RequiredFieldValidator control for the RequiredControl.
		/// </summary>
		protected RequiredFieldValidator RequiredControl;

		/// <summary>
		/// A TextBox control for the TextControl.
		/// </summary>
		protected TextBox TextControl;

		/// <summary>
		/// Gets the prevalues.
		/// </summary>
		/// <value>The prevalues.</value>
		public SortedList Prevalues
		{
			get
			{
				var list = new SortedList();

				foreach (PreValue val in PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId).GetValueList())
				{
					list.Add(val.Id, val.Value);
				}

				return list;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NoDBOptionsKeyValuePrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		/// <param name="dbType">Type of the database column.</param>
		public NoDBOptionsKeyValuePrevalueEditor(BaseDataType dataType, DBTypes dbType)
			: base()
		{
			this.m_DataType = dataType;
			this.m_DataType.DBType = dbType;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (HttpContext.Current.Request.QueryString["delete"] != null)
			{
				this.Delete(HttpContext.Current.Request.QueryString["delete"]);
			}

			this.EnsureChildControls();

			this.AddResourceToClientDependency(Settings.PrevalueEditorCssResourcePath, ClientDependency.Core.ClientDependencyType.Css);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// create the controls
			this.TextControl = new TextBox() { ID = "TextId" };
			this.RequiredControl = new RequiredFieldValidator() { ID = "requiredField", ForeColor = Color.Red, ControlToValidate = this.TextControl.ID, ErrorMessage = " Field is required" };

			// add the controls
			this.Controls.AddPrevalueControls(this.TextControl, this.RequiredControl);
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow("Add prevalue:", this.TextControl, this.RequiredControl);

			// get the existing prevalues
			var prevalues = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId).GetValueList();

			// check if there are any prevalues
			if (prevalues != null && prevalues.Count > 0)
			{
				// create placeholder DIV tag
				var placeholder = new HtmlGenericControl("div");

				// loop through each of the prevalues
				foreach (PreValue value in prevalues)
				{
					// if the value is empty, then remove it
					if (string.IsNullOrEmpty(value.Value))
					{
						value.Delete();
						break;
					}

					// create row
					var row = new HtmlGenericControl("div");
					row.Attributes.Add("class", "row clearfix");

					// create the label
					var label = new HtmlGenericControl("div");
					label.Attributes.Add("class", "label");
					label.InnerText = value.Value;

					// create the field
					var field = new HtmlGenericControl("div");
					field.Attributes.Add("class", "field");

					// create the anchor
					var anchor = new System.Web.UI.HtmlControls.HtmlAnchor();
					anchor.HRef = string.Concat("?id=", this.m_DataType.DataTypeDefinitionId, "&delete=", value.Id);
					anchor.InnerText = ui.Text("delete");
					anchor.Attributes.Add("onclick", "javascript:return confirm('Are you sure you want to delete this value?');");

					// add the anchor to the field
					field.Controls.Add(anchor);

					// add the label and field to the row
					row.Controls.Add(label);
					row.Controls.Add(field);

					// add the row to the placeholder
					placeholder.Controls.Add(row);

				}

				// render the placeholder
				writer.AddPrevalueRow("Values:", placeholder);
			}
		}

		/// <summary>
		/// Deletes the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		private void Delete(string id)
		{
			foreach (PreValue val in PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId).GetValueList())
			{
				if (val.Id.ToString() == id)
				{
					val.Delete();
					return;
				}
			}
		}

		/// <summary>
		/// Gets the editor.
		/// </summary>
		/// <value>The editor.</value>
		public override Control Editor
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public override void Save()
		{
			PreValue.MakeNew(this.m_DataType.DataTypeDefinitionId, this.TextControl.Text);
		}
	}
}