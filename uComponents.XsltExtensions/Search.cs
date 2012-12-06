using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using Examine;
using uComponents.Core;
using umbraco;

namespace uComponents.XsltExtensions
{
	/// <summary>
	/// The Search class exposes XSLT extensions to offer extended Examine/Lucene functionality.
	/// </summary>
	[XsltExtension("ucomponents.search")]
	public class Search
	{
		/// <summary>
		/// Performs an advanced search against an Examine/Lucene index.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <returns>
		/// Returns an XML structure of the advanced search results.
		/// </returns>
		public static XPathNodeIterator AdvancedSearch(string searchText)
		{
			return AdvancedSearch(searchText, true);
		}

		/// <summary>
		/// Performs an advanced search against an Examine/Lucene index.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
		/// <returns>
		/// Returns an XML structure of the advanced search results.
		/// </returns>
		public static XPathNodeIterator AdvancedSearch(string searchText, bool useWildcards)
		{
			return AdvancedSearch(searchText, useWildcards, ExamineManager.Instance.DefaultSearchProvider.Name);
		}

		/// <summary>
		/// Advanceds the search.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
		/// <param name="providerName">Name of the provider.</param>
		/// <returns>
		/// Returns an XML structure of the advanced search results.
		/// </returns>
		public static XPathNodeIterator AdvancedSearch(string searchText, bool useWildcards, string providerName)
		{
			return AdvancedSearch(searchText, useWildcards, providerName, "content");
		}

		/// <summary>
		/// Performs an advanced search against an Examine/Lucene index.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
		/// <param name="providerName">Name of the provider.</param>
		/// <param name="indexType">Type of the index.</param>
		/// <returns>
		/// Returns an XML structure of the advanced search results.
		/// </returns>
		public static XPathNodeIterator AdvancedSearch(string searchText, bool useWildcards, string providerName, string indexType)
		{
			const string COMMA = ",";

			var provider = ExamineManager.Instance.SearchProviderCollection[providerName];

			// create the search criteria
			var criteria = provider.CreateSearchCriteria(indexType);

			// set the default query for the search
			string queryPath = string.Concat("+path:\\", uQuery.RootNodeId, ",");

			// parse the querystring-styled query into a name-value collection.
			NameValueCollection searchParams = HttpUtility.ParseQueryString(searchText);

			// loop through each of the search parameters
			foreach (string key in searchParams)
			{
				string value = searchParams[key];

				switch (key.ToUpper())
				{
					case "ID":
						int nodeId;
						if (int.TryParse(value, out nodeId))
						{
							criteria = criteria.Id(nodeId).Compile();
						}
						break;

					case "NODENAME":
						criteria = criteria.NodeName(value).Compile();
						break;

					case "NODETYPEALIAS":
						criteria = criteria.NodeTypeAlias(value).Compile();
						break;

					//case "ORDERBY":
					//    string[] values = value.Split(new[] { COMMA }, StringSplitOptions.RemoveEmptyEntries);
					//    criteria = criteria.OrderBy(values).Compile();
					//    break;

					//case "ORDERBYDESCENDING":
					//    string[] valuesDesc = value.Split(new[] { COMMA }, StringSplitOptions.RemoveEmptyEntries);
					//    criteria = criteria.OrderByDescending(valuesDesc).Compile();
					//    break;

					//case "PARENTID":
					//    int parentId;
					//    if (int.TryParse(value, out parentId))
					//    {
					//        criteria = criteria.ParentId(parentId).Compile(); // 'parentID' isn't currently indexed by Examine/Lucene (only stored).
					//    }
					//    break;

					case "PATH":
						// explode/implode the path (removes empty entries)
						string path = string.Join(COMMA, value.Split(new[] { COMMA }, StringSplitOptions.RemoveEmptyEntries));

						if (!string.IsNullOrEmpty(path))
						{
							// add the selected path to the 'queryPath'
							queryPath = string.Concat(queryPath, path);
						}

						break;

					//case "RANGE":
					//    int start, end;
					//    string[] parts = value.Split('~');
					//    if (parts.Length > 2 && int.TryParse(parts[1], out start) && int.TryParse(parts[2], out end))
					//    {
					//        criteria = criteria.Range(parts[0], start, end).Compile();
					//    }
					//    break;

					//case "DATERANGE":
					//    DateTime startDate, endDate;
					//    string[] partsDate = value.Split('~');
					//    if (partsDate.Length > 2 && DateTime.TryParse(partsDate[1], out startDate) && DateTime.TryParse(partsDate[2], out endDate))
					//    {
					//        criteria = criteria.Range(partsDate[0], startDate, endDate).Compile();
					//    }
					//    break;

					default:
						criteria = criteria.Field(key, value).Compile();
						break;
				}
			}

			// perform a raw query search.
			return RawQuery(string.Concat(queryPath, "*"), useWildcards, providerName, indexType);
		}

		/// <summary>
		/// Gets the node ids.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <returns>
		/// Returns a CSV of node Ids from a basic/quick search
		/// </returns>
		public static string GetNodeIds(string searchText)
		{
			return GetNodeIds(searchText, true);
		}

