namespace uComponents.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	using Umbraco.Core.PropertyEditors;

	using umbraco.interfaces;

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

				var converters = new List<IPropertyEditorValueConverter>();
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();

				foreach (var type in assemblies.SelectMany(x => x.GetTypes()).Where(x => typeof(IPropertyEditorValueConverter).IsAssignableFrom(x) && x.IsClass))
				{
					var instance = (IPropertyEditorValueConverter)Activator.CreateInstance(type);
					converters.Add(instance);
				}

				return propertyValueConverters = converters;
			}

			/// <summary>
			/// Gets the property value converter for the specified datatype.
			/// </summary>
			/// <param name="dataType">The datatype.</param>
			/// <returns>A property value converter.</returns>
			public static IPropertyEditorValueConverter GetPropertyValueConverter(IDataType dataType)
			{
				var converters = GetPropertyValueConverters();

				return converters.FirstOrDefault(x => x.IsConverterFor(dataType.Id, string.Empty, string.Empty));
			}
		}
	}
}