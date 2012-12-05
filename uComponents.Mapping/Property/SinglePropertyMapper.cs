using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using umbraco;

namespace uComponents.Mapping.Property
{
    internal class SinglePropertyMapper : PropertyMapperBase
    {
        private Func<object, int?> _mapping;
        private Type _sourcePropertyType;

        /// <summary>
        /// Maps a single relationship.
        /// </summary>
        /// <param name="mapping">
        /// Mapping from <paramref name="sourcePropertyType"/> to a nullable node ID.  
        /// If <c>null</c>, the mapping will be deduced from the other parameters.
        /// </param>
        /// <param name="sourcePropertyType">
        /// The type of the first parameter being supplied to <paramref name="mapping"/>.
        /// Cannot be <c>null</c> if <paramref name="mapping"/> is not <c>null</c>.
        /// </param>
        /// <param name="sourcePropertyAlias">
        /// The alias of the node property to map from.  If null, the closest ancestor which is 
        /// compatible with <paramref name="destinationProperty"/> will be mapped instead.
        /// </param>
        public SinglePropertyMapper(
            Func<object, int?> mapping,
            Type sourcePropertyType,
            NodeMapper nodeMapper,
            PropertyInfo destinationProperty,
            string sourcePropertyAlias
            )
            : base(nodeMapper, destinationProperty, sourcePropertyAlias)
        {
            RequiresInclude = true;
            AllowCaching = true;
            _mapping = mapping;
            _sourcePropertyType = sourcePropertyType;
        }

        public override object MapProperty(NodeMappingContext context)
        {
            int? id = null;

            // Get ID
            if (AllowCaching
                && Engine.CacheProvider != null
                && Engine.CacheProvider.ContainsPropertyValue(context.Id, DestinationInfo.Name))
            {
                id = Engine.CacheProvider.GetPropertyValue(context.Id, DestinationInfo.Name) as int?;
            }
            else
            {
                var node = context.GetNode();

                if (node == null || string.IsNullOrEmpty(node.Name))
                {
                    throw new InvalidOperationException("Node cannot be null or empty");
                }

                if (_mapping != null)
                {
                    // Custom mapping
                    id = _mapping(GetSourcePropertyValue(node, _sourcePropertyType));
                }
                else if (!string.IsNullOrEmpty(SourcePropertyAlias))
                {
                    // Map ID from node property
                    id = GetSourcePropertyValue<int?>(node);
                }
                else
                {
                    // Get closest parent
                    var aliases = Engine
                        .GetCompatibleNodeTypeAliases(DestinationInfo.PropertyType)
                        .ToArray();
                    var ancestorNode = node.GetAncestorNodes()
                        .FirstOrDefault(x => aliases.Contains(x.NodeTypeAlias));

                    if (ancestorNode != null)
                    {
                        // Found one
                        id = ancestorNode.Id;

                        context.AddNodeToContextCache(ancestorNode);
                    }
                }

                if (AllowCaching
                    && Engine.CacheProvider != null)
                {
                    Engine.CacheProvider.InsertPropertyValue(context.Id, DestinationInfo.Name, id);
                }
            }

            if (!id.HasValue)
            {
                // Not found
                return null;
            }

            // Map to model
            var childPaths = GetNextLevelPaths(context.Paths.ToArray());
            var childContext = new NodeMappingContext(id.Value, childPaths, context);

            return Engine.Map(childContext, DestinationInfo.PropertyType);
        }
    }
}
