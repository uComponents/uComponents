using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace uComponents.Mapping.Property
{
    internal class CustomPropertyMapper : PropertyMapperBase
    {
        private CustomPropertyMapping _mapping;

        public CustomPropertyMapper(
            CustomPropertyMapping mapping,
            bool requiresInclude,
            bool allowCaching,
            NodeMapper nodeMapper,
            PropertyInfo destinationProperty,
            string sourcePropertyAlias
            )
            :base(nodeMapper, destinationProperty, sourcePropertyAlias)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }

            RequiresInclude = requiresInclude;
            AllowCaching = allowCaching;
            _mapping = mapping;
        }

        public override object MapProperty(NodeMappingContext context)
        {
            // Implement this in a derived class.
            throw new NotImplementedException();
        }
    }
}
