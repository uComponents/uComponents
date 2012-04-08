using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.RenderMacro
{
	/// <summary>
	/// A RenderMacro data-type for Umbraco
	/// </summary>
	public class RenderMacroDataType : AbstractDataEditor
	{
		/// <summary>
		/// The PreValue Editor for the data-type.
		/// </summary>
		private RenderMacroPrevalueEditor m_PreValueEditor;

		/// <summary>
		/// The Data Editor for the data-type.
		/// </summary>
		private RenderMacroDataEditor m_DataEditor;

		/// <summary>
		/// Field for the options.
		/// </summary>
		private RenderMacroOptions options;

		/// <summary>
		/// Gets the options.
		/// </summary>
		/// <value>The options.</value>
		public RenderMacroOptions Options
		{
			get
			{
				if (this.options == null)
				{
					this.options = ((RenderMacroPrevalueEditor)this.PrevalueEditor).GetPreValueOptions<RenderMacroOptions>();
				}

				return this.options;
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
				return new Guid(DataTypeConstants.RenderMacroId);
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
				return "uComponents: Render Macro";
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
					this.m_PreValueEditor = new RenderMacroPrevalueEditor(this);
				}

				return this.m_PreValueEditor;
			}
		}

		/// <summary>
		/// Gets the prevalue editor.
		/// </summary>
		/// <value>The prevalue editor.</value>
		public override IDataEditor DataEditor
		{
			get
			{
				if (this.m_DataEditor == null)
				{
					this.m_DataEditor = new RenderMacroDataEditor(this.Data, this.Options);
				}

				return this.m_DataEditor;
			}
		}
	}
}
