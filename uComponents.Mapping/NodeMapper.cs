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
    /// <summary>
    /// Mapper which maps from Umbraco Node properties to strongly typed model properties
    /// </summary>
    internal class NodeMapper : INodeMapper
    {
        public NodeMappingEngine Engine { get; private set; }
        public Type DestinationType { get; private set; }
        public List<NodePropertyMapper> PropertyMappers { get; private set; }
        public string SourceNodeTypeAlias { get; private set; }

        public NodeMapper(NodeMappingEngine engine, Type destinationType, string sourceNodeTypeAlias)
        {
            if (string.IsNullOrEmpty(sourceNodeTypeAlias))
            {
                throw new ArgumentException("Source node type alias must be specified");
            }
            else if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }
            else if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            SourceNodeTypeAlias = sourceNodeTypeAlias;
            Engine = engine;
            DestinationType = destinationType;
            PropertyMappers = new List<NodePropertyMapper>();
        }

        /// <summary>
        /// Maps a Node to a strongly typed model
        /// </summary>
        /// <param name="sourceNode">The node to map from</param>
        /// <param name="includeRelationships">
        /// Whether to include relationships with other nodes
        /// </param>
        /// <returns>The strongly typed model</returns>
        public object MapNode(Node sourceNode, bool includeRelationships)
        {
            var destination = Activator.CreateInstance(DestinationType);

            foreach (var propertyMapper in PropertyMappers)
            {
                if (includeRelationships || !propertyMapper.IsRelationship)
                {
                    var destinationValue = propertyMapper.MapProperty(sourceNode);
                    propertyMapper.DestinationInfo.SetValue(destination, destinationValue, null);
                }
            }

            return destination;
        }
    }

    internal class NodeMapper<TDestination> : NodeMapper, INodeMapper<TDestination>
    {
        public NodeMapper(NodeMappingEngine engine, string sourceNodeTypeAlias)
            : base(engine, typeof(TDestination), sourceNodeTypeAlias)
        {
        }

        /// <summary>
        /// Maps a Node to an instance of TDestination
        /// </summary>
        /// <param name="sourceNode">The node to map from</param>
        /// <param name="includeRelationships">
        /// Whether to include relationships with other nodes
        /// </param>
        public new TDestination MapNode(Node sourceNode, bool includeRelationships)
        {
            return (TDestination)base.MapNode(sourceNode, includeRelationships);
        }
    }
}
