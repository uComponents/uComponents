using System.ComponentModel;

using uComponents.DataTypes.Shared.PrevalueEditors;

namespace uComponents.DataTypes.UserPicker
{
	/// <summary>
	/// The options for the User Picker data-type.
	/// </summary>
	public class UserPickerOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserPickerOptions"/> class.
		/// </summary>
		public UserPickerOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserPickerOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public UserPickerOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether [use drop down].
		/// </summary>
		/// <value><c>true</c> if [use drop down]; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool UseDropDown { get; set; }

		/// <summary>
		/// Gets or sets the selected user types.
		/// </summary>
		/// <value>The selected user types.</value>
		[DefaultValue(new[] { "admin", "writer", "editor", "translator" })]
		public string[] SelectedUserTypes { get; set; }
	}
}
