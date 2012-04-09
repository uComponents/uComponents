using System.Net;
using System.Web;
using umbraco.interfaces;

namespace uComponents.NotFoundHandlers
{
	/// <summary>
	/// NotFoundHandler that checks a 'umbracoPageNotFound' property has been set.
	/// </summary>
	public class SearchForPageNotFound : INotFoundHandler
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

			try
			{
				// set the XPath query (based on new or legacy schema)
				var xpath = uQuery.IsLegacyXmlSchema() ? 
					"descendant::node[@id and normalize-space(data[@alias='umbracoPageNotFound'])][1]" : 
					"descendant::*[@isDoc and normalize-space(umbracoPageNotFound)][1]";

				// get the nodes
				var nodes = uQuery.GetNodesByXPath(xpath);

				if (nodes.Count > 0)
				{
					// get the first node
					var node = nodes[0];

					// get the property that holds the node id for the 404 page
					var errorId = node.GetProperty<string>("umbracoPageNotFound");

					// if the node id is numeric, then set the redirectId
					success = int.TryParse(errorId, out this._redirectId);
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