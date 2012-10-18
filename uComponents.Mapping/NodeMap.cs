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
    public class NodeMap
    {
        public Type Type { get; set; }
        public List<NodePropertyMap> PropertyMappings { get; set; }

        public NodeMap(Type destinationType)
        {
            Type = destinationType;
            PropertyMappings = new List<NodePropertyMap>();
        }
    }

    public class NodeMap<TDestination> : NodeMap
    {
        public NodeMap()
            : base(typeof(TDestination))
        {
        }
    }
}
