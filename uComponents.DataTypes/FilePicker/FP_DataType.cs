using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using umbraco.IO;

namespace uComponents.DataTypes.FilePicker
{
	/// <summary>
	/// A file picker data type for Umbraco.
	/// </summary>
	public class FP_DataType : AbstractDataEditor
	{
		/// <summary>
		/// A FilePicker control for the data-type.
		/// </summary>
		private FP_Control m_Control = new FP_Control();

		/// <summary>
		/// The PreValue Editor.
		/// </summary>
		private FP_PrevalueEditor m_PreValueEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="FP_DataType"/> class.
		/// </summary>
		public FP_DataType()
			: base()
		{
			// set the render control
			this.RenderControl = this.m_Control;

			// assign the initialise event for the placeholder
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
				return new Guid(DataTypeConstants.FilePickerId);
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
				return "uComponents: File Picker";
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
					this.m_PreValueEditor = new FP_PrevalueEditor(this);
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
			// get the prevalue options
			this.m_Control.SelectedDirectory = ((FP_PrevalueEditor)this.PrevalueEditor).SelectedDirectory.Replace("~/", string.Empty);

			// set the value of the control
			this.m_Control.Text = this.Data.Value != null ? this.Data.Value.ToString() : string.Empty;
		}

		/// <summary>
		/// Saves the editor control value.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void DataEditorControl_OnSave(EventArgs e)
		{
			// save the value of the control
			this.Data.Value = this.m_Control.Text;
		}
	}
}
