using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

namespace uComponents.DataTypes.Shared.Enums
{
	/// <summary>
	/// Helper methods for working with Enums
	/// </summary>
	public class EnumHelper
	{

		/// <summary>
		/// Gets the Enum of the supplied type and returns its attribute values as a ListItem
		/// </summary>
		/// <param name="enumType">The full type name of the enum to parse</param>
		/// <returns>List of ListItems representing the Enum's attribute values</returns>
		public static IEnumerable<ListItem> GetEnumListAttributeValues(Type enumType)
		{
			var enums = new List<ListItem>();

			foreach (var enumName in Enum.GetNames(enumType))
			{
				var fieldInfo = enumType.GetField(enumName);
				var listItem = new ListItem(enumName, enumName); // Default to the enum item name

				// Loop though any custom attributes that may have been applied the the curent enum item
				foreach (var customAttributeData in CustomAttributeData.GetCustomAttributes(fieldInfo))
				{
					if (customAttributeData.Constructor.DeclaringType != null && (new []{ "EnumDropDownListAttribute", "EnumCheckBoxListAttribute" }.Contains(customAttributeData.Constructor.DeclaringType.Name) && customAttributeData.NamedArguments != null))
					{
						// Loop though each property on the EnumDropDownListAttribute
						foreach (var customAttributeNamedArguement in customAttributeData.NamedArguments)
						{
							switch (customAttributeNamedArguement.MemberInfo.Name)
							{
								case "Text":
									listItem.Text = customAttributeNamedArguement.TypedValue.Value.ToString();
									break;

								case "Value":
									listItem.Value = customAttributeNamedArguement.TypedValue.Value.ToString();
									break;

								case "Enabled":
									listItem.Enabled = (bool)customAttributeNamedArguement.TypedValue.Value;
									break;
							}
						}

						enums.Add(listItem);
					}
				}

			}

			return enums;
		}

	}
}
