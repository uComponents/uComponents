using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uComponents.UI
{
	/// <summary>
	/// Information about a resource that can be embedded in pages
	/// </summary>
	public class ResourceRegistration
	{
		/// <summary>
		/// The name of the embedded resource
		/// </summary>
		public string ResourceName { get; set; }

		/// <summary>
		/// Is the resource a script or a stylesheet?
		/// </summary>
		public bool IsScript { get; set; }

		/// <summary>
		/// If this is specified the resource is only included if one of these matches Request.CurrentExecutionFilePath
		/// </summary>
		public IEnumerable<Regex> Filters { get; set; }

		/// <summary>
		/// If this is specified it is possible to filter the ressources based on string, e.g. "shortcuts". Use "*" for any
		/// </summary>
		public IEnumerable<string> Purposes { get; set; }
	}
}