namespace uComponents.DataTypes.Shared.Models
{
	/// <summary>
	/// Model for an AutoCompleteItem
	/// </summary>
	/// <remarks>
	/// For use with SqlAutoComplete and XPathAutoComplete data-types
	/// </remarks>
	public class AutoCompleteItem
	{
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value { get; set; }
	}
}