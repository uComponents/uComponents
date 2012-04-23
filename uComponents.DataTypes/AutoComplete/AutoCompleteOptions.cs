
namespace uComponents.DataTypes.AutoComplete
{
    /// <summary>
    /// Configuration options for AutoComplete
    /// </summary>
    internal class AutoCompleteOptions
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoCompleteOptions"/> class.
		/// </summary>
		public AutoCompleteOptions()
		{
			this.TypeToPick = uQuery.UmbracoObjectType.Document.GetGuid().ToString();
			this.XPath = "descendant::*";
		}

		/// <summary>
		/// Gets or sets the type to pick.
		/// </summary>
		/// <value>The type to pick.</value>
		/// <remarks>The type of object to pick. The default is Document</remarks>
        public string TypeToPick { get; set; }

		/// <summary>
		/// Gets or sets the XPath expression.
		/// </summary>
		/// <value>The XPath expression.</value>
		/// <remarks>Default to "descendant::*"</remarks>
        public string XPath { get; set; }
    }
}