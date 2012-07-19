using System;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.RadiobuttonListWithImages
{
	/// <summary>
	/// The RadiobuttonListWithImages data-type.
	/// </summary>
	public class RLWI_DataType : AbstractDataEditor
	{
		/// <summary>
		/// The internal control to render
		/// </summary>
		RLWI_DataEditor m_RadioBoxList = new RLWI_DataEditor();

		/// <summary>
		/// Internal pre value editor to render
		/// </summary>
		MultiImageUpload_PrevalueEditor m_PreValueEditor;

		/// <summary>
		/// Constructor, setup data editor
		/// </summary>
		public RLWI_DataType()
			: base()
		{
			base.RenderControl = m_RadioBoxList;

			this.m_RadioBoxList.Init += new EventHandler(this.m_RadioBoxList_Init);

			base.DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(this.DataEditorControl_OnSave);
		}

		/// <summary>
		/// Handles the Init event of the m_RadioBoxList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void m_RadioBoxList_Init(object sender, EventArgs e)
		{
			this.m_RadioBoxList.m_prevalues = ((MultiImageUpload_PrevalueEditor)this.PrevalueEditor).Prevalues;

			if (this.Data != null && this.Data.Value != null)
			{
				this.m_RadioBoxList.PickedValues = this.Data.Value.ToString();
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
				return new Guid(DataTypeConstants.RadioButtonListWithImagesId);
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
				return "uComponents (Legacy): RadioButtonList with images";
			}
		}
		
		/// <summary>
		/// return a custom pre value editor
		/// </summary>
		public override IDataPrevalue PrevalueEditor
		{
			get
			{
				if (this.m_PreValueEditor == null)
				{
					this.m_PreValueEditor = new MultiImageUpload_PrevalueEditor(this);
				}

				return this.m_PreValueEditor;
			}
		}

		/// <summary>
		/// Handle the saving event, need to give data to Umbraco
		/// </summary>
		/// <param name="e"></param>
		private void DataEditorControl_OnSave(EventArgs e)
		{
			this.Data.Value = this.m_RadioBoxList.PickedValues;
		}
	}
}