using System.Net;
using System.Web;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using umbraco.NodeFactory;

namespace uComponents.Core.NotFoundHandlers
{
	/// <summary>
	/// A NotFoundHandler for multiple web-site set-up.
	/// </summary>
	public class MultiSitePageNotFoundHandler : INotFoundHandler
	{
		/// <summary>
		/// Field to store the redirect node Id.
		/// </summary>
		private int _redirectId = uQuery.RootNodeId;

		/// <summary>
		/// Gets a value indicating whether [cache URL].
		/// </summary>
		/// <value><c>true</c> if [cache URL]; otherwise, <c>false</c>.</value>
		public bool CacheUrl
		{
			get
			{
				return false;
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
				return this._redirectId;
			}
		}

		/// <summary>
		/// Executes the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>Returns whether the NotFoundHandler has a match.</returns>
		public bool Execute(string url)
		{
			HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;

			var success = false;

			// get the current domain name
			var domain = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

			// get the root node id of the domain
			var rootNodeId = Domain.GetRootFromDomain(domain);

			try
			{
				if (rootNodeId > 0)
				{
					// get the node
					var node = new Node(rootNodeId);

					// get the property that holds the node id for the 404 page
					var property = node.GetProperty("umbracoPageNotFound");
					if (property != null)
					{
						var errorId = property.Value;
						if (!string.IsNullOrEmpty(errorId))
						{
							// if the node id is numeric, then set the redirectId
							success = int.TryParse(errorId, out this._redirectId);
						}
					}
				}
			}
			catch
			{
				success = false;
			}

			return success;
		}
	}
}