using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using umbraco;
using umbraco.cms;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;
using umbraco.interfaces;

namespace uComponents.DataTypes.IncrementalTextBox
{
	/// <summary>
	/// The IncrementalTextBox data-type.
	/// </summary>
	public class IT_DataType : AbstractDataEditor
	{
		/// <summary>
		/// The Data Editor for the data-type.
		/// </summary>
		private IT_DataEditor m_TextBox = new IT_DataEditor();

		/// <summary>
		/// The Prevalue Editor for the data-type.
		/// </summary>
		private IT_PrevalueEditor m_PrevalueEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="IT_DataType"/> class.
		/// </summary>
		public IT_DataType()
			: base()
		{
			this.RenderControl = this.m_TextBox;

			this.m_TextBox.Init += new EventHandler(this.m_TextBox_Init);

			this.DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(this.DataEditorControl_OnSave);
		}

		/// <summary>
		/// Gets the name of the data type.
		/// </summary>
		/// <value>The name of the data type.</value>
		public override string DataTypeName
		{
			get
			{
				return "uComponents: Incremental TextBox";
			}
		}

		/// <summary>
		/// Gets the id of the data-type.
		/// </summary>
		/// <value>The id of the data-type.</value>
		public override Guid Id
		{
			get
			{
				return new Guid(DataTypeConstants.IncrementalTextBoxId);
			}
		}

		/// <summary>
		/// return a custom pre value editor
		/// </summary>
		public override IDataPrevalue PrevalueEditor
		{
			get
			{
				if (this.m_PrevalueEditor == null)
				{
					this.m_PrevalueEditor = new IT_PrevalueEditor(this);
				}

				return this.m_PrevalueEditor;
			}
		}

		/// <summary>
		/// Datas the editor control_ on save.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void DataEditorControl_OnSave(EventArgs e)
		{
			this.Data.Value = this.m_TextBox.Text;
		}

		/// <summary>
		/// Handles the Init event of the m_TextBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void m_TextBox_Init(object sender, EventArgs e)
		{
			this.m_TextBox.m_prevalues = (IT_PrevalueEditor)this.PrevalueEditor;

			if (this.Data.Value != null && !string.IsNullOrEmpty(this.Data.Value.ToString()))
			{
				this.m_TextBox.Text = this.Data.Value.ToString();
			}
			else
			{
				this.m_TextBox.Text = "0";
			}
		}
	}
}