namespace uComponents.Core.DataTypes.PropertyPicker
{
	/// <summary>
	/// Property Picker Options
	/// </summary>
	public class PropertyPickerOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyPickerOptions"/> class.
		/// </summary>
		public PropertyPickerOptions()
		{
			this.ContentTypeId = string.Empty;
			this.ObjectTypeId = string.Empty;
		}

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		public string ContentTypeId { get; set; }

		/// <summary>
		/// Gets or sets the type of the object.
		/// </summary>
		/// <value>The type of the object.</value>
		public string ObjectTypeId { get; set; }
	}
}