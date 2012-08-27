using umbraco.presentation.umbracobase;
using System.Web;
using umbraco;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Web.Script.Serialization;


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
            return (SqlAutoCompleteOptions)HttpContext.Current.Cache[DataTypeConstants.SqlAutoCompleteId + "_" + datatypeDefinitionId.ToString()];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autoCompleteText"></param>
        /// <returns>a JSON string collection of Key, Value pairs</returns>
        [RestExtensionMethod(returnXml = false)]
        public static string GetData(int datatypeDefinitionId, int currentId, string autoCompleteText)
        {
            // default json returned if it wasn't able to get any data
            string json = @"[
                                { 
                                    'label' : '_Error',
                                    'value' : ''
                                }
                            ]";

            // get the options data for the current datatype instance
            SqlAutoCompleteOptions options = GetOptions(datatypeDefinitionId);

            // double check, as client shouldn't call this method if invalid
            if (autoCompleteText.Length >= options.LetterCount)
            {
                string sql = options.Sql;

                sql = sql.Replace("@currentId", currentId.ToString());
                sql = sql.Replace("@autoComplete", autoCompleteText);

                using (SqlConnection sqlConnection = new SqlConnection(options.GetConnectionString()))
                {
                    SqlCommand sqlCommand = new SqlCommand()
                    {
                        Connection = sqlConnection,
                        CommandType = CommandType.Text,
                        CommandText = sql
                    };

                    sqlConnection.Open();

                    json = new JavaScriptSerializer().Serialize(
                                from dbDataRecord in sqlCommand.ExecuteReader().Cast<DbDataRecord>()
                                select new
                                {
                                    label = dbDataRecord["Text"].ToString(),
                                    value = dbDataRecord["Value"].ToString()
                                }
                            );

                    sqlConnection.Close();
                }
            }

            return json;
        }
    }
}
