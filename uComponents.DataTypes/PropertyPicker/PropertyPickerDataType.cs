using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.PropertyPicker
{
	/// <summary>
	/// The Property Picker data-type.
	/// </summary>
	public class PropertyPickerDataType : BaseDataType, IDataType
	{
		/// <summary>
		/// Private field for the Data.
		/// </summary>
		private IData m_Data;

		/// <summary>
		/// Private field for the Editor.
		/// </summary>
		private IDataEditor m_DataEditor;

		/// <summary>
		/// Private field for the Prevalue Editor.
		/// </summary>
		private PropertyPickerPrevalueEditor m_PreValueEditor;

		/// <summary>
		/// Gets the name of the data type.
		/// </summary>
		/// <value>The name of the data type.</value>
		public override string DataTypeName
		{
			get
			{
				return "uComponents: Property Picker";
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
				return new Guid(DataTypeConstants.PropertyPickerId);
			}
		}

		/// <summary>
		/// Gets the data.
		/// </summary>
		/// <value>The data.</value>
		public override IData Data
		{
			get
			{
				if (this.m_Data == null)
				{
					this.m_Data = new DefaultData(this);
				}

				return this.m_Data;
			}
		}

		/// <summary>
		/// Gets the data editor.
		/// </summary>
		/// <value>The data editor.</value>
		public override IDataEditor DataEditor
		{
			get
			{
				if (this.m_DataEditor == null)
				{
					this.m_DataEditor = new PropertyPickerDataEditor(this.Data, ((PropertyPickerPrevalueEditor)this.PrevalueEditor).Options);
				}

				return this.m_DataEditor;
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
					this.m_PreValueEditor = new PropertyPickerPrevalueEditor(this);
				}

				return this.m_PreValueEditor;
			}
		}
	}
}