using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using umbraco;
using umbraco.DataLayer;
using umbraco.presentation.umbracobase;

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
			return (SqlAutoCompleteOptions)HttpContext.Current.Cache[string.Concat(DataTypeConstants.SqlAutoCompleteId, "_options_", datatypeDefinitionId)];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="datatypeDefinitionId"></param>
		/// <param name="currentId"></param>
		/// <returns></returns>
		[RestExtensionMethod(returnXml = false)]
		public static string GetData(int datatypeDefinitionId, int currentId)
		{
			var autoCompleteText = HttpContext.Current.Request.Form["autoCompleteText"];
			var selectedItemsXml = HttpContext.Current.Request.Form["selectedItems"];

			string[] selectedValues = null;
			if (!string.IsNullOrWhiteSpace(selectedItemsXml))
			{
				// parse selectedItemsXml to get unique collection of ids
				selectedValues = XDocument.Parse(selectedItemsXml).Descendants("Item").Select(x => x.Attribute("Value").Value).ToArray();
			}

			// default json returned if it wasn't able to get any data
			var json = @"[]";

			// get the options data for the current datatype instance
			var options = GetOptions(datatypeDefinitionId);

			// double check, as client shouldn't call this method if invalid
			if (options != null && autoCompleteText.Length >= options.MinLength)
			{
				using (var sqlHelper = options.GetSqlHelper())
				{
					var sql = options.Sql;
					var parameters = new List<IParameter>();

					if (sql.Contains("@currentId"))
						parameters.Add(sqlHelper.CreateParameter("@currentId", currentId.ToString()));

					if (sql.Contains("@autoCompleteText"))
					{
						// HACK: For backwards-compatibility, remove any wildcards from the original query [LK]
						if (sql.Contains("'%@autoCompleteText%'"))
							sql = sql.Replace("'%@autoCompleteText%'", "@autoCompleteText");

						parameters.Add(sqlHelper.CreateParameter("@autoCompleteText", string.Concat("%", autoCompleteText, "%")));
					}

					using (var dataReader = sqlHelper.ExecuteReader(sql, parameters.ToArray()))
					{
						if (dataReader != null)
						{
							IEnumerable<KeyValuePair<string, string>> data = dataReader.Cast<DbDataRecord>()
								.Where(x => (options.AllowDuplicates) || (selectedValues == null || !selectedValues.Contains(x["Value"].ToString())))
								.Select(x => new KeyValuePair<string, string>(x["Text"].ToString(), x["Value"].ToString()))
								.OrderBy(x => x.Key);

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
								});
						}
					}
				}
			}

			return json;
		}
	}
}