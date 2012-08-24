using System.ComponentModel;
using uComponents.Core.Shared.PrevalueEditors;

namespace uComponents.Core.DataTypes.SqlDropDownList
{
	/// <summary>
	/// The options for the SqlDropDownList data-type.
	/// </summary>
    internal class SqlDropDownListOptions : AbstractOptions
	{
		/// <summary>
		/// Sql expression used to get drop down list values
		/// this expression must return both Text and Value fields
		/// </summary>
        [DefaultValue("")]
		public string Sql { get; set; }

		/// <summary>
		/// Gets or sets an optional connection string (if null then umbraco connection string is used)
		/// </summary>
        [DefaultValue("")]
		public string ConnectionString { get; set; }

		/// <summary>
		/// Initializes an instance of SqlDropDownListOptions
		/// </summary>
		public SqlDropDownListOptions()
		{
		}

        public SqlDropDownListOptions(bool loadDefaults) : base(loadDefaults)
        {
        }
	}
}
