using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using ClientDependency.Core.Controls;

using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.ToggleBox
{
	/// <summary>
	/// A CheckToggle data-type for Umbraco.
	/// </summary>
	public class TB_DataType : AbstractDataEditor
	{
		/// <summary>
		/// The CheckToggle control.
		/// </summary>
		private TB_Control m_Control = new TB_Control();

		/// <summary>
		/// The PreValue Editor for the data-type.
		/// </summary>
		private TB_PrevalueEditor m_PreValueEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="TB_DataType"/> class.
		/// </summary>
		public TB_DataType()
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
				return new Guid(DataTypeConstants.ToggleBoxId);
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
				return "uComponents: ToggleBox";
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
					this.m_PreValueEditor = new TB_PrevalueEditor(this);
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
			var options = ((TB_PrevalueEditor)this.PrevalueEditor).GetPreValueOptions<TB_Options>();

			// set the value of the control (not on PostBack)
			if (!this.m_Control.Page.IsPostBack && this.Data.Value != null && !string.IsNullOrEmpty(this.Data.Value.ToString()))
			{
				this.m_Control.Checked = this.Data.Value.ToString() == "1";
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
			this.Data.Value = this.m_Control.Checked;
		}
	}
}
