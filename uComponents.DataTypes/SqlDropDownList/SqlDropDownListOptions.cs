
namespace uComponents.DataTypes.SqlDropDownList
{
	/// <summary>
	/// 
	/// </summary>
	internal class SqlDropDownListOptions
	{
		/// <summary>
		/// Sql expression used to get drop down list values
		/// this expression must return both Text and Value fields
		/// </summary>
		public string Sql { get; set; }

		/// <summary>
		/// Gets or sets an optional connection string (if null then umbraco connection string is used)
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Initializes an instance of SqlDropDownListOptions
		/// </summary>
		public SqlDropDownListOptions()
		{
			this.Sql = string.Empty;
			this.ConnectionString = null;
		}
	}
}
