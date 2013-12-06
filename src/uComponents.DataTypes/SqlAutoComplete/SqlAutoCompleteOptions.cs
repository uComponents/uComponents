using System;
using System.ComponentModel;
using System.Configuration;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco;

namespace uComponents.DataTypes.SqlAutoComplete
{
	/// <summary>
	/// The options for the SqlAutoCompleteOptions data-type.
	/// </summary>
	public class SqlAutoCompleteOptions : SqlOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlAutoCompleteOptions"/> class.
		/// </summary>
		public SqlAutoCompleteOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlAutoCompleteOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public SqlAutoCompleteOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets the length of the min.
		/// </summary>
		/// <value>The length of the min.</value>
		[DefaultValue(1)]
		public int MinLength { get; set; }

		/// <summary>
		/// Gets or sets the max suggestions.
		/// </summary>
		/// <value>The max suggestions.</value>
		[DefaultValue(0)]
		public int MaxSuggestions { get; set; }

		/// <summary>
		/// Gets or sets the min items.
		/// </summary>
		/// <value>The min items.</value>
		[DefaultValue(0)]
		public int MinItems { get; set; }

		/// <summary>
		/// Gets or sets the max items.
		/// </summary>
		/// <value>The max items.</value>
		[DefaultValue(0)]
		public int MaxItems { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [allow duplicates].
		/// </summary>
		/// <value><c>true</c> if [allow duplicates]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public bool AllowDuplicates { get; set; }
	}
}
