using System.ComponentModel;
using System.Configuration;
using umbraco;
using umbraco.editorControls;

namespace uComponents.DataTypes.SqlDropDownList
{
    /// <summary>
    /// 
    /// </summary>
	internal class SqlDropDownListOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes an instance of SqlDropDownListOptions
		/// </summary>
		public SqlDropDownListOptions()
		{
		}

		public SqlDropDownListOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

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
		public string ConnectionStringName { get; set; }

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
