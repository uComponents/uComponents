using System;

using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.ElasticTextBox
{
	/// <summary>
	/// Data Editor for the ElasticTextBox data-type.
	/// </summary>
	public class ETB_DataType : AbstractDataEditor
	{
		/// <summary>
		/// The ElasticTextBox control.
		/// </summary>
		private ETB_Control m_Control = new ETB_Control();

		/// <summary>
		/// The PreValue Editor for the data-type.
		/// </summary>
		private ETB_PrevalueEditor m_PreValueEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="ETB_DataType"/> class.
		/// </summary>
		public ETB_DataType()
			: base()
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
			get
			{
				return new Guid(DataTypeConstants.ElasticTextBoxId);
			}
		}

		/// <summary>
		/// Gets the name of the data type.
		/// </summary>
		/// <value>The name of the data type.</value>
		public override string DataTypeName
		{
			get
			{
				return "uComponents: Elastic TextBox";
			}
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
					this.m_PreValueEditor = new ETB_PrevalueEditor(this);
				}

				return this.m_PreValueEditor;
			}
		}

		/// <summary>
		/// Handles the Init event of the control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void m_Control_Init(object sender, EventArgs e)
		{
			// get the options from the Prevalue Editor.
			this.m_Control.Options = ((ETB_PrevalueEditor)this.PrevalueEditor).GetPreValueOptions<ETB_Options>();

			// set the value of the control
			if (this.Data.Value != null)
			{
				this.m_Control.Text = this.Data.Value.ToString();
			}
			else
			{
				this.m_Control.Text = string.Empty;
			}
		}

		/// <summary>
		/// Datas the editor control_ on save.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void DataEditorControl_OnSave(EventArgs e)
		{
			this.Data.Value = this.m_Control.Text;
		}
	}
}
