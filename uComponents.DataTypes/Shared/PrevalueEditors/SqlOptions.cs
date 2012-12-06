using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using umbraco.editorControls;
using System.ComponentModel;
using umbraco.DataLayer;
using umbraco;
using System.Configuration;

namespace uComponents.DataTypes.Shared.PrevalueEditors
{
	/// <summary>
	/// The abstracted options for the Sql-powered data-types.
	/// </summary>
	public abstract class SqlOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlOptions"/> class.
		/// </summary>
		public SqlOptions()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public SqlOptions(bool loadDefaults)
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
		[Obsolete("Please use SqlDropDownListOptions.GetSqlHelper() instead.", false)]
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

		/// <summary>
		/// Gets the SQL helper.
		/// </summary>
		/// <returns></returns>
		public ISqlHelper GetSqlHelper()
		{
			if (!string.IsNullOrWhiteSpace(this.ConnectionStringName))
			{
				var connectionStringSettings = ConfigurationManager.ConnectionStrings[this.ConnectionStringName];
				if (connectionStringSettings != null)
				{
					return DataLayerHelper.CreateSqlHelper(connectionStringSettings.ConnectionString);
				}
			}

			return uQuery.SqlHelper;
		}
	}
}
