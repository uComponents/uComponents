using System;
using uComponents.Core.Interfaces;

namespace uComponents.Core.UnitTests.Abstractions
{
	/// <summary>
	/// Mock <c>umbraco.library</c> which can return canned results to ease unit-testing.
	/// </summary>
	public sealed class MockUmbracoLibrary : IUmbracoLibrary
	{
		private string DefaultNiceUrl = "/some/test/data/1234.aspx";

		public MockUmbracoLibrary()
		{
		}

		public MockUmbracoLibrary(string defaultNiceUrl)
		{
			this.DefaultNiceUrl = defaultNiceUrl;
		}

		public string NiceUrl(int nodeId)
		{
			return this.DefaultNiceUrl;
		}

		public Random GetRandom()
		{
			return GetRandom(0);
		}

		public Random GetRandom(int seed)
		{
			return new Random(seed);
		}
	}
}