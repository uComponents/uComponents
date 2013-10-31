using System;
using System.ComponentModel;
using System.Configuration;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco;

namespace uComponents.DataTypes.SqlCheckBoxList
{
	/// <summary>
	/// The options for the SqlCheckBoxListOptions data-type.
	/// </summary>
	public class SqlCheckBoxListOptions : SqlOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCheckBoxListOptions"/> class.
		/// </summary>
		public SqlCheckBoxListOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCheckBoxListOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public SqlCheckBoxListOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Defaults to true, where the property value will be stored as an Xml Fragment, else if false, a Csv will be stored
		/// </summary>
		[DefaultValue(true)]
		public bool UseXml { get; set; }
	}
}
