using umbraco.presentation.umbracobase;
using System.Web;
using umbraco;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Web.Script.Serialization;
namespace uComponents.DataTypes.XPathAutoComplete
{
    /// <summary>
    /// Use the data-type guid as the start of the /base request
    /// </summary>
    [RestExtension(DataTypeConstants.XPathAutoCompleteId)]
    public class XPathAutoCompleteBase
    {
        private static XPathAutoCompleteOptions GetOptions(int datatypeDefinitionId)
        {
            return (XPathAutoCompleteOptions)HttpContext.Current.Cache[DataTypeConstants.XPathAutoCompleteId + "_" + datatypeDefinitionId.ToString()];
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
            string autoCompleteText = HttpContext.Current.Request.Form["autoCompleteText"];

            // default json returned if it wasn't able to get any data
            string json = @"[
                                { 
                                    'label' : '_Error',
                                    'value' : ''
                                }
                            ]";

            // get the options data for the current datatype instance
            XPathAutoCompleteOptions options = GetOptions(datatypeDefinitionId);

            // double check, as client shouldn't call this method if invalid
            if (autoCompleteText.Length >= options.MinLength)
            {

                // cause XPath to execute ? or search a cache ? (would be per datatypeDefinitionId)


            }

            return json;
        }
    }
}
