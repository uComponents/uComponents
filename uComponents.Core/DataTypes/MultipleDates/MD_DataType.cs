using System;
using uComponents.Core.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.Core.DataTypes.MultipleDates
{
	/// <summary>
	/// The MultipleDates data-type.
	/// </summary>
	public class MD_DataType : AbstractDataEditor
	{
		/// <summary>
		/// The internal control to render
		/// </summary>
		private MD_DataEditor m_Dates = new MD_DataEditor();

		/// <summary>
		/// The PreValue Editor for the data-type.
		/// </summary>
		private IDataPrevalue m_PrevalueEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="MD_DataType"/> class.
		/// </summary>
		public MD_DataType()
			: base()
		{
			base.RenderControl = m_Dates;

			this.m_Dates.Init += new EventHandler(this.m_Dates_Init);

			base.DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(this.DataEditorControl_OnSave);
		}

		/// <summary>
		/// Handles the Init event of the m_dates control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void m_Dates_Init(object sender, EventArgs e)
		{
			if (this.Data != null && this.Data.Value != null)
			{
				this.m_Dates.PickedValues = this.Data.Value.ToString();
			}
		}

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public override Guid Id
		{
			get
			{
				return new Guid(DataTypeConstants.MultipleDatesId);
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
				return "uComponents: Multiple Dates";
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
				if (this.m_PrevalueEditor == null)
				{
					this.m_PrevalueEditor = new NoOptionsPrevalueEditor(this, DBTypes.Ntext);
				}

				return this.m_PrevalueEditor;
			}
		}

		/// <summary>
		/// Handle the saving event, need to give data to Umbraco
		/// </summary>
		/// <param name="e"></param>
		void DataEditorControl_OnSave(EventArgs e)
		{
			this.Data.Value = this.m_Dates.PickedValues;
		}
	}
}
