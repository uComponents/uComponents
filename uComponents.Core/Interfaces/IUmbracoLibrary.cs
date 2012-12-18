using System;
namespace uComponents.Core.Interfaces
{
	public interface IUmbracoLibrary
	{
		string NiceUrl(int nodeId);

		Random GetRandom();

		Random GetRandom(int seed);
	}
}