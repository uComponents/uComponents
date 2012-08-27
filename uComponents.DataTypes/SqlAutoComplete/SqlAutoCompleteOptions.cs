using System.ComponentModel;
using umbraco.editorControls;
using umbraco;

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
        public string ConnectionString { get; set; }


        [DefaultValue(0)]
        public int LetterCount { get; set; }

        /// <summary>
        /// Defaults to true, where the property value will be stored as an Xml Fragment, else if false, a Csv will be stored
        /// </summary>
        [DefaultValue(true)]
        public bool UseXml { get; set; }

        public string GetConnectionString()
        {
            string connectionString;

            if (!string.IsNullOrEmpty(this.ConnectionString))
            {
                connectionString = this.ConnectionString;
            }
            else
            {
                connectionString = uQuery.SqlHelper.ConnectionString;
            }

            return connectionString;
        }
    }
}
