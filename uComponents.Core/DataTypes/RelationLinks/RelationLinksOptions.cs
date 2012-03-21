using umbraco.cms.businesslogic.relation;
using System.Web.Script.Serialization;

namespace uComponents.Core.DataTypes.RelationLinks
{    
    public class RelationLinksOptions
    {

        /// <summary>
        /// The Id of the RelationType to use 
        /// </summary>        
        public int RelationTypeId { get; set; }

        ///// <summary>
        ///// Don't serialize this to the persisted settings
        ///// </summary>
        //[ScriptIgnore]
        //public RelationType RelationType
        //{
        //    get
        //    {
        //        if (this.RelationTypeId > 0)
        //        {
        //            return new RelationType(this.RelationTypeId);
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        public RelationLinksOptions()
        {
            this.RelationTypeId = -1;
        }

    }
}
