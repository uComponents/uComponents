using System;
namespace uComponents.Core.Interfaces
{
	/// <summary>
	/// Interface for exposing methods used from <c>umbraco.library</c>.
	/// </summary>
	internal interface IUmbracoLibrary
	{
		/// <summary>
		/// Exposes the <c>umbraco.library.NiceUrl</c> method.
		/// </summary>
		/// <param name="nodeId">The node id.</param>
		/// <returns>Returns the nice URL for the node Id.</returns>
		string NiceUrl(int nodeId);

		/// <summary>
		/// Exposes the <c>umbraco.library.GetRandom</c> method.
		/// </summary>
		/// <returns>Returns a <c>Random</c> object.</returns>
		Random GetRandom();

		/// <summary>
		/// Exposes the <c>umbraco.library.GetRandom</c> method.
		/// </summary>
		/// <param name="seed">The seed.</param>
		/// <returns>Returns a seeded <c>Random</c> object.</returns>
		Random GetRandom(int seed);
	}
}