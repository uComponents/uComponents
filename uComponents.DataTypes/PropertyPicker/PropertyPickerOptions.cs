using System.ComponentModel;
using umbraco.editorControls;

namespace uComponents.DataTypes.PropertyPicker
{
	/// <summary>
	/// Property Picker Options
	/// </summary>
	public class PropertyPickerOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyPickerOptions"/> class.
		/// </summary>
		public PropertyPickerOptions()
			: base()
		{
		}

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		[DefaultValue("")]
		public string ContentTypeId { get; set; }

		/// <summary>
		/// Gets or sets the type of the object.
		/// </summary>
		/// <value>The type of the object.</value>
		[DefaultValue("")]
		public string ObjectTypeId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [include default attributes].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [include default attributes]; otherwise, <c>false</c>.
		/// </value>
		[DefaultValue(false)]
		public bool IncludeDefaultAttributes { get; set; }
	}
}