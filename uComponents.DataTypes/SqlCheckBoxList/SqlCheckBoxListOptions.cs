using System.ComponentModel;
using System.Configuration;
using umbraco;
using umbraco.editorControls;

namespace uComponents.DataTypes.SqlCheckBoxList
{
	internal class SqlCheckBoxListOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes an instance of SqlCheckBoxListOptions
		/// </summary>
		public SqlCheckBoxListOptions()
		{
		}

		public SqlCheckBoxListOptions(bool loadDefaults)
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
		/// Defaults to true, where the property value will be stored as an Xml Fragment, else if false, a Csv will be stored
		/// </summary>
		[DefaultValue(true)]
		public bool UseXml { get; set; }

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
