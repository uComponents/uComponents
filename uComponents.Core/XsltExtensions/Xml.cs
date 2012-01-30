using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.XPath;
using uComponents.Core.Shared;
using umbraco;

namespace uComponents.Core.XsltExtensions
{
	/// <summary>
	/// The Xml class exposes XSLT extensions to offer extended XML/XSLT functionality.
	/// </summary>
	public class Xml
	{
		/// <summary>
		/// Filters the node-set with the specified XPath.
		/// </summary>
		/// <param name="nodeset">The nodeset.</param>
		/// <param name="xpath">The XPath expression.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the filtered node-set.
		/// </returns>
		public static XPathNodeIterator FilterNodes(XPathNodeIterator nodeset, string xpath)
		{
			try
			{
				// iterate over the node-set.
				while (nodeset.MoveNext())
				{
					// evaluate the current node with the xpath expression.
					var nav = nodeset.Current;
					return (XPathNodeIterator)nav.Evaluate(xpath);
				}
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}

			// fall-back returning the original node-set.
			return nodeset;
		}

		/// <summary>
		/// Gets the XML document by URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="cacheInSeconds">The cache in seconds.</param>
		/// <param name="isGzipped">if set to <c>true</c> [is gzipped].</param>
		/// <returns></returns>
		public static XPathNodeIterator GetXmlDocumentByUrl(string url, int cacheInSeconds, bool isGzipped)
		{
			if (!isGzipped)
			{
				// if not gzip, then use Umbraco library
				return library.GetXmlDocumentByUrl(url, cacheInSeconds);
			}

			var cacheKey = string.Concat("GetXmlDoc_", url);

			if (cacheInSeconds > 0)
			{
				// check the cache
				var cached = HttpContext.Current.Cache.Get(cacheKey);
				if (cached != null)
				{
					return (XPathNodeIterator)cached;
				}
			}

			// get the gzipped XML
			var document = new XmlDocument();

			try
			{
				using (var client = new WebClient())
				{
					var buffer = client.DownloadData(url);
					using (var stream = new MemoryStream(buffer))
					using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
					using (var reader = new XmlTextReader(gzip))
					{
						document.Load(reader);
					}
				}
			}
			catch (Exception exception)
			{
				document.LoadXml(string.Format("<error url=\"{0}\">{1}</error>", HttpContext.Current.Server.HtmlEncode(url), exception));
			}

			var xmlDocumentByUrl = document.CreateNavigator().Select("/");

			if (cacheInSeconds > 0)
			{
				HttpContext.Current.Cache.Insert(cacheKey, xmlDocumentByUrl, null, DateTime.Now.Add(new TimeSpan(0, 0, cacheInSeconds)), TimeSpan.Zero, CacheItemPriority.Low, null);
			}

			return xmlDocumentByUrl;
		}

		/// <summary>
		/// Parses the specified XML string.
		/// </summary>
		/// <param name="xml">The XML string.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the XML string.
		/// </returns>
		public static XPathNodeIterator Parse(string xml)
		{
			return ParseXml(xml, "/");
		}

