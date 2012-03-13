using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uComponents.Core.DataTypes.EnumDropDownList
{
	internal class EnumDropDownListOptions
	{
		/// <summary>
		/// Assembly in which to look for an enum
		/// </summary>
		public string Assembly { get; set; }

		/// <summary>
		/// Enum to use as the values for the drop down list
		/// </summary>
		public string Enum { get; set; }

        /// <summary>
        /// If true, then the first item in the enum will be selected
        /// </summary>
        public bool DefaultToFirstItem { get; set; }

		/// <summary>
		/// Initializes an instance of EnumDropDownListOptions
		/// </summary>
		public EnumDropDownListOptions()
		{
			this.Assembly = string.Empty;
			this.Enum = string.Empty;
		}
	}
}
