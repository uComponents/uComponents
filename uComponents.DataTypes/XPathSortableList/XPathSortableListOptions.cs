using System;
using System.ComponentModel;
using uComponents.Core;
using umbraco;
using umbraco.editorControls;

namespace uComponents.DataTypes.XPathSortableList
{
    /// <summary>
    /// The options for the XPathSortableListOptions data-type.
    /// </summary>
    public class XPathSortableListOptions : AbstractOptions
    {
        private string type = null;

        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            get
            {
                if (this.type == null)
                {
                    return uQuery.UmbracoObjectType.Document.GetGuid().ToString();
                }

                return this.type;
            }

            set
            {
                this.type = value;
            }
        }

        /// <summary>
        /// Gets or sets the X path.
        /// </summary>
        /// <value>The X path.</value>
        [DefaultValue("//*")]
        public string XPath { get; set; }

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
                return uQuery.GetUmbracoObjectType(new Guid(this.Type));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPathSortableListOptions"/> class.
        /// </summary>
        public XPathSortableListOptions()
            : base(true)
        {
        }
    }
}
