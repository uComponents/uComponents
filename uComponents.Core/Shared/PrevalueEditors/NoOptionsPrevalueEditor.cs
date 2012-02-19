using System;
using System.Web.UI;
using uComponents.Core.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.Core.Shared.PrevalueEditors
{
	/// <summary>
	/// The PreValue Editor with no options.
	/// </summary>
	public class NoOptionsPrevalueEditor : AbstractPrevalueEditor
	{
		/// <summary>
		/// The underlying base data-type.
		/// </summary>
		private readonly BaseDataType m_DataType;

		/// <summary>
		/// An object to temporarily lock writing to the database.
		/// </summary>
		private static readonly object m_Locker = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="NoOptionsPrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		/// <param name="dbType">Type of the db.</param>
		public NoOptionsPrevalueEditor(BaseDataType dataType, DBTypes dbType)
			: base()
		{
			this.m_DataType = dataType;
			this.m_DataType.DBType = dbType;
		}

		/// <summary>
		/// Gets the editor.
		/// </summary>
		/// <value>The editor.</value>
		public override Control Editor
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public override void Save()
		{
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow(string.Empty, "There are no options for this data-type.");
		}
	}
}
