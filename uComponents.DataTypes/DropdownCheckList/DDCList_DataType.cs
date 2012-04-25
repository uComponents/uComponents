using System;

using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.DropdownCheckList
{
	/// <summary>
	/// The DropdownCheckList data-type.
	/// </summary>
	public class DDCList_DataType : AbstractDataEditor
	{
		/// <summary>
		/// The Prevalue Editor for the data-type.
		/// </summary>
		private NoDBOptionsKeyValuePrevalueEditor m_PreValueEditor;

		/// <summary>
		/// The DataEditor control for the data-type.
		/// </summary>
		private DDCList_DataEditor m_DropDown = new DDCList_DataEditor();

		/// <summary>
		/// Initializes a new instance of the <see cref="DDCList_DataType"/> class.
		/// </summary>
		public DDCList_DataType()
		{
			base.RenderControl = this.m_DropDown;

			this.m_DropDown.Init += new EventHandler(this.m_DropDown_Init);

			base.DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(this.DataEditorControl_OnSave);
		}

		/// <summary>
		/// Gets the name of the data-type.
		/// </summary>
		/// <value>The name of the data-type.</value>
		public override string DataTypeName
		{
			get
			{
				return "uComponents: Dropdown Check List";
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
				return new Guid(DataTypeConstants.DropdownCheckListId);
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
					this.m_PreValueEditor = new NoDBOptionsKeyValuePrevalueEditor(this, DBTypes.Ntext);
				}

				return this.m_PreValueEditor;
			}
		}

		/// <summary>
		/// Datas the editor control_ on save.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void DataEditorControl_OnSave(EventArgs e)
		{
			this.Data.Value = this.m_DropDown.PickedValues;
		}

		/// <summary>
		/// Handles the Init event of the m_DropDown control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void m_DropDown_Init(object sender, EventArgs e)
		{
			this.m_DropDown.PreValues = ((NoDBOptionsKeyValuePrevalueEditor)PrevalueEditor).Prevalues;

			if (Data != null && Data.Value != null)
			{
				this.m_DropDown.PickedValues = Data.Value.ToString();
			}
		}
	}
}