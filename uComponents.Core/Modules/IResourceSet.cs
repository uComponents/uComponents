using System.Collections.Generic;

namespace uComponents.Core.Modules
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