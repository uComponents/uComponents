using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.XPath;
using uComponents.Core.uQueryExtensions;
using umbraco.presentation.nodeFactory;

namespace uComponents.Core
{
	/// <summary>
	/// 
	/// </summary>
	public static partial class uQuery
	{
		/// <summary>
		/// Gets the Root Node Id (-1)
		/// </summary>
		public static readonly int RootNodeId = -1;

		/// <summary>
		/// Get a collection of Umbraco Nodes from an XPath expression
		/// </summary>
		/// <param name="xPath">XPath expression to get Nodes, can use $ancestorOrSelf which will use the current Node if published, else it'll use the nearest published parent
		/// $currentPage will be depreciated</param>
		/// <returns>an empty collection or a collection of nodes</returns>
		public static List<Node> GetNodesByXPath(string xPath)
		{
			var nodes = new List<Node>();

			if (xPath.Contains("$currentPage") || xPath.Contains("$ancestorOrSelf") || xPath.Contains("$parentPage"))
			{
				var ancestorOrSelfId = RootNodeId;
				var parentPageId = RootNodeId;

				var currentNode = uQuery.GetCurrentNode();
				if (currentNode != null)
				{
					ancestorOrSelfId = currentNode.Id;
					parentPageId = (currentNode.Parent != null) ? currentNode.Parent.Id : currentNode.Id;
				}
				else
				{
					// current node is unpublished or can't be found, so try via the Document API
					var currentDocument = uQuery.GetCurrentDocument();
					if (currentDocument != null)
					{
						// need to find first published parent						
						var publishedDocument = currentDocument.GetAncestorOrSelfDocuments().Where(document => document.Published == true).FirstOrDefault();
						if (publishedDocument != null)
						{
							// found the nearest published document
							ancestorOrSelfId = publishedDocument.Id;
							parentPageId = (publishedDocument.Id == currentDocument.Id) ? currentDocument.ParentId : publishedDocument.Id;
						}
					}
				}

				xPath = xPath.Replace("$parentPage", string.Concat("/descendant::*[@id='", parentPageId, "']"));
				xPath = xPath.Replace("$currentPage", "$ancestorOrSelf"); // $currentPage TO BE DECPRECIATED
				xPath = xPath.Replace("$ancestorOrSelf", string.Concat("/descendant::*[@id='", ancestorOrSelfId, "']"));
			}

			// Get Umbraco Xml
			XPathNavigator xPathNavigator = umbraco.content.Instance.XmlContent.CreateNavigator();
			XPathExpression xPathExpression;

			// Check to see if XPathExpression is in the cache
			if (HttpContext.Current.Cache[xPath] == null)
			{
				// Build Compiled XPath expression
				xPathExpression = xPathNavigator.Compile(xPath);

				// Store in Cache
				HttpContext.Current.Cache[xPath] = xPathExpression;
			}
			else // Get from Cache
			{
				xPathExpression = (XPathExpression)HttpContext.Current.Cache[xPath];
			}

			// [LK] Interested in exploring options to call custom extension methods in XPath expressions.
			// http://msdn.microsoft.com/en-us/library/ms950806.aspx
			// http://msdn.microsoft.com/en-us/library/dd567715.aspx

			XPathNodeIterator xPathNodeIterator = xPathNavigator.Select(xPathExpression);

			while (xPathNodeIterator.MoveNext())
			{
				var node = uQuery.GetNode(xPathNodeIterator.Current.GetAttribute("id", string.Empty));
				if (node != null)
				{
					nodes.Add(node);
				}
			}

			return nodes;
		}

		/// <summary>
		/// Returns a collection of Nodes, from a delimited list of Ids (as per the format used with UltimatePicker)
		/// </summary>
		/// <param name="csv">string csv of Ids</param>
		/// <returns>an empty collection or a collection or Nodes</returns>
		public static List<Node> GetNodesByCsv(string csv)
		{
			var nodes = new List<Node>();
			var ids = uQuery.GetCsvIds(csv);

			if (ids != null)
			{
				foreach (string id in ids)
				{
					var node = uQuery.GetNode(id);
					if (node != null)
					{
						nodes.Add(node);
					}
				}
			}

			return nodes;
		}

