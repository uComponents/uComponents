using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco;
using umbraco.NodeFactory;
using umbraco.interfaces;

namespace uComponents.DataTypes.XPathChildNodePicker
{
	/// <summary>
	/// Renders a CheckBoxList using with option nodes obtained by an XPath expression
	/// </summary>
	public class XPathChildNodePickerDataEditor : CompositeControl, IDataEditor
	{
		/// <summary>
		/// Field reference for the Data.
		/// </summary>
		private IData data;

		/// <summary>
		/// Field for the configuration options.
		/// </summary>
		private XPathChildNodePickerOptions options;

		/// <summary>
		/// 
		/// </summary>
		private PlaceHolder placeholder = new PlaceHolder();

		/// <summary>
		/// 
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
		/// Initializes a new instance of XPathChildNodePickerDataEditor
		/// </summary>
		/// <param name="data"></param>
		/// <param name="options"></param>
		internal XPathChildNodePickerDataEditor(IData data, XPathChildNodePickerOptions options)
		{
			this.data = data;
			this.options = options;
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			this.placeholder.Controls.Add(new LiteralControl("<table>"));

			var nodes = uQuery.GetNodesByXPath(this.options.XPath);

			foreach (var node in nodes)
			{
				var descendants = new List<Node>(node.GetChildNodes());
				if (descendants != null && descendants.Count > 0)
				{
					ListControl listControl;

					switch (this.options.ListControlType)
					{
						case XPathChildNodePickerOptions.ListControlTypes.DropDownList:
							listControl = new DropDownList();
							break;

						case XPathChildNodePickerOptions.ListControlTypes.RadioButtonList:
							listControl = new RadioButtonList()
							{
								RepeatColumns = 4,
								RepeatDirection = RepeatDirection.Horizontal,
								RepeatLayout = RepeatLayout.Table
							};
							break;

						case XPathChildNodePickerOptions.ListControlTypes.CheckBoxList:
						default:
							listControl = new CheckBoxList()
							{
								RepeatColumns = 4,
								RepeatDirection = RepeatDirection.Horizontal,
								RepeatLayout = RepeatLayout.Table
							};
							break;
					}

					if (listControl != null)
					{
						listControl.DataSource = descendants.ToNameIds();
						listControl.DataTextField = "Value";
						listControl.DataValueField = "Key";

						listControl.DataBind();

						this.placeholder.Controls.Add(new LiteralControl(string.Concat("<tr><th style='padding-top: 8px;'>", node.Name, "</th>")));
						this.placeholder.Controls.Add(new LiteralControl("<td valign='top' style='padding-bottom:10px;'>"));
						this.placeholder.Controls.Add(listControl);
						this.placeholder.Controls.Add(new LiteralControl("</td></tr>"));
					}
				}
			}

			this.placeholder.Controls.Add(new LiteralControl("</table>"));

			this.Controls.Add(this.placeholder);
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
				// get selected items from stored CSV of IDs
				var csv = this.data.Value.ToString();
				var selected = new List<string>(csv.Split(Constants.Common.COMMA));

				foreach (var control in this.placeholder.Controls)
				{
					if (control is ListControl)
					{
						var lst = (ListControl)control;
						foreach (ListItem item in lst.Items)
						{
							if (selected.Contains(item.Value))
							{
								item.Selected = true;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Called by Umbraco when saving the node
		/// </summary>
		public void Save()
		{
			// get all checked item values
			var selected = new List<string>();

			foreach (var control in this.placeholder.Controls)
			{
				if (control is ListControl)
				{
					var lst = (ListControl)control;
					foreach (ListItem item in lst.Items)
					{
						if (item.Selected)
						{
							selected.Add(item.Value);
						}
					}
				}
			}

			// save csv
			this.data.Value = string.Join(new string(Constants.Common.COMMA, 1), selected.ToArray());
		}
	}
}
