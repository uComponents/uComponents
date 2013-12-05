using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.Xml.XPath;
using uComponents.Core;
using uComponents.Core.Abstractions;
using uComponents.Core.Interfaces;
using umbraco;
using umbraco.cms.helpers;
using umbraco.NodeFactory;

namespace uComponents.XsltExtensions
{
	/// <summary>
	/// The Urls class exposes XSLT extensions to create and modify Urls.
	/// </summary>
	[XsltExtension("ucomponents.urls")]
	public class Urls
	{
		/// <summary>
		/// Instance of the wrapper for <c>umbraco.library</c>, so we can 'mock' this for unit-testing.
		/// </summary>
		private static IUmbracoLibrary Library = new UmbracoLibraryWrapper();

		/// <summary>
		/// Before NiceUrl generated event handler delegate
		/// </summary>
		/// <param name="nodeId">The node id</param>
		/// <param name="e">The event arguments</param>
		public delegate void BeforeNiceUrlGeneratedEventHandler(ref int nodeId, EventArgs e);

		/// <summary>
		/// After NiceUrl generated event handler delegate
		/// </summary>
		/// <param name="nodeId">The node id</param>
		/// <param name="url">The generated url</param>
		/// <param name="e">The event arguments</param>
		public delegate void AfterNiceUrlGeneratedEventHandler(ref int nodeId, ref string url, EventArgs e);

		/// <summary>
		/// Before NiceUrl generated event
		/// </summary>
		public static event BeforeNiceUrlGeneratedEventHandler BeforeNiceUrlGenerated;

		/// <summary>
		/// After NiceUrl generated event
		/// </summary>
		public static event AfterNiceUrlGeneratedEventHandler AfterNiceUrlGenerated;

		/// <summary>
		/// Appends the alternative template to the url as if it was a folder instead of a querystring. Is "use directory urls" aware if set.
		/// </summary>
		/// <param name="url">The url to the content</param>
		/// <param name="altTemplateAlias">The name of the alternative template to use</param>
		/// <returns>Complete url with alt template merged in.</returns>
		public static string AddAltTemplate(string url, string altTemplateAlias)
		{
			return AddAltTemplate(url, altTemplateAlias, false);
		}

		/// <summary>
		/// Appends the alternative template to the url as a querystring or folder instead of a querystring. Is "use directory urls" aware if set.
		/// </summary>
		/// <param name="url">The url to the content</param>
		/// <param name="altTemplateAlias">The name of the alternative template to use</param>
		/// <param name="useQueryString">Should the alttemplate be added as a folder or querystring?</param>
		/// <returns>Complete url with alt template merged in.</returns>
		/// <remarks>Intended for use in our unit tests but here if you need it</remarks>
		public static string AddAltTemplate(string url, string altTemplateAlias, bool useQueryString)
		{
			return AddAltTemplate(url, altTemplateAlias, useQueryString, GlobalSettings.UseDirectoryUrls);
		}

		/// <summary>
		/// Appends the alternative template to the url as a querystring or folder instead of a querystring. Is "use directory urls" aware if set.
		/// </summary>
		/// <param name="url">The url to the content</param>
		/// <param name="altTemplateAlias">The name of the alternative template to use</param>
		/// <param name="useQueryString">Should the alttemplate be added as a folder or querystring?</param>
		/// <param name="useDirectoryUrls">Should we render out with a .aspx extension or just a trailing slash?</param>
		/// <returns>Complete url with alt template merged in.</returns>
		/// <remarks>Intended for use in our unit tests but here if you need it</remarks>
		public static string AddAltTemplate(string url, string altTemplateAlias, bool useQueryString, bool useDirectoryUrls)
		{
			string niceUrl = url;

			if (!string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(altTemplateAlias))
			{
				altTemplateAlias = altTemplateAlias.ToLower();

				if (useQueryString)
				{
					niceUrl = AppendOrUpdateQueryString(niceUrl, Constants.Umbraco.Url.AltTemplate, altTemplateAlias);
				}
				else
				{
					url = url.Replace(Constants.Common.DOTASPX, string.Empty);
					niceUrl = ConcatAsUrl(url, altTemplateAlias);
					if (useDirectoryUrls)
					{
						niceUrl += Constants.Common.SLASH;
					}
					else
					{
						niceUrl += Constants.Common.DOTASPX;
					}
				}
			}

			return niceUrl;
		}

