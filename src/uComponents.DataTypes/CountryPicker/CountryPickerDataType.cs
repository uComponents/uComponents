using System;

using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.CountryPicker
{
	/// <summary>
	/// The Country Picker data-type.
	/// </summary>
	public class CountryPickerDataType : AbstractDataEditor
	{
		/// <summary>
		/// The Prevalue Editor for the data-type.
		/// </summary>
		private CountryPickerPreValueEditor _countryPickerPreValueEditor;

		/// <summary>
		/// The Data Editor control for the data-type.
		/// </summary>
		private CountryPickerDataEditor m_Control = new CountryPickerDataEditor();

		/// <summary>
		/// Gets the id of the data-type.
		/// </summary>
		/// <value>The id of the data-type.</value>
		public override Guid Id
		{
			get
			{
				return new Guid(DataTypeConstants.CountryPickerId);
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
				return "uComponents: Country Picker";
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
				if (this._countryPickerPreValueEditor == null)
				{
					this._countryPickerPreValueEditor = new CountryPickerPreValueEditor(this);
				}

				return this._countryPickerPreValueEditor;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CountryPickerDataType"/> class.
		/// </summary>
		public CountryPickerDataType()
		{
			this.RenderControl = this.m_Control;

			this.m_Control.Init += this.m_Control_Init;

			this.DataEditorControl.OnSave += this.DataEditorControl_OnSave;
		}

		/// <summary>
		/// Handles the Init event of the m_Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void m_Control_Init(object sender, EventArgs e)
		{
			if (((CountryPickerPreValueEditor)this.PrevalueEditor).SelectedPickerType == "ListBox")
			{
				this.m_Control.IsMultiSelect = true;
			}

			this.m_Control.ChooseText = ((CountryPickerPreValueEditor)this.PrevalueEditor).ChooseText;
			this.m_Control.SelectedText = base.Data.Value != null ? base.Data.Value.ToString() : string.Empty;

		}

		/// <summary>
		/// Datas the editor control_ on save.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void DataEditorControl_OnSave(EventArgs e)
		{
			base.Data.Value = this.m_Control.SelectedValues;
		}
	}
}
