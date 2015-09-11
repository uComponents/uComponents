
using System.Collections;
using System.Collections.Generic;
using umbraco.NodeFactory;
using umbraco;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using System.Xml;

namespace uComponents.DataTypes.XPathAutoComplete
{
    public class XPathAutoComplete : uQuery.IGetProperty
    {
        public XPathAutoComplete()
        {

        }

        public IEnumerable<Node> SelectedNodes
        {
            get;
            private set;
        }

        public IEnumerable<Media> SelectedMedia            
        {
            get;
            private set;
        }

        public IEnumerable<Member> SelectedMembers
        {
            get;
            private set;
        }

        uQuery.IGetProperty.LoadPropertyValue(string value)
        {
            
        }
    }
}
