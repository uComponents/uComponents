using System;

using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.CharLimit
{
	/// <summary>
	/// Data Editor for the CharLimit data-type.
	/// </summary>
	public class CharLimitDataType : AbstractDataEditor
	{
		/// <summary>
		/// The CharLimit control.
		/// </summary>
		private CharLimitControl m_Control = new CharLimitControl();

		/// <summary>
		/// The PreValue Editor for the data-type.
		/// </summary>
		private CharLimitPrevalueEditor m_PreValueEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="CharLimitDataType"/> class.
		/// </summary>
		public CharLimitDataType()
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
				return new Guid(DataTypeConstants.CharLimitId);
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
				return "uComponents: Character Limit";
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
					this.m_PreValueEditor = new CharLimitPrevalueEditor(this);
				}

				return this.m_PreValueEditor;
			}
		}

		/// <summary>
		/// Handles the Init event of the m_Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void m_Control_Init(object sender, EventArgs e)
		{
			// get the options from the Prevalue Editor.
			this.m_Control.Options = ((CharLimitPrevalueEditor)this.PrevalueEditor).GetPreValueOptions<CharLimitOptions>();

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