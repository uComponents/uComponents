using System;
using System.Collections.Specialized;
using System.Web;
using System.Xml;
using System.Xml.XPath;

using uComponents.Core.Shared;
using umbraco;

namespace uComponents.Core.XsltExtensions
{
	/// <summary>
	/// The Request class exposes XSLT extensions to access data from the HttpRequest object.
	/// </summary>
	public class Request
	{
		/// <summary>
		/// Converts the Request.ServerVariables object into a XPathNodeIterator object.
		/// </summary>
		/// <returns>Returns a XPathNodeIterator object that represents the Request.ServerVariables object.</returns>
		public static XPathNodeIterator ServerVariables()
		{
			return ConvertNameValueCollectionToXPathNodeIterator("Request.ServerVariables", HttpContext.Current.Request.ServerVariables);
		}

		/// <summary>
		/// Converts the Request.QueryString object into a XPathNodeIterator object.
		/// </summary>
		/// <returns>Returns a XPathNodeIterator object that represents the Request.QueryString object.</returns>
		public static XPathNodeIterator QueryString()
		{
			return ConvertNameValueCollectionToXPathNodeIterator("Request.QueryString", HttpContext.Current.Request.QueryString);
		}

		/// <summary>
		/// Converts the Request.Form object into a XPathNodeIterator object.
		/// </summary>
		/// <returns>Returns a XPathNodeIterator object that represents the Request.Form object.</returns>
		public static XPathNodeIterator Form()
		{
			return ConvertNameValueCollectionToXPathNodeIterator("Request.Form", HttpContext.Current.Request.Form);
		}

		/// <summary>
		/// Converts the Request.Cookies object into a XPathNodeIterator object.
		/// </summary>
		/// <returns>Returns a XPathNodeIterator object that represents the Request.Cookies object.</returns>
		public static XPathNodeIterator Cookies()
		{
			var xd = new XmlDocument();
			xd.LoadXml("<Request.Cookies/>");

			if (HttpContext.Current.Request.Cookies != null)
			{
				try
				{
					HttpCookieCollection cookies = HttpContext.Current.Request.Cookies;
					for (int i = 0; i < cookies.Count; i++)
					{
						var cookie = cookies.Get(i);
						var node = xmlHelper.addTextNode(xd, "cookie", string.Empty);

						node.Attributes.Append(xmlHelper.addAttribute(xd, "name", cookie.Name));
						node.Attributes.Append(xmlHelper.addAttribute(xd, "domain", cookie.Domain));
						node.Attributes.Append(xmlHelper.addAttribute(xd, "expires", cookie.Expires.ToString()));
						node.Attributes.Append(xmlHelper.addAttribute(xd, "hasKeys", cookie.HasKeys.ToString()));
						node.Attributes.Append(xmlHelper.addAttribute(xd, "httpOnly", cookie.HttpOnly.ToString()));
						node.Attributes.Append(xmlHelper.addAttribute(xd, "path", cookie.Path));
						node.Attributes.Append(xmlHelper.addAttribute(xd, "secure", cookie.Secure.ToString()));

						for (int j = 0; j < cookie.Values.Count; j++)
						{
							var value = xmlHelper.addTextNode(xd, "value", cookie.Values.Get(j));
							
							if (cookie.HasKeys)
							{
								value.Attributes.Append(xmlHelper.addAttribute(xd, "name", cookie.Values.GetKey(j)));
							}

							node.AppendChild(value);
						}

						xd.DocumentElement.AppendChild(node);
					}
				}
				catch (Exception ex)
				{
					xd.DocumentElement.AppendChild(xmlHelper.addTextNode(xd, "error", ex.Message));
				}
			}
			else
			{
				xd.DocumentElement.AppendChild(xmlHelper.addTextNode(xd, "error", string.Concat("The Request.Cookies object is empty.")));
			}

			return xd.CreateNavigator().Select("/");
		}

		/// <summary>
		/// Iterates over the NameValueCollection, appending each entry to an XmlTextNode.
		/// </summary>
		/// <param name="rootName">The name of the root node of the XmlDocument.</param>
		/// <param name="nvc">The NameValueCollection to be converted.</param>
		/// <returns>Returns a XPathNodeIterator object that represents the NameValueCollection.</returns>
		private static XPathNodeIterator ConvertNameValueCollectionToXPathNodeIterator(string rootName, NameValueCollection nvc)
		{
			var xd = new XmlDocument();
			xd.LoadXml(string.Concat("<", rootName, "/>"));

			if (nvc != null)
			{
				try
				{
					for (int i = 0; i < nvc.Count; i++)
					{
						var node = xmlHelper.addCDataNode(xd, nvc.GetKey(i), nvc.Get(i));
						xd.DocumentElement.AppendChild(node);
					}
				}
				catch (Exception ex)
				{
					xd.DocumentElement.AppendChild(xmlHelper.addTextNode(xd, "error", ex.Message));
				}
			}
			else
			{
				xd.DocumentElement.AppendChild(xmlHelper.addTextNode(xd, "error", string.Concat("The ", rootName, " object is empty.")));
			}

			return xd.CreateNavigator().Select("/");
		}
	}
}
