using System.Web;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;

namespace uComponents.NotFoundHandlers
{
	/// <summary>
	/// A NotFoundHandler for 301 Moved Permanently requests.
	/// </summary>
	public class SearchFor301MovedPermanently : INotFoundHandler
	{
		/// <summary>
		/// Field to store the flag to cache the URL.
		/// </summary>
		private bool cacheUrl = true;

		/// <summary>
		/// Field to store the redirect node Id.
		/// </summary>
		private int redirectId = uQuery.RootNodeId;

		/// <summary>
		/// Gets a value indicating whether [cache URL].
		/// </summary>
		/// <value><c>true</c> if [cache URL]; otherwise, <c>false</c>.</value>
		public bool CacheUrl
		{
			get
			{
				return this.cacheUrl;
			}
		}

		/// <summary>
		/// Gets the redirect ID.
		/// </summary>
		/// <value>The redirect ID.</value>
		public int redirectID
		{
			get
			{
				return this.redirectId;
			}
		}

		/// <summary>
		/// Executes the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>Returns whether the NotFoundHandler has a match.</returns>
		public bool Execute(string url)
		{
			var str = url;

			if (str.Length > 0)
			{
				if (str.Substring(0, 1) == "/")
				{
					str = str.Substring(1, str.Length - 1);
				}

				HttpContext.Current.Trace.Write("umbraco301MovedPermanently", string.Concat("'", str, "'"));

				var tmp = string.Empty;
				var xpath = string.Empty;
				var domainName = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
				var port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];

				// if there is a port number, append it
				if (!string.IsNullOrEmpty(port) && port != "80")
				{
					domainName = string.Concat(domainName, ":", port);
				}

				if (Domain.Exists(domainName))
				{
					if (uQuery.IsLegacyXmlSchema())
					{
						tmp = string.Concat("//node[@id=\"", Domain.GetRootFromDomain(domainName), "\"]");
					}
					else
					{
						tmp = string.Concat("//*[@isDoc and @id=\"", Domain.GetRootFromDomain(domainName), "\"]");
					}

					this.cacheUrl = false;
				}

				if (uQuery.IsLegacyXmlSchema())
				{
					xpath = string.Concat(tmp, "//node[contains(concat(\",\", translate(data[@alias=\"umbraco301MovedPermanently\"], \"ABCDEFGHIJKLMNOPQRSTUVWXYZ\", \"abcdefghijklmnopqrstuvwxyz\"), \",\"), \",", str.Replace(".aspx", string.Empty).ToLower(), ",\")]");
				}
				else
				{
					xpath = string.Concat(tmp, "//*[@isDoc and contains(concat(\",\", translate(umbraco301MovedPermanently, \"ABCDEFGHIJKLMNOPQRSTUVWXYZ\", \"abcdefghijklmnopqrstuvwxyz\"), \",\"), \",", str.Replace(".aspx", string.Empty).ToLower(), ",\")]");
				}

				var node = content.Instance.XmlContent.DocumentElement.SelectSingleNode(xpath);

				if (node != null)
				{
					if (int.TryParse(node.Attributes.GetNamedItem("id").Value, out this.redirectId))
					{
						var niceUrl = library.NiceUrl(this.redirectId);

						this.cacheUrl = true;

						HttpContext.Current.Response.Clear();
						HttpContext.Current.Response.StatusCode = 301;
						HttpContext.Current.Response.Status = "301 Moved Permanently";
						HttpContext.Current.Response.AddHeader("Location", niceUrl);
						HttpContext.Current.Response.End();

						return true;
					}
				}
			}

			return false;
		}
	}
}