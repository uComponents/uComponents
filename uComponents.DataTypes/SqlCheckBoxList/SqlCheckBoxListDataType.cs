using System;
using umbraco.editorControls;
using umbraco.interfaces;

namespace uComponents.DataTypes.SqlCheckBoxList
{
	/// <summary>
	/// SQL CheckBoxList data-type
	/// </summary>
	public class SqlCheckBoxListDataType : umbraco.cms.businesslogic.datatype.BaseDataType, IDataType
	{
		/// <summary>
		/// The PreValue Editor for the data-type.
		/// </summary>
		private SqlCheckBoxListPreValueEditor preValueEditor;

		/// <summary>
		/// The Data- Editor for the data-type.
		/// </summary>
		private IDataEditor dataEditor;

		/// <summary>
		/// The Data object for the data-type.
		/// </summary>
		private IData data;

		/// <summary>
		/// 
		/// </summary>
		private SqlCheckBoxListOptions options;

		/// <summary>
		/// Gets the options.
		/// </summary>
		/// <value>The options.</value>
		private SqlCheckBoxListOptions Options
		{
			get
			{
				if (this.options == null)
				{
					this.options = ((SqlCheckBoxListPreValueEditor)this.PrevalueEditor).Options;
				}

				return this.options;
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
				return "uComponents: SQL CheckBoxList";
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
				return new Guid(DataTypeConstants.SqlCheckBoxListId);
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
				if (this.preValueEditor == null)
				{
					this.preValueEditor = new SqlCheckBoxListPreValueEditor(this);
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
					this.dataEditor = new SqlCheckBoxListDataEditor(this.Data, this.Options);
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
					if (this.Options.UseXml)
					{
						// Storing an Xml fragment
						this.data = new XmlData(this);
					}
					else
					{
						// Storing a Csv
						this.data = new umbraco.cms.businesslogic.datatype.DefaultData(this);
					}
				}

				return this.data;
			}
		}
	}
}
