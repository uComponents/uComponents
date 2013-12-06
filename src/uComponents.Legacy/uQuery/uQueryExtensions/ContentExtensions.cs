using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using System.Xml;
using umbraco.MacroEngines;

namespace uComponents.Core.uQueryExtensions
{
	/// <summary>
	/// uQuery extensions for the Content object
	/// </summary>
	[Obsolete(Constants.uQueryLegacyObsoleteMessage)]
	public static class ContentExtensions
	{
		/// <summary>
		/// Determines whether the specified content item has property.
		/// </summary>
		/// <param name="item">The content item.</param>
		/// <param name="propertyAlias">The property alias.</param>
		/// <returns>
		/// 	<c>true</c> if the specified content item has property; otherwise, <c>false</c>.
		/// </returns>
		public static bool HasProperty(this Content item, string propertyAlias)
		{
			var property = item.getProperty(propertyAlias);
			return (property != null);
		}

		/// <summary>
		/// Get a value (of specified type) from a content item's property.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="item">The content item.</param>
		/// <param name="propertyAlias">alias of property to get</param>
		/// <returns>default(T) or property value cast to (T)</returns>
		public static T GetProperty<T>(this Content item, string propertyAlias)
		{
			var typeConverter = TypeDescriptor.GetConverter(typeof(T));

			if (typeConverter != null)
			{
				// Boolean
				if (typeof(T) == typeof(bool))
				{
#pragma warning disable 0618
					return (T)typeConverter.ConvertFrom(item.GetPropertyAsBoolean(propertyAlias).ToString());
#pragma warning restore 0618
				}

				// XmlDocument
				else if (typeof(T) == typeof(XmlDocument))
				{
					var xmlDocument = new XmlDocument();

					try
					{
#pragma warning disable 0618
						xmlDocument.LoadXml(item.GetPropertyAsString(propertyAlias));
#pragma warning restore 0618
					}
					catch
					{
					}

					return (T)((object)xmlDocument);
				}

				// umbraco.MacroEngines.DynamicXml
				else if (typeof(T) == typeof(DynamicXml))
				{
					try
					{
#pragma warning disable 0618
						return (T)((object)new DynamicXml(item.GetPropertyAsString(propertyAlias)));
#pragma warning restore 0618
					}
					catch
					{
					}
				}

				try
				{
#pragma warning disable 0618
					return (T)typeConverter.ConvertFromString(item.GetPropertyAsString(propertyAlias));
#pragma warning restore 0618
				}
				catch
				{
				}
			}

			return default(T);
		}

		/// <summary>
		/// Get a string value from a content item's property.
		/// </summary>
		/// <param name="item">The content item.</param>
		/// <param name="propertyAlias">alias of propety to get</param>
		/// <returns>
		/// empty string, or property value as string
		/// </returns>
		[Obsolete("Please use the .GetProperty<T>() extension method - eg. .GetProperty<string>(\"propertyAlias\");")]
		public static string GetPropertyAsString(this Content item, string propertyAlias)
		{
			var propertyValue = string.Empty;
			var property = item.getProperty(propertyAlias);

			if (property != null)
			{
				propertyValue = Convert.ToString(property.Value);
			}

			return propertyValue;
		}

		/// <summary>
		/// Get a boolean value from a content item's property, (works with built in Yes/No dataype).
		/// </summary>
		/// <param name="item">The content item.</param>
		/// <param name="propertyAlias">alias of propety to get</param>
		/// <returns>
		/// true if can cast value, else false for all other circumstances
		/// </returns>
		[Obsolete("Please use the .GetProperty<T>() extension method - eg. .GetProperty<bool>(\"propertyAlias\");")]
		public static bool GetPropertyAsBoolean(this Content item, string propertyAlias)
		{
			var propertyValue = false;
			var property = item.getProperty(propertyAlias);

			if (property != null)
			{
				// Umbraco yes / no datatype stores a string value of '1' or '0'
				if (Convert.ToString(property.Value) == "1")
				{
					propertyValue = true;
				}
				else
				{
					bool.TryParse(Convert.ToString(property.Value), out propertyValue);
				}
			}

			return propertyValue;
		}

		/// <summary>
		/// Get a DateTime value from a content item's property.
		/// </summary>
		/// <param name="item">The content item.</param>
		/// <param name="propertyAlias">alias of propety to get</param>
		/// <returns>
		/// DateTime value or DateTime.MinValue for all other circumstances
		/// </returns>
		[Obsolete("Please use the .GetProperty<T>() extension method - eg. .GetProperty<DateTime>(\"propertyAlias\");")]
		public static DateTime GetPropertyAsDateTime(this Content item, string propertyAlias)
		{
			var propertyValue = DateTime.MinValue;
			var property = item.getProperty(propertyAlias);

			if (property != null)
			{
				DateTime.TryParse(Convert.ToString(property.Value), out propertyValue);
			}

			return propertyValue;
		}

