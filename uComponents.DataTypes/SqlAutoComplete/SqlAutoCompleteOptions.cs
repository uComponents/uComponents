using System.ComponentModel;
using System.Configuration;
using umbraco;
using umbraco.editorControls;

namespace uComponents.DataTypes.SqlAutoComplete
{
	/// <summary>
	/// The options for the SqlAutoCompleteOptions data-type.
	/// </summary>
	public class SqlAutoCompleteOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlAutoCompleteOptions"/> class.
		/// </summary>
		public SqlAutoCompleteOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlAutoCompleteOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public SqlAutoCompleteOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Sql expression used to get drop down list values
		/// this expression must return both Text and Value fields
		/// </summary>
		/// <value>The SQL.</value>
		[DefaultValue("")]
		public string Sql { get; set; }

		/// <summary>
		/// Gets or sets an optional connection string (if null then umbraco connection string is used)
		/// </summary>
		/// <value>The name of the connection string.</value>
		[DefaultValue("")]
		public string ConnectionStringName { get; set; }

		/// <summary>
		/// Gets or sets the length of the min.
		/// </summary>
		/// <value>The length of the min.</value>
		[DefaultValue(1)]
		public int MinLength { get; set; }

		/// <summary>
		/// Gets or sets the max suggestions.
		/// </summary>
		/// <value>The max suggestions.</value>
		[DefaultValue(0)]
		public int MaxSuggestions { get; set; }

		/// <summary>
		/// Gets or sets the min items.
		/// </summary>
		/// <value>The min items.</value>
		[DefaultValue(0)]
		public int MinItems { get; set; }

		/// <summary>
		/// Gets or sets the max items.
		/// </summary>
		/// <value>The max items.</value>
		[DefaultValue(0)]
		public int MaxItems { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [allow duplicates].
		/// </summary>
		/// <value><c>true</c> if [allow duplicates]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public bool AllowDuplicates { get; set; }

		/// <summary>
		/// Checks web.config for a matching named connection string, else returns the current Umbraco database connection
		/// </summary>
		/// <returns>a connection string</returns>
		public string GetConnectionString()
		{
			if (!string.IsNullOrWhiteSpace(this.ConnectionStringName))
			{
				// attempt to get connection string from the web.config
				ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[this.ConnectionStringName];
				if (connectionStringSettings != null)
				{
					return connectionStringSettings.ConnectionString;
				}
			}

			return uQuery.SqlHelper.ConnectionString; // default if unknown;
		}
	}
}
