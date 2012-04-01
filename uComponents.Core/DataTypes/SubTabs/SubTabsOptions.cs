using System.Collections.Generic;

namespace uComponents.Core.DataTypes.SubTabs
{
    /// <summary>
    /// Options as set by using the PreValueEditor
    /// </summary>
    internal class SubTabsOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubTabsOptions"/> class.
        /// </summary>
        public SubTabsOptions()
        {
            this.TabIds = new List<int>();
            this.SubTabType = global::SubTabType.Buttons; // Default
            this.ShowLabel = true;
        }

        /// <summary>
        /// Gets or sets the tab ids to convert hide and toggle with the drop down
        /// </summary>
        public List<int> TabIds { get; set; }

        /// <summary>
        /// Gets or sets the type of the sub tab type ( Buttons or DropDownList)
        /// </summary>
        public SubTabType SubTabType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the label is shown
        /// </summary>
        public bool ShowLabel { get; set; }
    }
}
