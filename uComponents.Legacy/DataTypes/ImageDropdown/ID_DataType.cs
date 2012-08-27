using System;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.ImageDropdown
{
	/// <summary>
	/// The ImageDropdown data-type.
	/// </summary>
	public class ID_DataType : AbstractDataEditor
	{
		/// <summary>
		/// The internal control to render
		/// </summary>
		private ID_DataEditor m_ImageDropDown = new ID_DataEditor();

		/// <summary>
		/// Internal pre value editor to render
		/// </summary>
		private MultiImageUpload_PrevalueEditor m_PreValues;

		/// <summary>
		/// Initializes a new instance of the <see cref="ID_DataType"/> class.
		/// </summary>
		public ID_DataType()
			: base()
		{
			base.RenderControl = this.m_ImageDropDown;

			this.m_ImageDropDown.Init += new EventHandler(this.m_ImageDropDown_Init);

			base.DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(this.DataEditorControl_OnSave);
		}

		/// <summary>
		/// Handles the Init event of the m_ImageDropDown control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void m_ImageDropDown_Init(object sender, EventArgs e)
		{
			this.m_ImageDropDown.Options = ((MultiImageUpload_PrevalueEditor)this.PrevalueEditor).Prevalues;

			if (this.Data != null && this.Data.Value != null)
			{
				this.m_ImageDropDown.PickedValue = this.Data.Value.ToString();
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
				return new Guid(DataTypeConstants.ImageDropdownId);
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
				return "uComponents-Legacy: Image DropDownList";
			}
		}

		/// <summary>
		/// Handle the saving event, need to give data to Umbraco
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void DataEditorControl_OnSave(EventArgs e)
		{
			this.m_ImageDropDown.Store();
			this.Data.Value = this.m_ImageDropDown.PickedValue;
		}

		/// <summary>
		/// Returns the PrevalueEditor.
		/// </summary>
		public override IDataPrevalue PrevalueEditor
		{
			get
			{
				if (this.m_PreValues == null)
				{
					this.m_PreValues = new MultiImageUpload_PrevalueEditor(this);
				}

				return this.m_PreValues;
			}
		}
	}
}
