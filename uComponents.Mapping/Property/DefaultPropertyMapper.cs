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
            object value = null;

            // Check cache
            if (Engine.CacheProvider != null && Engine.CacheProvider.ContainsPropertyValue(context.Id, DestinationInfo.Name))
            {
                value = Engine.CacheProvider.GetPropertyValue(context.Id, DestinationInfo.Name);
            }
            else
            {
                var node = context.GetNode();

                if (node == null || string.IsNullOrEmpty(node.Name))
                {
                    throw new InvalidOperationException("Node cannot be null or empty");
                }

                value = _mapping(node);

                Engine.CacheProvider.InsertPropertyValue(context.Id, DestinationInfo.Name, value);
            }

            return value;
        }
    }
}
