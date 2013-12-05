using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.TextstringArray
{
	/// <summary>
	/// Data Editor for the Textstring Array data type.
	/// </summary>
	public class TextstringArrayDataType : AbstractDataEditor
	{
		/// <summary>
		/// The control for the Textstring Array data-editor.
		/// </summary>
		private TextstringArrayControl m_Control = new TextstringArrayControl();

		/// <summary>
		/// The Data object for the data-type.
		/// </summary>
		private IData m_Data;

		/// <summary>
		/// The PreValue Editor for the data-type.
		/// </summary>
		private IDataPrevalue m_PreValueEditor;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="TextstringArrayDataType"/> class.
		/// </summary>
		public TextstringArrayDataType()
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
				return new Guid(DataTypeConstants.TextstringArrayId);
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
				return "uComponents: Textstring Array";
			}
		}

		/// <summary>
		/// Gets the data for the data-type.
		/// </summary>
		/// <value>The data for the data-type.</value>
		public override IData Data
		{
			get
			{
				if (this.m_Data == null)
				{
					this.m_Data = new TextstringArrayData(this);
				}

				return this.m_Data;
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
					this.m_PreValueEditor = new TextstringArrayPrevalueEditor(this);
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
			var options = ((TextstringArrayPrevalueEditor)this.PrevalueEditor).GetPreValueOptions<TextstringArrayOptions>();

			if (options == null)
			{
				// load defaults
				options = new TextstringArrayOptions(true);
			}

			// check if the data value is available...
			if (this.Data.Value != null)
			{
				// set the value of the control
				this.m_Control.Values = this.Data.Value.ToString();
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
			this.Data.Value = this.m_Control.Values;
		}
	}
}
