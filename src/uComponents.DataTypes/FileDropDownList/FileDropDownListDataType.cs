using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.FileDropDownList
{
	/// <summary>
	/// Data Editor for the File DropDownList data-type.
	/// </summary>
	public class FileDropDownListDataType : AbstractDataEditor
	{
		/// <summary>
		/// The control for the File DropDownList data-editor.
		/// </summary>
		private FileDropDownListControl m_Control = new FileDropDownListControl();

		/// <summary>
		/// The PreValue Editor for the data-type.
		/// </summary>
		private IDataPrevalue m_PreValueEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileDropDownListDataType"/> class.
		/// </summary>
		public FileDropDownListDataType()
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
				return new Guid(DataTypeConstants.FileDropDownListId);
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
				return "uComponents: File DropDownList";
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
					this.m_PreValueEditor = new FileDropDownListPrevalueEditor(this);
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
			var options = ((FileDropDownListPrevalueEditor)this.PrevalueEditor).GetPreValueOptions<FileDropDownListOptions>();

			// check if the data value is available...
			if (this.Data.Value != null)
			{
				// set the value of the control
				this.m_Control.SelectedValue = this.Data.Value.ToString();
			}

			// set the controls options
			this.m_Control.Options = options;
		}

		/// <summary>
		/// Saves the data for the editor control.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void DataEditorControl_OnSave(EventArgs e)
		{
			this.Data.Value = this.m_Control.SelectedValue;
		}
	}
}
