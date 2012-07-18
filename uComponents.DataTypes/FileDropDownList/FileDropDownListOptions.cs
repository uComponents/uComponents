using System.ComponentModel;
using umbraco.editorControls;

namespace uComponents.DataTypes.FileDropDownList
{
	/// <summary>
	/// The options for the File DropDownList data-type.
	/// </summary>
	public class FileDropDownListOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileDropDownListOptions"/> class.
		/// </summary>
		public FileDropDownListOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileDropDownListOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public FileDropDownListOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets the directory.
		/// </summary>
		/// <value>The directory.</value>
		[DefaultValue("~/")]
		public string Directory { get; set; }

		/// <summary>
		/// Gets or sets the search pattern.
		/// </summary>
		/// <value>The search pattern.</value>
		[DefaultValue("*.*")]
		public string SearchPattern { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [use directories].
		/// </summary>
		/// <value><c>true</c> if [use directories]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public bool UseDirectories { get; set; }
	}
}
