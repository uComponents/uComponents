using System.ComponentModel;
using System.Configuration;
using umbraco;
using umbraco.editorControls;

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

        [DefaultValue(1)]
        public int MinLength { get; set; }

        [DefaultValue(0)]
        public int MaxSuggestions { get; set; }

        [DefaultValue(0)]
        public int MinItems { get; set; }

        [DefaultValue(0)]
        public int MaxItems { get; set; }

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
