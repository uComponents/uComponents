using umbraco.presentation.umbracobase;
using System.Web;
using umbraco;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Web.Script.Serialization;
using System.Xml.Linq;


namespace uComponents.DataTypes.SqlAutoComplete
{
    /// <summary>
    /// Use the data-type guid as the start of the /base request
    /// </summary>
    [RestExtension(DataTypeConstants.SqlAutoCompleteId)]
    public class SqlAutoCompleteBase
    {
        private static SqlAutoCompleteOptions GetOptions(int datatypeDefinitionId)
        {
            return (SqlAutoCompleteOptions)HttpContext.Current.Cache[DataTypeConstants.SqlAutoCompleteId + "_options_" + datatypeDefinitionId.ToString()];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autoCompleteText"></param>
        /// <returns>a JSON string collection of Key, Value pairs</returns>
        [RestExtensionMethod(returnXml = false)]
        public static string GetData(int datatypeDefinitionId, int currentId)
        {
            string autoCompleteText = HttpContext.Current.Request.Form["autoCompleteText"];
            string selectedItemsXml = HttpContext.Current.Request.Form["selectedItems"];

            string[] selectedValues = null;
            if (!string.IsNullOrWhiteSpace(selectedItemsXml))
            {
                // parse selectedItemsXml to get unique collection of ids
                selectedValues = XDocument.Parse(selectedItemsXml).Descendants("Item").Select(x => x.Attribute("Value").Value).ToArray();
            }

            // default json returned if it wasn't able to get any data
            string json = @"[]";

            // get the options data for the current datatype instance
            SqlAutoCompleteOptions options = GetOptions(datatypeDefinitionId);

            // double check, as client shouldn't call this method if invalid
            if (options != null && autoCompleteText.Length >= options.MinLength)
            {
                string sql = options.Sql;

                sql = sql.Replace("@currentId", currentId.ToString());
                sql = sql.Replace("@autoCompleteText", autoCompleteText);

                using (SqlConnection sqlConnection = new SqlConnection(options.GetConnectionString()))
                {
                    SqlCommand sqlCommand = new SqlCommand()
                    {
                        Connection = sqlConnection,
                        CommandType = CommandType.Text,
                        CommandText = sql
                    };

                    sqlConnection.Open();

                    IEnumerable<KeyValuePair<string, string>> data;
                    
                    data = sqlCommand.ExecuteReader().Cast<DbDataRecord>()
                                                        .Where(x => (options.AllowDuplicates) || (selectedValues == null || !selectedValues.Contains(x["Value"].ToString())))
                                                        .Select(x => new KeyValuePair<string, string>(x["Text"].ToString(), x["Value"].ToString()));

                    if (options.MaxSuggestions > 0)
                    {
                        data = data.Take(options.MaxSuggestions);
                    }

                    json = new JavaScriptSerializer().Serialize(

                                from keyValuePair in data
                                select new
                                    {
                                        label = keyValuePair.Key,
                                        value = keyValuePair.Value
                                    }
                                );

                    sqlConnection.Close();
                }
            }

            return json;
        }
    }
}
