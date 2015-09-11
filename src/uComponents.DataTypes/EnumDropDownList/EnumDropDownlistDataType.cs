using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.EnumDropDownList
{
	/// <summary>
	/// The Enum DropDownList data-type.
	/// </summary>
	public class EnumDropDownListDataType : BaseDataType, IDataType
	{
		/// <summary>
		/// Private field for the Prevalue Editor.
		/// </summary>
		private EnumDropDownListPreValueEditor preValueEditor;

		/// <summary>
		/// Private field for the Editor.
		/// </summary>
		private IDataEditor dataEditor;

		/// <summary>
		/// Private field for the Data.
		/// </summary>
		private IData data;

		/// <summary>
		/// Gets the name of the data type.
		/// </summary>
		/// <value>The name of the data type.</value>
		public override string DataTypeName
		{
			get
			{
				return "uComponents: Enum DropDownList";
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
				return new Guid(DataTypeConstants.EnumDropDownListId);
			}
		}

		/// <summary>
		/// Lazy load the associated PreValueEditor instance,
		/// this is constructed supplying 'this'
		/// </summary>
		public override IDataPrevalue PrevalueEditor
		{
			get
			{
				return this.preValueEditor ?? (this.preValueEditor = new EnumDropDownListPreValueEditor(this));
			}
		}

		/// <summary>
		/// Lazy load the associated DataEditor, 
		/// this is constructed supplying the data value stored by the PreValueEditor, and also the configuration settings of the PreValueEditor 
		/// </summary>
		public override IDataEditor DataEditor
		{
			get
			{
				return this.dataEditor
				       ?? (this.dataEditor =
				           new EnumDropDownListDataEditor(this.Data, ((EnumDropDownListPreValueEditor)this.PrevalueEditor).Options));
			}
		}

		/// <summary>
		/// Lazy load an empty DefaultData object, this is used to pass data between the PreValueEditor and the DataEditor
		/// </summary>
		public override IData Data
		{
			get
			{
				return this.data ?? (this.data = new DefaultData(this));
			}
		}
	}
}