		/// <summary>
		/// Get an int value from a content item's property.
		/// </summary>
		/// <param name="item">The content item.</param>
		/// <param name="propertyAlias">alias of propety to get</param>
		/// <returns>
		/// int value of property or int.MinValue for all other circumstances
		/// </returns>
		[Obsolete("Please use the .GetProperty<T>() extension method - eg. .GetProperty<int>(\"propertyAlias\");")]
		public static int GetPropertyAsInt(this Content item, string propertyAlias)
		{
			var propertyValue = int.MinValue;
			var property = item.getProperty(propertyAlias);

			if (property != null)
			{
				int.TryParse(Convert.ToString(property.Value), out propertyValue);
			}

			return propertyValue;
		}

		/// <summary>
		/// Gets the random content item.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <param name="items">The content items.</param>
		/// <returns>
		/// Returns a random content item from a collection of content items.
		/// </returns>
		public static TSource GetRandom<TSource>(this ICollection<TSource> items)
		{
			return items.RandomOrder().First();
		}

		/// <summary>
		/// Gets a collection of random content items.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <param name="items">The content items.</param>
		/// <param name="numberOfItems">The number of items.</param>
		/// <returns>
		/// Returns the specified number of random content items from a collection of content items.
		/// </returns>
		public static IEnumerable<TSource> GetRandom<TSource>(this ICollection<TSource> items, int numberOfItems)
		{
			if (numberOfItems > items.Count)
			{
				numberOfItems = items.Count;
			}

			return items.RandomOrder().Take(numberOfItems);
		}

		/// <summary>
		/// Randomizes the order of the content items.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <param name="items">The content items.</param>
		/// <returns>Returns a list of content items in a random order.</returns>
		public static IEnumerable<TSource> RandomOrder<TSource>(this IEnumerable<TSource> items)
		{
			var random = umbraco.library.GetRandom();
			return items.OrderBy(x => (random.Next()));
		}

		/// <summary>
		/// Sets a property value, and returns self.
		/// </summary>
		/// <param name="item">The content item.</param>
		/// <param name="propertyAlias">The alias of property to set.</param>
		/// <param name="value">The value to set.</param>
		/// <returns>
		/// The same content item on which this is an extension method.
		/// </returns>
		public static Content SetProperty(this Content item, string propertyAlias, object value)
		{
			var property = item.getProperty(propertyAlias);

			if (property != null)
			{
				if (value != null)
				{
					var dataTypeGuid = property.PropertyType.DataTypeDefinition.DataType.Id.ToString();

					// switch based on datatype of property being set - if setting a built in ddl or radion button list, then string supplied is checked against prevalues
					switch (dataTypeGuid.ToUpper())
					{
						case "A74EA9C9-8E18-4D2A-8CF6-73C6206C5DA6": // DropDownList
						case "A52C7C1C-C330-476E-8605-D63D3B84B6A6": // RadioButtonList

							var preValues = PreValues.GetPreValues(property.PropertyType.DataTypeDefinition.Id);
							PreValue preValue = null;

							// switch based on the supplied value type
							switch (Type.GetTypeCode(value.GetType()))
							{
								case TypeCode.String:
									// attempt to get prevalue from the label
									preValue = preValues.Values.Cast<PreValue>().Where(x => x.Value == (string)value).FirstOrDefault();
									break;

								case TypeCode.Int16:
								case TypeCode.Int32:
									// attempt to get prevalue from the id
									preValue = preValues.Values.Cast<PreValue>().Where(x => x.Id == (int)value).FirstOrDefault();
									break;
							}

							if (preValue != null)
							{
								// check db field type being saved to and store prevalue id as an int or a string - note can never save a prevalue id to a date field ! 
								switch (((DefaultData)property.PropertyType.DataTypeDefinition.DataType.Data).DatabaseType)
								{
									case DBTypes.Ntext:
									case DBTypes.Nvarchar:
										property.Value = preValue.Id.ToString();
										break;

									case DBTypes.Integer:
										property.Value = preValue.Id;
										break;
								}
							}

							break;

						case "23E93522-3200-44E2-9F29-E61A6FCBB79A": // Date (NOTE: currently assumes database type is set to Date)

							switch (Type.GetTypeCode(value.GetType()))
							{
								case TypeCode.DateTime:
									property.Value = ((DateTime)value).Date;
									break;
								case TypeCode.String:
									DateTime valueDateTime;
									if (DateTime.TryParse((string)value, out valueDateTime))
									{
										property.Value = valueDateTime.Date;
									};
									break;
							}

							break;

						default:
							// This saves the property value
							property.Value = value;
							break;
					}
				}
				else
				{
					// the value is NULL
					property.Value = value;
				}
			}

			item.Save();

			return item;
		}
	}
}