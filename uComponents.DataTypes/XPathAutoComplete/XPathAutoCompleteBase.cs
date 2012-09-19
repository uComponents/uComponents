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
            return (XPathAutoCompleteOptions)HttpContext.Current.Cache[DataTypeConstants.XPathAutoCompleteId + "_options_" + datatypeDefinitionId.ToString()];
        }
        
        /// <summary>
        /// The index is a collection of the words used for the autocomplete text search, and their associated Ids
        /// </summary>
        /// <param name="datatypeDefinitionId"></param>
        /// <returns></returns>
        private static List<KeyValuePair<string, int>> GetIndex(int datatypeDefinitionId)
        {
            string indexCacheKey = DataTypeConstants.XPathAutoCompleteId + "_index_" + datatypeDefinitionId.ToString();

            List<KeyValuePair<string, int>> index = HttpContext.Current.Cache[indexCacheKey] as List<KeyValuePair<string, int>>;

            if (index == null)
            {
                // a sorted list is used, so that it's quick to find duplicates when adding items
                SortedList<string,int> sortedList = new SortedList<string, int>();

                // get the options so we know how to retrieve the data
                XPathAutoCompleteOptions options = XPathAutoCompleteBase.GetOptions(datatypeDefinitionId);

                switch (options.UmbracoObjectType)
                {
                    case uQuery.UmbracoObjectType.Document:
                        
                        foreach (KeyValuePair<string, int> keyValuePair in uQuery.GetNodesByXPath(options.XPath).Select(x => new KeyValuePair<string, int>(x.Name, x.Id)))
                        {
                            XPathAutoCompleteBase.AddToSortedList(ref sortedList, keyValuePair);
                        }

                        break;

                    case uQuery.UmbracoObjectType.Media:

                        foreach (KeyValuePair<string, int> keyValuePair in uQuery.GetMediaByXPath(options.XPath).Select(x => new KeyValuePair<string, int>(x.Text, x.Id)))
                        {
                            XPathAutoCompleteBase.AddToSortedList(ref sortedList, keyValuePair);
                        }

                        break;

                    case uQuery.UmbracoObjectType.Member:

                        foreach (KeyValuePair<string, int> keyValuePair in uQuery.GetMembersByXPath(options.XPath).Select(x => new KeyValuePair<string, int>(x.Text, x.Id)))
                        {
                            XPathAutoCompleteBase.AddToSortedList(ref sortedList, keyValuePair);
                        }

                        break;
                }

                // convert the SortedList into a regular list, and store that (regular list so that we can do a binary search on it)

                index = sortedList.ToList();

                // do we have to sort ?

                // put into cache 
                HttpContext.Current.Cache[indexCacheKey] = index;
            }

            return index;
        }

        private static void AddToSortedList(ref SortedList<string, int> index, KeyValuePair<string, int> keyValuePair)
        {
            if (keyValuePair.Key != null)
            {
                // does the index already contain the string as a key, if it does then append the id into the key
                if (!index.ContainsKey(keyValuePair.Key))
                {
                    // add to collection
                    index.Add(keyValuePair.Key, keyValuePair.Value);
                }
                else
                {
                    // key already exists, so double check shouldn't be required as ids should be unique, hence making a unique key
                    index.Add(keyValuePair.Key + "(" + keyValuePair.Value.ToString() + ")", keyValuePair.Value);
                }
            }
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
            string json = @"[]";

            // get the options data for the current datatype instance
            XPathAutoCompleteOptions options = GetOptions(datatypeDefinitionId);

            // double check, as client shouldn't call this method if invalid
            if (options != null && autoCompleteText.Length >= options.MinLength)
            {
                // get the index to search on from the cache (index contains all strings used for autocomplete comparrision + the Ids to which they relate)
                List<KeyValuePair<string, int>> index = XPathAutoCompleteBase.GetIndex(datatypeDefinitionId);

                // TODO: implement a BinarySearch on the index looking for the first match, and then another binary search looking for the first non match from that point
                // then serialize that subset (will be much quicker with large datasets as the following string parses every item)

                json = new JavaScriptSerializer().Serialize(

                        from keyValuePair in index
                        where keyValuePair.Key.ToUpper().StartsWith(autoCompleteText.ToUpper())
                        select new
                            {
                                label = keyValuePair.Key,
                                value = keyValuePair.Value
                            }
                        );
            }

            return json;
        }
    }
}