		/// <summary>
		/// Gets the node ids.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
		/// <returns>
		/// Returns a CSV of node Ids from a basic/quick search
		/// </returns>
		public static string GetNodeIds(string searchText, bool useWildcards)
		{
			return GetNodeIds(searchText, useWildcards, ExamineManager.Instance.DefaultSearchProvider.Name);
		}

		/// <summary>
		/// Gets the node ids.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
		/// <param name="providerName">Name of the provider.</param>
		/// <returns>
		/// Returns a CSV of node Ids from a basic/quick search
		/// </returns>
		public static string GetNodeIds(string searchText, bool useWildcards, string providerName)
		{
			var results = InternalSearch(searchText, useWildcards, providerName);
			var list = results.Select<SearchResult, string>(r => r.Id.ToString());

			return string.Join(new string(Constants.Common.COMMA, 1), list.ToArray());
		}

		/// <summary>
		/// Performs a basic/quick search against an Examine/Lucene index.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <returns>
		/// Returns an XML structure of the basic/quick search results.
		/// </returns>
		public static XPathNodeIterator QuickSearch(string searchText)
		{
			return QuickSearch(searchText, true);
		}

		/// <summary>
		/// Performs a basic/quick search against an Examine/Lucene index.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
		/// <returns>
		/// Returns an XML structure of the basic/quick search results.
		/// </returns>
		public static XPathNodeIterator QuickSearch(string searchText, bool useWildcards)
		{
			return QuickSearch(searchText, useWildcards, ExamineManager.Instance.DefaultSearchProvider.Name);
		}

		/// <summary>
		/// Performs a basic/quick search against an Examine/Lucene index.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
		/// <param name="providerName">Name of the provider.</param>
		/// <returns>
		/// Returns an XML structure of the basic/quick search results.
		/// </returns>
		public static XPathNodeIterator QuickSearch(string searchText, bool useWildcards, string providerName)
		{
			var results = InternalSearch(searchText, useWildcards, providerName);
			return GetResultsAsXml(results);
		}

		/// <summary>
		/// Performs a raw query against the specified Examine/Lucene provider.
		/// </summary>
		/// <param name="rawQuery">The raw query.</param>
		/// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
		/// <param name="providerName">Name of the provider.</param>
		/// <param name="indexType">Type of the index.</param>
		/// <returns>
		/// Returns an XML structure of the raw search query results.
		/// </returns>
		public static XPathNodeIterator RawQuery(string rawQuery, bool useWildcards, string providerName, string indexType)
		{
			var provider = ExamineManager.Instance.SearchProviderCollection[providerName];

			// create the search criteria
			var criteria = provider.CreateSearchCriteria(indexType);

			// create a raw Lucene query, as Examine removes the '-' from start of string.
			criteria = criteria.RawQuery(rawQuery);

			// perform the search query
			var results = provider.Search(criteria);

			// convert the search results into an XPathNodeIterator - and return it
			return GetResultsAsXml(results);
		}

		/// <summary>
		/// Perform a search (interally).
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
		/// <param name="providerName">Name of the provider.</param>
		/// <returns></returns>
		internal static ISearchResults InternalSearch(string searchText, bool useWildcards, string providerName)
		{
			var provider = ExamineManager.Instance.SearchProviderCollection[providerName];
			return provider.Search(searchText, useWildcards);
		}

		/// <summary>
		/// Gets the results as XML.
		/// </summary>
		/// <param name="results">The results.</param>
		/// <returns>
		/// Returns an XML structure of the search results.
		/// </returns>
		internal static XPathNodeIterator GetResultsAsXml(ISearchResults results)
		{
			var legacy = uQuery.IsLegacyXmlSchema();
			var attributes = new List<string>() { "id", "nodeName", "updateDate", "writerName", "path", "nodeTypeAlias", "parentID", "loginName", "email" };
			var doc = new XDocument();

			if (results.TotalItemCount > 0)
			{
				var nodes = new XElement("results");
				foreach (SearchResult result in results)
				{
					var node = new XElement(legacy ? "node" : result.Fields["nodeTypeAlias"]);
					node.Add(new object[] { new XAttribute("score", result.Score) });

					foreach (KeyValuePair<string, string> item in result.Fields)
					{
						// if the field key starts with '__' (double-underscore) - then skip.
						if (item.Key.StartsWith("__"))
						{
							continue;
						}
						// if not legacy schema and 'nodeTypeAlias' - add the @isDoc attribute
						else if (!legacy && item.Key == "nodeTypeAlias")
						{
							node.Add(new object[] { new XAttribute("isDoc", string.Empty) });
						}
						// check if the field is an attribute or a data value
						else if (attributes.Contains(item.Key))
						{
							// attribute field
							node.Add(new object[] { new XAttribute(item.Key, item.Value) });
						}
						else
						{
							// data field
							var data = new XElement(legacy ? "data" : item.Key);

							// if legacy add the 'alias' attribute
							if (legacy)
							{
								data.Add(new object[] { new XAttribute("alias", item.Key) });
							}

							// CDATA the value - because we don't know what it is!
							data.Add(new object[] { new XCData(item.Value) });

							// add the data field to the node.
							node.Add(data);
						}
					}

					// add the node to the collection.
					nodes.Add(node);
				}

				doc.Add(nodes);
			}
			else
			{
				doc.Add(new XElement("error", "There were no search results."));
			}

			return doc.CreateNavigator().Select("/*");
		}
	}
}