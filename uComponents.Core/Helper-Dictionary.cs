using umbraco;

namespace uComponents.Core
{
	/// <summary>
	/// Generic helper methods
	/// </summary>
	public static partial class Helper
	{
		/// <summary>
		/// Dictionary helpers
		/// </summary>
		public static class Dictionary
		{
			/// <summary>
			/// Gets the dictionary item with uComponents prefix ("uComponents.")
			/// </summary>
			/// <param name="key">The dictionary item key.</param>
			/// <param name="fallback">The fallback text.</param>
			/// <returns>The dictionary item value as a string.</returns>
			public static string GetDictionaryItem(string key, string fallback)
			{
				return uQuery.GetDictionaryItem(string.Concat(Constants.ApplicationName, Constants.Common.DOT, key), fallback);
			}
		}
	}
}
