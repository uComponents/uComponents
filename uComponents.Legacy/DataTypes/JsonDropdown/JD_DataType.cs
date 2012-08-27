using System;

using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.JsonDropdown
{
	/// <summary>
	/// The ImageDropdown data-type.
	/// </summary>
	public class JD_DataType : AbstractDataEditor
	{
		/// <summary>
		/// The CheckToggle control.
		/// </summary>
		private JD_Control m_Control = new JD_Control();

		/// <summary>
		/// The PreValue Editor for the data-type.
		/// </summary>
		private JD_PrevalueEditor m_PreValueEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="JD_DataType"/> class.
		/// </summary>
		public JD_DataType()
			: base()
		{
			// set the render control as the placeholder
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
				return new Guid(DataTypeConstants.JsonDropdownId);
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
				return "uComponents-Legacy: JSON Datasource Drop Down";
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
					this.m_PreValueEditor = new JD_PrevalueEditor(this);
				}

				return this.m_PreValueEditor;
			}
		}

		/// <summary>
		/// Handles the Init event of the m_Placeholder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void m_Control_Init(object sender, EventArgs e)
		{
			// get the CheckToggle options from the Prevalue Editor.
			var options = ((JD_PrevalueEditor)this.PrevalueEditor).GetPreValueOptions<JD_Options>();

			// set the value of the control (not on PostBack)
			if (!this.m_Control.Page.IsPostBack && this.Data.Value != null && !string.IsNullOrEmpty(this.Data.Value.ToString()))
			{
				this.m_Control.Text = this.Data.Value.ToString();
			}

			// set the CheckToggle options
			this.m_Control.Options = options;
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