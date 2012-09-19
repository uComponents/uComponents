using System;
using System.ComponentModel;
using System.Configuration;
using umbraco;
using umbraco.editorControls;

namespace uComponents.DataTypes.XPathAutoComplete
{
    internal class XPathAutoCompleteOptions : AbstractOptions
    {
        /// <summary>
        /// Initializes an instance of XPathAutoCompleteOptions
        /// </summary>
        public XPathAutoCompleteOptions()
        {
        }

        public XPathAutoCompleteOptions(bool loadDefaults)
            : base(loadDefaults)
        {
        }

        [DefaultValue("")]
        public string Type { get; set; }

        [DefaultValue("")]
        public string XPath { get; set; }

        [DefaultValue("")]
        public string Property { get; set; }

        [DefaultValue(3)]
        public int MinLength { get; set; }

        [DefaultValue(0)]
        public int MinItems { get; set; }

        [DefaultValue(0)]
        public int MaxItems { get; set; }

        /// <summary>
        /// Helper to get the UmbracoObjectType from the stored value
        /// </summary>
        public uQuery.UmbracoObjectType UmbracoObjectType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Type))
                {
                    return uQuery.UmbracoObjectType.Document;
                }
                else
                {
                    return uQuery.GetUmbracoObjectType(new Guid(this.Type));    
                }
            }
        }

    }
}
