using umbraco.cms.businesslogic.relation;
using System.Web.Script.Serialization;

namespace uComponents.Core.DataTypes.RelationLinks
{
    /// <summary>
    /// Options class for the RelatedLinks datatype
    /// </summary>
    public class RelationLinksOptions
    {
        /// <summary>
        /// The Id of the RelationType to use 
        /// </summary>        
        public int RelationTypeId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationLinksOptions"/> class.
        /// </summary>
        public RelationLinksOptions()
        {
            this.RelationTypeId = -1;
        }
    }
}
