using System.ComponentModel;

namespace uComponents.DataTypes.EnumCheckBoxList
{
	internal class EnumCheckBoxListOptions
	{
		/// <summary>
		/// Initializes an instance of EnumCheckBoxListOptions
		/// </summary>
		public EnumCheckBoxListOptions()
		{
			this.Assembly = string.Empty;
			this.Enum = string.Empty;
		}

		/// <summary>
		/// Assembly in which to look for an enum
		/// </summary>
		public string Assembly { get; set; }

		/// <summary>
		/// Enum to use as the values for the drop down list
		/// </summary>
		public string Enum { get; set; }


		/// <summary>
		/// Defaults to true, where the property value will be stored as an Xml Fragment, else if false, a Csv will be stored
		/// </summary>
		[DefaultValue(true)]
		public bool UseXml { get; set; }
	}
}
