using System.ComponentModel;
using uComponents.Core;
using umbraco.editorControls;

namespace uComponents.DataTypes.CheckBoxTree
{
	/// <summary>
	/// The options for the CheckBoxTree data-type.
	/// </summary>
	public class CheckBoxTreeOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CheckBoxTreeOptions"/> class.
		/// </summary>
		public CheckBoxTreeOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CheckBoxTreeOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public CheckBoxTreeOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// The expand options.
		/// </summary>
		public enum ExpandOptions
		{
			/// <summary>
			/// Collapse All
			/// </summary>
			None = 0,

			/// <summary>
			/// Expand All
			/// </summary>
			All = 1,

			/// <summary>
			/// Expand Selected
			/// </summary>
			Selected = 2
		}

		/// <summary>
		/// Gets or sets the start tree node X path.
		/// </summary>
		/// <value>The start tree node X path.</value>
		[DefaultValue("//*")]
		public string StartTreeNodeXPath { get; set; }

		/// <summary>
		/// Gets or sets the selectable tree nodes X path.
		/// </summary>
		/// <value>The selectable tree nodes X path.</value>
		[DefaultValue("//*")]
		public string SelectableTreeNodesXPath { get; set; }

		/// <summary>
		/// Gets or sets the output format.
		/// </summary>
		/// <value>The output format.</value>
		[DefaultValue(Settings.OutputFormat.XML)]
		public Settings.OutputFormat OutputFormat { get; set; }

		/// <summary>
		/// Gets or sets the min selection.
		/// </summary>
		/// <value>The min selection.</value>
		[DefaultValue(0)]
		public int MinSelection { get; set; }

		/// <summary>
		/// Gets or sets the max selection.
		/// </summary>
		/// <value>The max selection.</value>
		[DefaultValue(0)]
		public int MaxSelection { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [select ancestors].
		/// </summary>
		/// <value><c>true</c> if [select ancestors]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public bool SelectAncestors { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [select descendents].
		/// </summary>
		/// <value><c>true</c> if [select descendents]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public bool ToggleDescendents { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [show tree icons].
		/// </summary>
		/// <value><c>true</c> if [show tree icons]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public bool ShowTreeIcons { get; set; }

		/// <summary>
		/// Gets or sets the expand option.
		/// </summary>
		/// <value>The expand option.</value>
		[DefaultValue(ExpandOptions.Selected)]
		public ExpandOptions ExpandOption { get; set; }
	}
}