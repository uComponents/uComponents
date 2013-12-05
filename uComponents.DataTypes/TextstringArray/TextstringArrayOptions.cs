using System.ComponentModel;
using umbraco.editorControls;

namespace uComponents.DataTypes.TextstringArray
{
	/// <summary>
	/// The options for the Multiple Textstring data-type.
	/// </summary>
	public class TextstringArrayOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TextstringArrayOptions"/> class.
		/// </summary>
		public TextstringArrayOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextstringArrayOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public TextstringArrayOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets the items per row.
		/// </summary>
		/// <value>The items per row.</value>
		[DefaultValue(2)]
		public int ItemsPerRow { get; set; }

		/// <summary>
		/// Gets or sets the maximum rows.
		/// </summary>
		/// <value>The maximum rows.</value>
		[DefaultValue(-1)]
		public int MaximumRows { get; set; }

		/// <summary>
		/// Gets or sets the minimum rows.
		/// </summary>
		/// <value>The minimum rows.</value>
		[DefaultValue(1)]
		public int MinimumRows { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [show column labels].
		/// </summary>
		/// <value><c>true</c> if [show column labels]; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ShowColumnLabels { get; set; }

		/// <summary>
		/// Gets or sets the labels columns (displayed in the header row).
		/// </summary>
		/// <value>The column labels.</value>
		[DefaultValue("")]
		public string ColumnLabels { get; set; }
	}
}
