using System.ComponentModel;
using umbraco.editorControls;

namespace uComponents.DataTypes.EnumCheckBoxList
{
	/// <summary>
	/// The options for the EnumCheckBoxList data-type.
	/// </summary>
	public class EnumCheckBoxListOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumCheckBoxListOptions"/> class.
		/// </summary>
		public EnumCheckBoxListOptions()
		{
			this.Assembly = string.Empty;
			this.Enum = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumCheckBoxListOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public EnumCheckBoxListOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Assembly in which to look for an enum
		/// </summary>
		[DefaultValue("")]
		public string Assembly { get; set; }

		/// <summary>
		/// Enum to use as the values for the drop down list
		/// </summary>
		[DefaultValue("")]
		public string Enum { get; set; }

		/// <summary>
		/// Defaults to true, where the property value will be stored as an Xml Fragment, else if false, a Csv will be stored
		/// </summary>
		[DefaultValue(true)]
		public bool UseXml { get; set; }
	}
}
