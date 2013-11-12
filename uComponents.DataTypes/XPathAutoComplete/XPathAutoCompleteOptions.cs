using System;
using System.ComponentModel;
using uComponents.Core;
using umbraco;
using umbraco.editorControls;

namespace uComponents.DataTypes.XPathAutoComplete
{
	/// <summary>
	/// The options for the XPathAutoCompleteOptions data-type.
	/// </summary>
	public class XPathAutoCompleteOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XPathAutoCompleteOptions"/> class.
		/// </summary>
		public XPathAutoCompleteOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathAutoCompleteOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public XPathAutoCompleteOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		[DefaultValue(Constants.Umbraco.ObjectTypes.Document)]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the X path.
		/// </summary>
		/// <value>The X path.</value>
		[DefaultValue("//*")]
		public string XPath { get; set; }

		/// <summary>
		/// Gets or sets the property.
		/// </summary>
		/// <value>The property.</value>
		[DefaultValue("")]
		public string Property { get; set; }

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

		/// <summary>
		/// Helper to get the UmbracoObjectType from the stored string guid
		/// </summary>
		/// <value>The type of the umbraco object.</value>
		public uQuery.UmbracoObjectType UmbracoObjectType
		{
			get
			{
				if (string.IsNullOrEmpty(this.Type))
				{
					return uQuery.GetUmbracoObjectType(new Guid(Constants.Umbraco.ObjectTypes.Document));
				}

				return uQuery.GetUmbracoObjectType(new Guid(this.Type));
			}
		}
	}
}
