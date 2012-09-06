using System;
using System.Web;

namespace uComponents.Core.Modules
{
	/// <summary>
	/// Placeholder for legacy UI HttpModule. Remains here to support an upgrade path.
	/// </summary>
	[Obsolete("Please use uComponents.UI.uComponentsModule", true)]
	public class uComponentsModule : IHttpModule
	{
		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
		/// </summary>
		public void Dispose() { }

		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
		public void Init(HttpApplication context) { }
	}
}
