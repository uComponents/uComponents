using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using uComponents.DataTypes.Similarity.Net;

namespace uComponents.DataTypes.Similarity
{
	/// <summary>
	/// wrapper class around lucene contrib similarity to get document like given document
	/// using specified fields to test for similarity
	/// </summary>
	public class MoreUmbracoDocsLikeThis : IDisposable
	{
		private int docId;
		private string indexToSearch;
		private int maxNo;
		private IEnumerable<string> fieldsToSearch;
		private FSDirectory directory;
		private IndexReader reader;
		private IndexSearcher searcher;

		/// <summary>
		/// Initializes a new instance of the <see cref="MoreUmbracoDocsLikeThis"/> class.
		/// </summary>
		/// <param name="DocId">The doc id.</param>
		/// <param name="IndexToSearch">The index to search.</param>
		/// <param name="MaxNo">The max no.</param>
		/// <param name="FieldsToSearch">The fields to search.</param>
		public MoreUmbracoDocsLikeThis(int DocId, string IndexToSearch, int MaxNo, IEnumerable<string> FieldsToSearch)
		{
			docId = DocId;
			indexToSearch = IndexToSearch;
			maxNo = MaxNo;
			fieldsToSearch = FieldsToSearch;
			try
			{
				directory = FSDirectory.Open(new DirectoryInfo(GetIndexPath(indexToSearch)));
				reader = IndexReader.Open(directory, true);
				searcher = new IndexSearcher(reader);
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		/// <summary>
		/// for given document and fields in that doc get fixed no of docs that are similar
		/// assumes you have index that is up to date
		/// </summary>
		/// <returns>list of similar docs found</returns>
		public IEnumerable<SearchResultItem> FindMoreLikeThis()
		{
			var results = new List<SearchResultItem>();
			if (IsInit())
			{
				var moreLikeThis = new MoreLikeThis(reader);
				moreLikeThis.SetFieldNames(fieldsToSearch.ToArray());
				moreLikeThis.SetMinTermFreq(1);
				moreLikeThis.SetMinDocFreq(1);
				int currentLuceneDocId = GetLuceneDocNo(docId);
				if (currentLuceneDocId != 0)
				{
					var query = moreLikeThis.Like(currentLuceneDocId);
					var docs = searcher.Search(query, maxNo);
					int count = docs.scoreDocs.Length;
					//start at 1 as first item will be current document itself which we dont want
					for (int i = 1; i < count; i++)
					{

						var d = reader.Document(docs.scoreDocs[i].doc);
						var item = new SearchResultItem
										{
											PageName = d.GetField("nodeName").StringValue(),
											NodeId = int.Parse(d.GetField("__NodeId").StringValue())
										};
						results.Add(item);
					}
				}
			}
			return results;

		}

		private bool IsInit()
		{
			if (directory != null && reader != null && searcher != null)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// from the current umbraco node id we need actual lucene index unique id to query on
		/// </summary>
		/// <param name="docId"></param>
		/// <returns></returns>
		private int GetLuceneDocNo(int docId)
		{
			var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "__NodeId", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29));
			var query = parser.Parse(docId.ToString());
			var doc = searcher.Search(query, 1);
			if (doc.totalHits == 0)
			{
				return 0;
			}
			return doc.scoreDocs[0].doc;
		}

		/// <summary>
		/// get path to lucene index for lucene to query on, probably better way using examine api
		/// but couldnt figure it out
		/// </summary>
		/// <param name="indexToQuery"></param>
		/// <returns></returns>
		private string GetIndexPath(string indexToQuery)
		{
			string configFile = HttpContext.Current.Server.MapPath("/config/ExamineIndex.Config");
			// Create the query 
			var config =
				XElement.Load(configFile).Elements("IndexSet").Where(
					c => c.Attribute("SetName").Value == indexToQuery.Replace("Indexer", "IndexSet"));

			return HttpContext.Current.Server.MapPath(config.Attributes("IndexPath").FirstOrDefault().Value + "Index");
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (directory != null)
			{
				directory.Close();
			}

			if (reader != null)
			{
				reader.Close();
			}

			if (searcher != null)
			{
				searcher.Close();
			}
		}
	}
}