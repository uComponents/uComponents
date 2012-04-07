using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uComponents.DataTypes.EnumDropDownList
{
	/// <summary>
	/// Attribute that can be applied to enum fields, to configure how the EnumDropDownList renders it's items
	/// </summary>
	public class EnumDropDownListAttribute : Attribute
	{
		/// <summary>
		/// Sets Text of property of the ListItem in the drop down list
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Sets the Value proeprty of the ListItem in the drop down list
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this enum field should be enabled in the drop down list
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumDropDownListAttribute"/> class.
		/// </summary>
		public EnumDropDownListAttribute()
		{
			this.Enabled = true;
		}
	}
}
