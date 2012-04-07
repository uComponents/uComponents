// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 11.01.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;

namespace uComponents.DataTypes.DataTypeGrid.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class PreValueEditorSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether [show label].
		/// </summary>
		/// <value>
		///   <c>true</c> if [show label]; otherwise, <c>false</c>.
		/// </value>
		[DefaultValue(false)]
		public bool ShowLabel { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [show header].
		/// </summary>
		/// <value>
		///   <c>true</c> if [show header]; otherwise, <c>false</c>.
		/// </value>
		[DefaultValue(true)]
		public bool ShowTableHeader { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [show footer].
		/// </summary>
		/// <value>
		///   <c>true</c> if [show footer]; otherwise, <c>false</c>.
		/// </value>
		[DefaultValue(true)]
		public bool ShowTableFooter { get; set; }

		/// <summary>
		/// Gets or sets the number of rows.
		/// </summary>
		/// <value>The number of rows.</value>
		[DefaultValue(10)]
		public int NumberOfRows { get; set; }

		/// <summary>
		/// Gets or sets the content sorting.
		/// </summary>
		/// <value>
		/// The content sorting.
		/// </value>
		[DefaultValue("")]
		public string ContentSorting { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PreValueEditorSettings"/> class.
		/// </summary>
		public PreValueEditorSettings()
		{
			this.NumberOfRows = 10;
			this.ShowLabel = false;
			this.ContentSorting = string.Empty;
			this.ShowTableHeader = true;
			this.ShowTableFooter = true;
		}
	}
}
