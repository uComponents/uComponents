using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using umbraco.NodeFactory;
using umbraco;

namespace uComponents.Mapping
{
    public class NodeMapper
    {
        public Type Type { get; set; }
        public List<NodePropertyMapper> PropertyMappers { get; set; }

        public NodeMapper(Type destinationType)
        {
            Type = destinationType;
            PropertyMappers = new List<NodePropertyMapper>();
        }
    }

    public class NodeMapper<TDestination> : NodeMapper
    {
        public NodeMapper()
            : base(typeof(TDestination))
        {
        }
    }
}
