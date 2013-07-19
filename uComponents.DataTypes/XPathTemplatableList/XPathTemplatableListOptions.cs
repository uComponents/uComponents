using System;
using System.ComponentModel;
using uComponents.Core;
using umbraco;
using umbraco.editorControls;

namespace uComponents.DataTypes.XPathTemplatableList
{
    /// <summary>
    /// The options for the XPathTemplatableListOptions data-type.
    /// </summary>
    public class XPathTemplatableListOptions : AbstractOptions
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
        /// 
        /// </summary>
        [DefaultValue("")]
        public string SortOn { get; set; }

        /// <summary>
        /// http://stackoverflow.com/questions/6372522/does-an-enum-exist-for-asc-or-desc-ordering-in-net
        /// </summary>
        [DefaultValue(ListSortDirection.Ascending)]
        public ListSortDirection SortDirection { get; set; }


        /// <summary>
        /// Gets or sets a limt to the number of source items (0 means no limit)
        /// </summary>
        [DefaultValue(0)]
        public int LimitTo { get; set; }

        /// <summary>
        /// Gets or sets the height of the list control
        /// </summary>
        /// <value>The height</value>
        [DefaultValue(0)]
        public int ListHeight { get; set; }

        /// <summary>
        /// Gets or sets the height of all list items
        /// </summary>
        [DefaultValue(21)]
        public int ItemHeight { get; set; }

        /// <summary>
        /// Gets or sets either "Text Template" or "Macro"
        /// </summary>
        [DefaultValue("Text Template")]
        public string TemplateType { get; set; }

        /// <summary>
        /// Gets or sets a handlebars syntax template, used for the text field of each list item
        /// can use the tokens :media: :member: and :node: between the local property and a property on the respective type
        /// </summary>
        [DefaultValue("{{Name}}")]
        public string TextTemplate { get; set; }

        /// <summary>
        /// Gets or sets the macro by alias to use as the rendering mechanism for each item
        /// </summary>
        [DefaultValue("")]
        public string MacroAlias { get; set; }

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
        /// Initializes a new instance of the <see cref="XPathTemplatableListOptions"/> class.
        /// </summary>
        public XPathTemplatableListOptions()
            : base(true)
        {
        }
    }
}
