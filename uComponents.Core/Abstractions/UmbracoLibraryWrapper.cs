using System;
using uComponents.Core.Interfaces;
using umbraco;

namespace uComponents.Core.Abstractions
{
	/// <summary>
	/// A wrapper class for the <c>umbraco.library</c> methods.
	/// </summary>
	/// <remarks>
	/// This wrapper class is used in order to easily switch during unit-testing.
	/// </remarks>
	public class UmbracoLibraryWrapper : IUmbracoLibrary
	{
		/// <summary>
		/// Gets the NiceUrl.
		/// </summary>
		/// <param name="nodeId">The node id.</param>
		/// <returns></returns>
		public string NiceUrl(int nodeId)
		{
			return library.NiceUrl(nodeId);
		}

		/// <summary>
		/// Gets the random.
		/// </summary>
		/// <returns></returns>
		public Random GetRandom()
		{
			return library.GetRandom();
		}

		/// <summary>
		/// Gets the random.
		/// </summary>
		/// <param name="seed">The seed.</param>
		/// <returns></returns>
		public Random GetRandom(int seed)
		{
			return library.GetRandom(seed);
		}
	}
}