using System.Collections.Generic;

namespace uComponents.Core.DataTypes.TabsToDropDownPanel
{
    /// <summary>
    /// Options as set by using the PreValueEditor
    /// </summary>
    internal class TabsToDropDownOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabsToDropDownOptions"/> class.
        /// </summary>
        public TabsToDropDownOptions()
        {
            this.TabIds = new List<int>();
            this.ShowLabel = true;
        }

        /// <summary>
        /// Gets or sets the tab ids to convert hide and toggle with the drop down
        /// </summary>
        public List<int> TabIds { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the label is shown
        /// </summary>
        public bool ShowLabel { get; set; }
    }
}