		/// <summary>
		/// Parses the specified XML string.
		/// </summary>
		/// <param name="xml">The XML string.</param>
		/// <param name="xpath">The XPath expression for the navigator selection.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the XML string.
		/// </returns>
		public static XPathNodeIterator ParseXml(string xml, string xpath)
		{
			try
			{
				// check if the string could be xml
				if (!CouldItBeXml(xml))
				{
					xml = "<root/>";
				}

				// load the xml string into a reader
				using (var reader = new StringReader(xml))
				{
					// load the reader into an xpath document and select the root node.
					return new XPathDocument(reader).CreateNavigator().Select(xpath);
				}
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Parses the specified XHTML string.
		/// </summary>
		/// <param name="xhtml">The XHTML string.</param>
		/// <returns>Returns an XPathNodeIterator of the XHTML string.</returns>
		public static XPathNodeIterator ParseXhtml(string xhtml)
		{
			var builder = new StringBuilder();

			if (!xhtml.StartsWith("<?xml") && !xhtml.StartsWith("<!DOCTYPE"))
			{
				builder
					.AppendLine("<!DOCTYPE WYSIWYG [ <!ENTITY nbsp \"&#160;\"> ]>")
					.Append("<xhtml>")
					.Append(xhtml)
					.Append("</xhtml>");
			}
			else
			{
				builder.Append(xhtml);
			}

			return ParseXml(builder.ToString(), "/xhtml");
		}

		/// <summary>
		/// Returns a random child node from the parent node.
		/// </summary>
		/// <param name="node">The (parent) node.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of a random child node from the parent node.
		/// </returns>
		public static XPathNodeIterator RandomChildNode(XPathNavigator node)
		{
			// gets the child nodes of the navigator
			var children = node.SelectChildren(XPathNodeType.Element);

			// gets a random number from the node count.
			var random = (children != null && children.Count > 0) ? library.GetRandom().Next(1, children.Count) : 1;

			// return the node at that position()
			return node.Select(string.Concat("*[", random, "]"));
		}

		/// <summary>
		/// Returns a random node from the node-set.
		/// </summary>
		/// <param name="nodeset">The node-set.</param>
		/// <returns>
		/// Returns an <c>System.Xml.XPath.XPathNodeIterator</c> of a random node from the node-set.
		/// </returns>
		public static XPathNodeIterator RandomNode(XPathNodeIterator nodeset)
		{
			try
			{
				// gets a random number from the node count.
				var random = library.GetRandom().Next(1, nodeset.Count + 1);

				// iterate over the node-set.
				while (nodeset.MoveNext())
				{
					// if the position == random number...
					if (nodeset.CurrentPosition == random)
					{
						// ... then return the current node.
						return nodeset.Current.Select(".");
					}
				}
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}

			// fall-back returning the original node-set.
			return nodeset;
		}

		/// <summary>
		/// Randomizes the sort order of the specified nodeset.
		/// </summary>
		/// <param name="node">The (parent) node.</param>
		/// <returns>
		/// Returns the nodeset with a random sort order.
		/// </returns>
		public static XPathNodeIterator RandomSort(XPathNavigator node)
		{
			return RandomSort(node, -1);
		}

		/// <summary>
		/// Randomizes the sort order of the specified nodeset, limited to a specified number.
		/// </summary>
		/// <param name="node">The (parent) node.</param>
		/// <param name="count">The count.</param>
		/// <returns>
		/// Returns the nodeset with a random sort order.
		/// </returns>
		public static XPathNodeIterator RandomSort(XPathNavigator node, int count)
		{
			var children = node.SelectChildren(XPathNodeType.Element);
			var list = new SortedDictionary<Guid, XPathNavigator>();

			while (children.MoveNext())
			{
				list.Add(Guid.NewGuid(), children.Current.Clone());
			}

			var xml = new XmlDocument();
			xml.AppendChild(xml.CreateElement(node.Name));
			var nav = xml.DocumentElement.CreateNavigator();
			var i = 0;

			foreach (var item in list)
			{
				if (count != -1 && i >= count)
				{
					break;
				}

				nav.AppendChild(item.Value);
				i++;
			}

			return nav.Select(".");
		}

		/// <summary>
		/// Splits the specified delimited string into an XPathNodeIterator.
		/// </summary>
		/// <param name="data">The delimited string data.</param>
		/// <returns>Returns an <c>System.Xml.XPath.XPathNodeIterator</c> representation of the delimited string data.</returns>
		public static XPathNodeIterator Split(string data)
		{
			return Split(data, new string(Settings.COMMA, 1));
		}

		/// <summary>
		/// Splits the specified delimited string into an XPathNodeIterator.
		/// </summary>
		/// <param name="data">The delimited string data.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>Returns an <c>System.Xml.XPath.XPathNodeIterator</c> representation of the delimited string data.</returns>
		public static XPathNodeIterator Split(string data, string separator)
		{
			return Split(data, separator, "values", "value");
		}

		/// <summary>
		/// Splits the specified delimited string into an XPathNodeIterator.
		/// </summary>
		/// <param name="data">The delimited string data.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="rootName">Name of the root node.</param>
		/// <param name="elementName">Name of the element node.</param>
		/// <returns>Returns an <c>System.Xml.XPath.XPathNodeIterator</c> representation of the delimited string data.</returns>
		public static XPathNodeIterator Split(string data, string separator, string rootName, string elementName)
		{
			var xd = Split(data, new[] { separator }, rootName, elementName);
			return xd.CreateNavigator().Select(string.Concat(Settings.SLASH, rootName));
		}

		/// <summary>
		/// Determines whether the specified string appears to be XML.
		/// </summary>
		/// <param name="xml">The XML string.</param>
		/// <returns>
		/// 	<c>true</c> if the specified string appears to be XML; otherwise, <c>false</c>.
		/// </returns>
		internal static bool CouldItBeXml(string xml)
		{
			if (!string.IsNullOrEmpty(xml))
			{
				xml = xml.Trim();

				if (xml.StartsWith("<") && xml.EndsWith(">"))
				{
					return true;
				}
			}

			return false;
		}

		internal static XmlDocument Split(string data, string[] separator, string rootName, string elementName)
		{
			return Split(new XmlDocument(), data, separator, rootName, elementName);
		}

		/// <summary>
		/// Splits the specified delimited string into an XML document.
		/// </summary>
		/// <param name="xml">The XML document.</param>
		/// <param name="data">The delimited string data.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="rootName">Name of the root node.</param>
		/// <param name="elementName">Name of the element node.</param>
		/// <returns>Returns an <c>System.Xml.XmlDocument</c> representation of the delimited string data.</returns>
		internal static XmlDocument Split(XmlDocument xml, string data, string[] separator, string rootName, string elementName)
		{
			// load new XML document.
			xml.LoadXml(string.Concat("<", rootName, "/>"));

			// get the data-value, check it isn't empty.
			if (!string.IsNullOrEmpty(data))
			{
				// explode the values into an array
				var values = data.Split(separator, StringSplitOptions.None);

				// loop through the array items.
				foreach (string value in values)
				{
					// add each value to the XML document.
					var xn = xmlHelper.addTextNode(xml, elementName, value);
					xml.DocumentElement.AppendChild(xn);
				}
			}

			// return the XML node.
			return xml;
		}
	}
}