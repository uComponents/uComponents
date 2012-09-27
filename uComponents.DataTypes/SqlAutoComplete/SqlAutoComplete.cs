
using System.Collections;
using System.Collections.Generic;
using umbraco.NodeFactory;
using umbraco;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using System.Xml;

namespace uComponents.DataTypes.SqlAutoComplete
{
    public class SqlAutoComplete : uQuery.IGetProperty
    {
        public SqlAutoComplete()
        {
        }

        public IEnumerable<KeyValuePair<string, string>> SelectedItems
        {
            get;
            private set;
        }


        uQuery.IGetProperty.LoadPropertyValue(string value)
        {
            
        }
    }
}