		/// <summary>
		/// Appends or updates a query string value to the current Url
		/// </summary>
		/// <param name="key">The query string key</param>
		/// <param name="value">The query string value</param>
		/// <returns>The updated Url</returns>
		public static string AppendOrUpdateQueryString(string key, string value)
		{
			return AppendOrUpdateQueryString(HttpContext.Current.Request.RawUrl, key, value);
		}

		/// <summary>
		/// Appends or updates a query string value to supplied Url
		/// </summary>
		/// <param name="url">The Url to update</param>
		/// <param name="key">The query string key</param>
		/// <param name="value">The query string value</param>
		/// <returns>The updated Url</returns>
		public static string AppendOrUpdateQueryString(string url, string key, string value)
		{
			var q = '?';

			if (url.IndexOf(q) == -1)
			{
				return string.Concat(url, q, key, '=', HttpUtility.UrlEncode(value));
			}

			var baseUrl = url.Substring(0, url.IndexOf(q));
			var queryString = url.Substring(url.IndexOf(q) + 1);
			var match = false;
			var kvps = HttpUtility.ParseQueryString(queryString);

			foreach (var queryStringKey in kvps.AllKeys)
			{
				if (queryStringKey == key)
				{
					kvps[queryStringKey] = value;
					match = true;
					break;
				}
			}

			if (!match)
			{
				kvps.Add(key, value);
			}

			return string.Concat(baseUrl, q, ConstructQueryString(kvps, null, false));
		}

		/// <summary>
		/// Used to stitch different segments (folders) of a url together. Handles if a slash is needed between the two strings even if one might exist on one of the strings already. Greatly simplified your XSLT logic.
		/// </summary>
		/// <param name="part1">The left part of the url to concat to</param>
		/// <param name="part2">The right part of the url to concat on</param>
		/// <returns>String of the full url represented by concating the two strings together using the default slash divider</returns>
		public static string ConcatAsUrl(string part1, string part2)
		{
			return ConcatAsUrl(part1, part2, Constants.Common.SLASH);
		}

		/// <summary>
		/// Used to stitch different segments (folders) of a url together. Handles if a slash is needed between the two strings even if one might exist on one of the strings already. Greatly simplified your XSLT logic.
		/// </summary>
		/// <param name="part1">The left part of the url to concat to</param>
		/// <param name="part2">The right part of the url to concat on</param>
		/// <param name="divider">A char character to use as the divider</param>
		/// <returns>String of the full url represented by concating the two strings together</returns>
		public static string ConcatAsUrl(string part1, string part2, char divider)
		{
			part1 = part1.Trim().TrimEnd(divider);
			part2 = part2.Trim().TrimStart(divider);
			return string.Concat(part1, divider, part2);
		}

		/// <summary>
		/// Formats the URL - replacing characters in the string to make a 'safe' URL.
		/// </summary>
		/// <param name="input">The input URL.</param>
		/// <returns>Returns a 'safe' URL, removing illegal characters.</returns>
		/// <remarks>This method is a wrapper for <c>umbraco.cms.helpers.url.FormatUrl</c>.</remarks>
		public static string FormatUrl(string input)
		{
			return url.FormatUrl(input);
		}

		/// <summary>
		/// Returns a nicely formated Url for a given node.
		/// </summary>
		/// <param name="nodeId">The node id.</param>
		/// <returns>The NiceUrl for the node id.</returns>
		public static string NiceUrl(int nodeId)
		{
			return NiceUrl(nodeId, null);
		}

		/// <summary>
		/// Returns a nicely formated Url for a given node and alternative template setting appended as a querystring.
		/// </summary>
		/// <param name="nodeId">The node id.</param>
		/// <param name="altTemplateAlias">The alias of the alternative template.</param>
		/// <returns>The NiceUrl for the node id.</returns>
		public static string NiceUrl(int nodeId, string altTemplateAlias)
		{
			return NiceUrl(nodeId, altTemplateAlias, false);
		}

		/// <summary>
		/// Returns a nicely formated Url for a given node and alternative template.
		/// </summary>
		/// <param name="nodeId">The node id.</param>
		/// <param name="altTemplateAlias">The alias of the alternative template.</param>
		/// <param name="useQueryString">Boolean to choose whether the append alternative template as a querystring parameter.</param>
		/// <returns>The NiceUrl for the node id.</returns>
		public static string NiceUrl(int nodeId, string altTemplateAlias, bool useQueryString)
		{
			OnBeforeNiceUrlGenerated(ref nodeId);

			string niceUrl = AddAltTemplate(GetNiceUrl(nodeId), altTemplateAlias, useQueryString);

			OnAfterNiceUrlGenerated(ref nodeId, ref niceUrl);

			return niceUrl;
		}

