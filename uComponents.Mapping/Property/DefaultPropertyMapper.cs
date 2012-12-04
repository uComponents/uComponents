using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using umbraco.NodeFactory;

namespace uComponents.Mapping.Property
{
    internal class DefaultPropertyMapper : PropertyMapperBase
    {
        private Func<Node, string> _mapping;

        public DefaultPropertyMapper(
            Func<Node, string> mapping,
            NodeMapper nodeMapper,
            PropertyInfo destinationProperty
            )
            :base(nodeMapper, destinationProperty, null)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }

            RequiresInclude = false;
            AllowCaching = true;
            _mapping = mapping;
        }

        public override object MapProperty(NodeMappingContext context)
        {
            // Implement this in a derived class.
            throw new NotImplementedException();
        }
    }
}
