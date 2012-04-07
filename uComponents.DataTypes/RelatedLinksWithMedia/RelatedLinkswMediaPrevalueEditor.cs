using System;

using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.DataTypes.RelatedLinksWithMedia
{
    /// <summary>
    /// The PreValue Editor for the Related Links with Media data-type.
    /// </summary>
    public class RelatedLinkswMediaPrevalueEditor : NoOptionsPrevalueEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedLinkswMediaPrevalueEditor"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        public RelatedLinkswMediaPrevalueEditor(BaseDataType dataType)
            : base(dataType, DBTypes.Ntext)
        {
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public string Configuration
        {
            get
            {
                return string.Empty;
            }
        }
    }
}
