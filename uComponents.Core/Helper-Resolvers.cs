namespace uComponents.Core
{
	using System.Collections.Generic;

	using Umbraco.Core.PropertyEditors;

	/// <summary>
	/// Generic helper methods
	/// </summary>
	internal static partial class Helper
	{
		/// <summary>
		/// IO helpers
		/// </summary>
		public static class Resolvers
		{
			/// <summary>
			/// The property value converters
			/// </summary>
			private static IEnumerable<IPropertyEditorValueConverter> propertyValueConverters;

			/// <summary>
			/// Gets the property value converters.
			/// </summary>
			/// <returns>A list of property value converters.</returns>
			public static IEnumerable<IPropertyEditorValueConverter> GetPropertyValueConverters()
			{
				if (propertyValueConverters != null)
				{
					return propertyValueConverters;
				}

				// TODO: Get all IPropertyEditorValueConverters
			    return null;
			}
		}
	}
}