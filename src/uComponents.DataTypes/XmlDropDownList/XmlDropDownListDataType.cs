using System;
using System.IO;
using System.Web.UI.WebControls;
using System.Xml;
using umbraco;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using Umbraco.Core.IO;

namespace uComponents.DataTypes.XmlDropDownList
{
	/// <summary>
	/// Data Editor for the XmlDropDownListDataType data type.
	/// </summary>
	public class XmlDropDownListDataType : AbstractDataEditor
	{
		/// <summary>
		/// The control for the DropDownList data-editor.
		/// </summary>
		private DropDownList m_Control = new DropDownList();

		/// <summary>
		/// The PreValue Editor for the data-type.
		/// </summary>
		private IDataPrevalue m_PreValueEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlDropDownListDataType"/> class.
		/// </summary>
		public XmlDropDownListDataType()
		{
			// set the render control as the placeholder
			this.RenderControl = this.m_Control;

			// assign the initialise event for the control
			this.m_Control.Init += new EventHandler(this.m_Control_Init);

			// assign the save event for the data-type/editor
			this.DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(this.DataEditorControl_OnSave);
		}

		/// <summary>
		/// Gets the id of the data-type.
		/// </summary>
		/// <value>The id of the data-type.</value>
		public override Guid Id
		{
			get { return new Guid(DataTypeConstants.XmlDropDownListId); }
		}

		/// <summary>
		/// Gets the name of the data type.
		/// </summary>
		/// <value>The name of the data type.</value>
		public override string DataTypeName
		{
			get { return "uComponents: XML DropDownList"; }
		}

		/// <summary>
		/// Gets the prevalue editor.
		/// </summary>
		/// <value>The prevalue editor.</value>
		public override IDataPrevalue PrevalueEditor
		{
			get
			{
				if (this.m_PreValueEditor == null)
				{
					this.m_PreValueEditor = new XmlDropDownListPrevalueEditor(this);
				}

				return this.m_PreValueEditor;
			}
		}

		/// <summary>
		/// Handles the Init event of the m_Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void m_Control_Init(object sender, EventArgs e)
		{
			var options = ((XmlDropDownListPrevalueEditor)this.PrevalueEditor).GetPreValueOptions<XmlDropDownListOptions>();

			if (options == null)
			{
				// load defaults
				options = new XmlDropDownListOptions(true);
			}

			if (!string.IsNullOrWhiteSpace(options.XmlFilePath) && !string.IsNullOrWhiteSpace(options.XPathExpression))
			{
				var path = IOHelper.MapPath(options.XmlFilePath);
				if (File.Exists(path))
				{
					var xml = new XmlDocument();
					xml.Load(path);

					this.m_Control.Items.Clear();
					this.m_Control.Items.Add(new ListItem(string.Concat(ui.Text("choose"), "..."), string.Empty));

					foreach (XmlNode node in xml.SelectNodes(options.XPathExpression))
					{
						var text = GetValueFromAttributeOrXPath(node, options.TextColumn);
						var value = GetValueFromAttributeOrXPath(node, options.ValueColumn);

						this.m_Control.Items.Add(new ListItem(text, value));
					}

					if (base.Data.Value != null)
					{
						this.m_Control.SelectedValue = base.Data.Value.ToString();
					}
				}
			}
		}

		/// <summary>
		/// Gets the value from attribute or XPath.
		/// </summary>
		/// <param name="node">The XML Node.</param>
		/// <param name="xpath">The XPath expression.</param>
		/// <returns></returns>
		private string GetValueFromAttributeOrXPath(XmlNode node, string xpath)
		{
			var attribute = node.Attributes[xpath];
			if (attribute != null)
				return attribute.InnerText;

			var element = node.SelectSingleNode(xpath);
			if (element != null)
				return element.InnerText;

			return string.Empty;
		}

		/// <summary>
		/// Saves the data for the editor control.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void DataEditorControl_OnSave(EventArgs e)
		{
			base.Data.Value = this.m_Control.SelectedValue;
		}
	}
}