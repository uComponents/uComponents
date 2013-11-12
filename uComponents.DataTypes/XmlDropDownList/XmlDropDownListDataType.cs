using System;
using System.IO;
using System.Web.UI.WebControls;
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
					using (var xml = new XmlDataSource() { DataFile = path, XPath = options.XPathExpression })
					{
						this.m_Control.DataSource = xml;
						this.m_Control.DataTextField = options.TextColumn;
						this.m_Control.DataValueField = options.ValueColumn;
						this.m_Control.DataBind();

						this.m_Control.Items.Insert(0, new ListItem(string.Concat(ui.Text("choose"), "..."), string.Empty));
					}

					if (base.Data.Value != null)
					{
						this.m_Control.SelectedValue = base.Data.Value.ToString();
					}
				}
			}
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