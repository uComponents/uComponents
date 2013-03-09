using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.XPathChildNodePicker
{
	/// <summary>
	/// 
	/// </summary>
	public class XPathChildNodePickerDataType : BaseDataType, IDataType
	{
		/// <summary>
		/// Field for the Prevalue Editor.
		/// </summary>
		private XPathChildNodePickerPrevalueEditor prevalueEditor;

		/// <summary>
		/// Field for the Data Editor.
		/// </summary>
		private IDataEditor dataEditor;

		/// <summary>
		/// Field for the Data.
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
				return "uComponents: XPath Child Node Picker";
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
				return new Guid(DataTypeConstants.XPathChildNodePickerId);
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
				if (this.prevalueEditor == null)
				{
					this.prevalueEditor = new XPathChildNodePickerPrevalueEditor(this);
				}

				return this.prevalueEditor;
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
				if (this.dataEditor == null)
				{
					this.dataEditor = new XPathChildNodePickerDataEditor(this.Data, ((XPathChildNodePickerPrevalueEditor)this.PrevalueEditor).Options);
				}

				return this.dataEditor;
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
				if (this.data == null)
				{
					this.data = new DefaultData(this);
				}

				return this.data;
			}
		}
	}
}