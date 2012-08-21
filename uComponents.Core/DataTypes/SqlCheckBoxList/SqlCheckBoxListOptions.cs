using System;
using System.ComponentModel;
using uComponents.Core.Shared.PrevalueEditors;

namespace uComponents.Core.DataTypes.SqlCheckBoxList
{
    internal class SqlCheckBoxListOptions : AbstractOptions
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
        /// Defaults to true, where the property value will be stored as an Xml Fragment, else if false, a Csv will be stored
        /// </summary>
        [DefaultValue(true)]
        public bool UseXml { get; set; }

		/// <summary>
		/// Initializes an instance of SqlCheckBoxListOptions
		/// </summary>
        public SqlCheckBoxListOptions() 
        { 
        }

        public SqlCheckBoxListOptions(bool loadDefaults) : base(loadDefaults) 
        { 
        }
    }
}