		/// <summary>
		/// Builds a node collection from an XML snippet
		/// </summary>
		/// <param name="xml">
		/// the expected Xml snippet is that stored by the Multi-Node Tree Picker (and XPathCheckBoxList when storing Ids)
		/// "<MultiNodePicker>
		///     <nodeId>1065</nodeId>
		///     <nodeId>1068</nodeId>
		///     <nodeId>1066</nodeId>
		///  </MultiNodePicker>"
		/// </param>
		/// <returns>an empty list or a list of nodes</returns>
		public static List<Node> GetNodesByXml(string xml)
		{
			var nodes = new List<Node>();
			var ids = uQuery.GetXmlIds(xml);

			if (ids != null)
			{
				foreach (int id in ids)
				{
					var node = uQuery.GetNode(id);
					if (node != null)
					{
						nodes.Add(node);
					}
				}
			}

			return nodes;
		}

		/// <summary>
		/// Get nodes by name
		/// </summary>
		/// <param name="name">name of node to look for</param>
		/// <returns>list of nodes, or empty list</returns>
		public static List<Node> GetNodesByName(string name)
		{
			return uQuery.GetNodesByXPath(string.Concat("descendant::*[@nodeName='", name, "']"));
		}

		/// <summary>
		/// Get nodes by document type alias
		/// </summary>
		/// <param name="documentTypeAlias">The document type alias</param>
		/// <returns>list of nodes, or empty list</returns>
		public static List<Node> GetNodesByType(string documentTypeAlias)
		{
			if (uQuery.IsLegacyXmlSchema())
			{
				return uQuery.GetNodesByXPath(string.Concat("descendant::*[@nodeTypeAlias='", documentTypeAlias, "']"));
			}
			else
			{
				return uQuery.GetNodesByXPath(string.Concat("descendant::", documentTypeAlias, "[@isDoc]"));
			}
		}

		/// <summary>
		/// Get nodes by document type id
		/// </summary>
		/// <param name="documentTypeId">The document type id.</param>
		/// <returns></returns>
		public static List<Node> GetNodesByType(int documentTypeId)
		{
			return uQuery.GetNodesByXPath(string.Concat("descendant::*[@nodeType='", documentTypeId, "']"));
		}

		/// <summary>
		/// Gets the node by URL.
		/// </summary>
		/// <param name="url">url to search for</param>
		/// <returns>null or node matching supplied url</returns>
		/// <remarks>Uses <c>uComponents.Core.XsltExtensions.Nodes.GetNodeIdByUrl</c></remarks>
		public static Node GetNodeByUrl(string url)
		{
			return uQuery.GetNode(XsltExtensions.Nodes.GetNodeIdByUrl(url));
		}

		/// <summary>
		/// Get top level content node
		/// </summary>
		/// <returns>the top level content node</returns>
		public static Node GetRootNode()
		{
			return new Node(RootNodeId);
		}

		/// <summary>
		/// checks to see if the current node can be got via the nodeFactory, if not then 
		/// checks to see if the current node can be got via an id on the QueryString
		/// </summary>
		/// <returns>the current node or null if not found</returns>
		public static Node GetCurrentNode()
		{
			Node currentNode = null;

			try
			{
				currentNode = Node.GetCurrent();
			}
			catch // if current node can't be found via the nodeFactory then Umbraco throws an exception
			{
				// look on QueryString for an id parameter (this is used in the backoffice)
				currentNode = uQuery.GetNode(uQuery.GetIdFromQueryString());
			}

			return currentNode;
		}

		/// <summary>
		/// Checks the supplied string can be cast to an integer, and returns the node with that Id
		/// </summary>
		/// <param name="nodeId">string representing the nodeId to return</param>
		/// <returns>Node or null</returns>
		public static Node GetNode(string nodeId)
		{
			int id;
			Node node = null;

			if (int.TryParse(nodeId, out id))
			{
				node = uQuery.GetNode(id);
			}

			return node;
		}

		/// <summary>
		/// Wrapper for Node constructor
		/// </summary>
		/// <param name="id">id of Node to get</param>
		/// <returns>Node or null</returns>
		public static Node GetNode(int id)
		{
			Node node;

			try
			{
				node = new Node(id);

				if (node.Id == 0)
				{
					node = null;
				}
			}
			catch
			{
				node = null;
			}

			return node;
		}

		/// <summary>
		/// Extension method on Node collection to return key value pairs of: node.Id / node.Name
		/// </summary>
		/// <param name="nodes">generic list of node objects</param>
		/// <returns>a collection of nodeIDs and their names</returns>
		public static Dictionary<int, string> ToNameIds(this List<Node> nodes)
		{
			var dictionary = new Dictionary<int, string>();

			foreach (var node in nodes)
			{
				dictionary.Add(node.Id, node.Name);
			}

			return dictionary;
		}
	}
}