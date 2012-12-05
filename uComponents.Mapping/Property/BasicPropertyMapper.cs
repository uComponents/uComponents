using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using umbraco;
using uComponents.Mapping;
using umbraco.NodeFactory;

namespace uComponents.Mapping.Property
{
    internal class BasicPropertyMapper : PropertyMapperBase
    {
        private Func<object, object> _mapping;
        private Type _sourcePropertyType;

        /// <summary>
        /// Maps a basic property value.
        /// </summary>
        /// <param name="mapping">
        /// The mapping for the property value.  Cannot be null.
        /// </param>
        /// <param name="sourcePropertyType">
        /// The type of the first parameter being supplied to <paramref name="mapping"/>.
        /// Cannot be <c>null</c>.
        /// </param>
        /// <param name="sourcePropertyAlias">
        /// The alias of the node property to map from.  Required.
        /// </param>
        public BasicPropertyMapper(
            Func<object, object> mapping,
            Type sourcePropertyType,
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
            else if (sourcePropertyType == null)
            {
                throw new ArgumentNullException("sourcePropertyType");
            }
            else if (string.IsNullOrEmpty(sourcePropertyAlias))
            {
                throw new ArgumentException("Source property alias is required.");
            }

            RequiresInclude = false;
            AllowCaching = true;
            _mapping = mapping;
            _sourcePropertyType = sourcePropertyType;
        }

        public override object MapProperty(NodeMappingContext context)
        {
            object value = null;

            // Check cache
            if (AllowCaching
                && Engine.CacheProvider != null 
                && Engine.CacheProvider.ContainsPropertyValue(context.Id, DestinationInfo.Name))
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

                var sourceValue = GetSourcePropertyValue(node, _sourcePropertyType);

                value = _mapping(sourceValue);

                if (AllowCaching
                    && Engine.CacheProvider != null)
                {
                    Engine.CacheProvider.InsertPropertyValue(context.Id, DestinationInfo.Name, value);
                }
            }

            return value;
        }
    }
}
