using System.ComponentModel;
using System.Configuration;
using umbraco;
using umbraco.editorControls;
using umbraco.DataLayer;
using System;
using uComponents.DataTypes.Shared.PrevalueEditors;

namespace uComponents.DataTypes.SqlDropDownList
{
	/// <summary>
	/// The options for the SqlDropDownListOptions data-type.
	/// </summary>
	public class SqlDropDownListOptions : SqlOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlDropDownListOptions"/> class.
		/// </summary>
		public SqlDropDownListOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlDropDownListOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public SqlDropDownListOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}
	}
}