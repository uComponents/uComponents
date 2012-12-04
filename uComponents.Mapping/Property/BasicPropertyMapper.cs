using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace uComponents.Mapping.Property
{
    internal class BasicPropertyMapper : PropertyMapperBase
    {
        private BasicPropertyMapping _mapping;

        public BasicPropertyMapper(
            BasicPropertyMapping mapping,
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
