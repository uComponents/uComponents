using System;

using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.AutoComplete
{
	/// <summary>
	/// The AutoComplete data-type for Umbraco.
	/// </summary>
	public class AutoCompleteDataType : BaseDataType, IDataType
	{
		/// <summary>
		/// The Prevalue Editor for the data-type.
		/// </summary>
		private AutoCompletePreValueEditor preValueEditor;

		/// <summary>
		/// The Data Editor for the data-type.
		/// </summary>
		private IDataEditor dataEditor;

		/// <summary>
		/// The Data for the data-type.
		/// </summary>
		private IData data;

		/// <summary>
		/// Gets the name of the data-type.
		/// </summary>
		/// <value>The name of the data-type.</value>
		public override string DataTypeName
		{
			get
			{
				return "uComponents Legacy: Auto Complete";
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
				return new Guid(DataTypeConstants.AutoCompleteId);
			}
		}

		/// <summary>
		/// Lazy load the associated PreValueEditor instance, this is constructed supplying 'this'
		/// </summary>
		public override IDataPrevalue PrevalueEditor
		{
			get
			{
				if (this.preValueEditor == null)
				{
					this.preValueEditor = new AutoCompletePreValueEditor(this);
				}

				return this.preValueEditor;
			}
		}

		/// <summary>
		/// Lazy load the assocated DataEditor, 
		/// this is constructed supplying the data value stored by the PreValueEditor, and also the configuration settings of the PreValueEditor 
		/// </summary>
		public override IDataEditor DataEditor
		{
			get
			{
				if (this.dataEditor == null)
				{
					this.dataEditor = new AutoCompleteDataEditorUpdatePanel(this.Data, ((AutoCompletePreValueEditor)this.PrevalueEditor).Options);
				}

				return this.dataEditor;
			}
		}

		/// <summary>
		/// Lazy load an empty DefaultData object, this is used to pass data between the PreValueEditor and the DataEditor
		/// </summary>
		public override IData Data
		{
			get
			{
				if (this.data == null)
				{
					this.data = new DefaultData(this);
				}

				return this.data;
			}
		}
	}
}
