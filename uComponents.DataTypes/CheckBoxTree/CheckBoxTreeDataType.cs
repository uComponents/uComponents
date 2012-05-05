using System;
using uComponents.Core;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.CheckBoxTree
{
	/// <summary>
	/// The CheckBoxTree data-type.
	/// </summary>
	public class CheckBoxTreeDataType : BaseDataType, IDataType
	{
		/// <summary>
		/// Field for the preValueEditor.
		/// </summary>
		private CheckBoxTreePreValueEditor preValueEditor;

		/// <summary>
		/// Field for the dataEditor.
		/// </summary>
		private IDataEditor dataEditor;

		/// <summary>
		/// Field for the data.
		/// </summary>
		private IData data;

		/// <summary>
		/// Field for the options.
		/// </summary>
		private CheckBoxTreeOptions options;

		/// <summary>
		/// Gets the options.
		/// </summary>
		/// <value>The options.</value>
		public CheckBoxTreeOptions Options
		{
			get
			{
				if (this.options == null)
				{
					this.options = ((CheckBoxTreePreValueEditor)this.PrevalueEditor).Options;
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
				return "uComponents: CheckBoxTree";
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
				return new Guid(DataTypeConstants.CheckBoxTreeId);
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
				if (this.preValueEditor == null)
				{
					this.preValueEditor = new CheckBoxTreePreValueEditor(this);
				}

				return this.preValueEditor;
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
					this.dataEditor = new CheckBoxTreeDataEditor(this.Data, this.Options);
				}

				return this.dataEditor;
			}
		}

		/// <summary>
		/// Lazy load an empty DefaultData object, this is used to pass data between the PreValueEditor and the DataEditor
		/// </summary>
		/// <value>The data.</value>
		public override IData Data
		{
			get
			{
				if (this.data == null)
				{
					switch (this.Options.OutputFormat)
					{
						case Settings.OutputFormat.XML:
							this.data = new XmlData(this);
							break;

						case Settings.OutputFormat.CSV:
						default:
							this.data = new DefaultData(this);
							break;
					}
				}

				return this.data;
			}
		}
	}
}