		/// <summary>
		/// Gets the hostname of the node Id.
		/// </summary>
		/// <param name="nodeId">The node Id.</param>
		/// <returns>Returns the hostname for the node Id.</returns>
		public static string GetHostName(int nodeId)
		{
			// get the domains for the node Id
			var domains = library.GetCurrentDomains(nodeId);

			// check that a domain exists
			if (domains != null && domains.Length > 0)
			{
				// return the first domain
				return domains[0].Name;
			}

			// otherwise return an empty string
			return string.Empty;
		}

		/// <summary>
		/// Gets the node Id by URL.
		/// </summary>
		/// <param name="url">The URL to get the XML node from.</param>
		/// <returns>Returns the node Id.</returns>
		[Obsolete("Please use uComponents.XsltExtensions.Nodes.GetNodeIdByUrl")]
		public static int GetNodeIdByUrl(string url)
		{
			return Nodes.GetNodeIdByUrl(url);
		}

		/// <summary>
		/// Gets the text by URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>Returns the text (<c>System.String</c>) from a given URL.</returns>
		public static string GetTextByUrl(string url)
		{
			try
			{
				using (var client = new WebClient())
				{
					return client.DownloadString(url);
				}
			}
			catch
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the XML node by URL.
		/// </summary>
		/// <param name="url">The URL to get the XML node from.</param>
		/// <returns>Returns the XML for the node.</returns>
		[Obsolete("Please use uComponents.XsltExtensions.Nodes.GetXmlNodeByUrl")]
		public static XPathNodeIterator GetXmlNodeByUrl(string url)
		{
			return Nodes.GetXmlNodeByUrl(url);
		}

		/// <summary>
		/// Guesses the NiceUrl based on the node id.
		/// </summary>
		/// <param name="nodeId">The node id.</param>
		/// <returns>Returns a guestimate of the NiceUrl for a node id.</returns>
		/// <remarks>
		/// GuessNiceUrl is not very performant when attempting to guess the URL for unpublished nodes.
		/// Do not over-use this method. It makes many database calls and will be slow!
		/// </remarks>
		public static string GuessNiceUrl(int nodeId)
		{
			var node = new Node(nodeId);
			var niceUrl = node.NiceUrl;
			var published = node.Path != null;

			if (!niceUrl.StartsWith(Constants.Common.HTTP) || !published)
			{
				const string URLNAME = Constants.Umbraco.Content.UrlName; // "umbracoUrlName";
				var hasDomain = false;
				var domain = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToLower();
				string nodeName;
				string nodePath;
				int nodeParentId;

				// if the node is published, get from the nodeFactory
				if (published)
				{
					nodeName = node.GetProperty(URLNAME) != null && !string.IsNullOrEmpty(node.GetProperty(URLNAME).Value) ? node.GetProperty(URLNAME).Value : node.Name;
					nodePath = node.Path;
					nodeParentId = node.Parent != null ? node.Parent.Id : uQuery.RootNodeId;
				}
				else
				{
					// otherwise, get from the Document object.
					var doc = new umbraco.cms.businesslogic.web.Document(nodeId);
					nodeName = doc.getProperty(URLNAME) != null && doc.getProperty(URLNAME).Value != null && !string.IsNullOrEmpty(doc.getProperty(URLNAME).Value.ToString()) ? doc.getProperty(URLNAME).Value.ToString() : doc.Text;
					nodePath = doc.Path;
					nodeParentId = doc.ParentId;
				}

				// check if the node has a domain associated.
				if (UmbracoSettings.UseDomainPrefixes)
				{
					// get the path
					var path = nodePath.Split(Constants.Common.SLASH);

					// loop through each part of the path in reverse order
					for (int i = path.Length - 1; i >= 0; i--)
					{
						int partId;
						if (int.TryParse(path[i], out partId))
						{
							var domains = umbraco.cms.businesslogic.web.Domain.GetDomainsById(partId);
							if (domains != null && domains.Length > 0)
							{
								hasDomain = true;
								domain = domains[0].Name;
								break;
							}
						}
					}
				}

				// if the node is unpublished...
				if (!published)
				{
					// get the published node for the parent node.
					var parentNode = new Node(nodeParentId);
					if (parentNode.Path != null || (nodeParentId == uQuery.RootNodeId && !hasDomain))
					{
						int level = parentNode.Path.Split(Constants.Common.COMMA).Length;
						string parentUrl = nodeParentId > 0 && !(level == 2 && GlobalSettings.HideTopLevelNodeFromPath) ? parentNode.NiceUrl : string.Empty;
						string urlName = string.Concat(Constants.Common.SLASH, url.FormatUrl(nodeName.ToLower()));
						string fileExtension = GlobalSettings.UseDirectoryUrls ? string.Empty : Constants.Common.DOTASPX;

						// construct the NiceUrl for the unpublished node.
						niceUrl = string.Concat(parentUrl.Replace(Constants.Common.DOTASPX, string.Empty), urlName, fileExtension);
					}
				}

				// if the node has a domain, and is unpublished, use the domain.
				if (niceUrl == Constants.Common.HASH && hasDomain)
				{
					niceUrl = string.Concat(Constants.Common.HTTP, domain);
				}

				// if the NiceUrl doesn't start with 'http://' (and isn't '#') then prepend it.
				if (!niceUrl.StartsWith(Constants.Common.HTTP) && niceUrl != Constants.Common.HASH)
				{
					niceUrl = string.Concat(Constants.Common.HTTP, domain, niceUrl);
				}
			}

			return niceUrl;
		}

		/// <summary>
		/// Constructs a NameValueCollection into a query string. Assumes delimiter is "&amp;" and will ommit keys with empty/null values
		/// </summary>
		/// <remarks>Consider this method to be the opposite of "System.Web.HttpUtility.ParseQueryString"</remarks>
		/// <param name="parameters">The NameValueCollection</param>
		/// <returns>A key/value structured query string, delimited by the specified String</returns>
		private static string ConstructQueryString(NameValueCollection parameters)
		{
			return ConstructQueryString(parameters, "&", true);
		}

		/// <summary>
		/// Constructs a NameValueCollection into a query string. Assumes delimiter is "&amp;".
		/// </summary>
		/// <remarks>Consider this method to be the opposite of "System.Web.HttpUtility.ParseQueryString"</remarks>
		/// <param name="parameters">The NameValueCollection</param>
		/// <param name="omitEmpty">Boolean to chose whether to omit empty values</param>
		/// <returns>A key/value structured query string, delimited by the specified String</returns>
		private static string ConstructQueryString(NameValueCollection parameters, bool omitEmpty)
		{
			return ConstructQueryString(parameters, "&", omitEmpty);
		}

		/// <summary>
		/// Constructs a NameValueCollection into a query string.
		/// </summary>
		/// <remarks>Consider this method to be the opposite of "System.Web.HttpUtility.ParseQueryString"</remarks>
		/// <param name="parameters">The NameValueCollection</param>
		/// <param name="delimiter">The String to delimit the key/value pairs</param>
		/// <param name="omitEmpty">Boolean to chose whether to omit empty values</param>
		/// <returns>A key/value structured query string, delimited by the specified String</returns>
		/// <example>
		/// http://blog.leekelleher.com/2009/09/19/how-to-convert-namevaluecollection-to-a-query-string-revised/
		/// </example>
		private static string ConstructQueryString(NameValueCollection parameters, string delimiter, bool omitEmpty)
		{
			if (string.IsNullOrEmpty(delimiter))
				delimiter = "&";

			var equals = '=';
			var items = new List<string>();

			for (var i = 0; i < parameters.Count; i++)
			{
				foreach (var value in parameters.GetValues(i))
				{
					var addValue = omitEmpty ? !string.IsNullOrEmpty(value) : true;
					if (addValue)
					{
						items.Add(string.Concat(parameters.GetKey(i), equals, HttpUtility.UrlEncode(value)));
					}
				}
			}

			return string.Join(delimiter, items.ToArray());
		}

		/// <summary>
		/// Gets the NiceUrl.
		/// </summary>
		/// <param name="nodeId">The node id.</param>
		/// <returns>Returns the NiceUrl for the node id.</returns>
		private static string GetNiceUrl(int nodeId)
		{
			return Library.NiceUrl(nodeId);
		}

		/// <summary>
		/// Dispatches a BeforeNiceUrlGenerated event.
		/// </summary>
		/// <param name="nodeId">The node id</param>
		private static void OnBeforeNiceUrlGenerated(ref int nodeId)
		{
			if (BeforeNiceUrlGenerated != null)
			{
				BeforeNiceUrlGenerated(ref nodeId, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Dispatches an AfterNiceUrlGenerated event.
		/// </summary>
		/// <param name="nodeId">The node id</param>
		/// <param name="url">The generated url</param>
		private static void OnAfterNiceUrlGenerated(ref int nodeId, ref string url)
		{
			if (AfterNiceUrlGenerated != null)
			{
				AfterNiceUrlGenerated(ref nodeId, ref url, EventArgs.Empty);
			}
		}
	}
}