using System.ComponentModel;
using umbraco.editorControls;
using umbraco;
using System.Configuration;

namespace uComponents.DataTypes.SqlAutoComplete
{
    internal class SqlAutoCompleteOptions : AbstractOptions
    {
        /// <summary>
        /// Initializes an instance of SqlAutoCompleteOptions
        /// </summary>
        public SqlAutoCompleteOptions()
        {
        }

        public SqlAutoCompleteOptions(bool loadDefaults)
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


        [DefaultValue(3)]
        public int MinLength { get; set; }

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
