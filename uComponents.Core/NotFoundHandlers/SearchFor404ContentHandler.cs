using System.Net;
using System.Web;
using umbraco.interfaces;
using umbraco.presentation.nodeFactory;

namespace uComponents.Core.NotFoundHandlers
{
	/// <summary>
	/// A NotFoundHandler for finding the nearest 404 page.
	/// </summary>
	public class SearchFor404ContentHandler : INotFoundHandler
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
		/// <returns>
		/// Returns whether the NotFoundHandler has a match.
		/// </returns>
		public bool Execute(string url)
		{
			HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;

			var success = false;

			try
			{
				Node bestNode = null;
				var allErrorPages = uQuery.GetNodesByType("ErrorPage");
				var bestUrlMatchLength = -1;

				foreach (var possiblePage in allErrorPages)
				{
					var matchLength = this.CalculateMatchLength(possiblePage.NiceUrl, "/" + url);
					if (matchLength > bestUrlMatchLength)
					{
						bestUrlMatchLength = matchLength;
						bestNode = possiblePage;
					}
				}

				if (bestNode != null)
				{
					this._redirectId = bestNode.Id;
					success = true;
				}
			}
			catch
			{
				success = false;
			}

			return success;
		}

		/// <summary>
		/// Calculate how much of the first part of string a matches with the first part of string b.
		/// </summary>
		/// <param name="a">The first parameter.</param>
		/// <param name="b">The second parameter.</param>
		/// <returns>Returns the length of the match.</returns>
		public int CalculateMatchLength(string a, string b)
		{
			var i = 0;

			for (; i < a.Length && i < b.Length && a[i] == b[i]; i++)
			{
			}

			return i;
		}
	}
}