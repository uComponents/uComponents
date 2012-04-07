using System.Collections.Generic;

namespace uComponents.UI
{
	/// <summary>
	/// A collection of resource registrations that can be embedded in pages. 
	/// </summary>
	public interface IResourceSet
	{
		/// <summary>
		/// The resources to embed.
		/// </summary>
		IEnumerable<ResourceRegistration> Resources { get; }
	}
}