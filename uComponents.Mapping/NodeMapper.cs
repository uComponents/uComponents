using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using umbraco.NodeFactory;
using umbraco;
using umbraco.cms.businesslogic.web;
using uComponents.Mapping.Property;

namespace uComponents.Mapping
{
    /// <summary>
    /// Mapper which maps from Umbraco Node properties to strongly typed model properties
    /// </summary>
    internal class NodeMapper
    {
        public NodeMappingEngine Engine { get; private set; }
        public Type DestinationType { get; private set; }
        public List<PropertyMapperBase> PropertyMappers { get; private set; }
        public string SourceNodeTypeAlias { get; private set; }

        public NodeMapper(NodeMappingEngine engine, Type destinationType, DocumentType sourceDocumentType)
        {
            if (sourceDocumentType == null)
            {
                throw new ArgumentNullException("sourceDocumentType");
            }
            else if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }
            else if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            SourceNodeTypeAlias = sourceDocumentType.Alias;
            Engine = engine;
            DestinationType = destinationType;
            PropertyMappers = new List<PropertyMapperBase>();

            // See if base properties have been mapped already.
            var baseNodeMapper = Engine.GetBaseNodeMapperForType(destinationType);
            if (baseNodeMapper != null)
            {
                // Use the property mappings of the closest parent
                // (ideally they will already have the mappings of all their ancestors)
                PropertyMappers.AddRange(baseNodeMapper.PropertyMappers);
            }

            // Map properties
            foreach (var destinationProperty in destinationType.GetProperties())
            {
                if (PropertyMappers.Any(mapper => mapper.DestinationInfo.Name == destinationProperty.Name))
                {
                    // A mapping already exists for this property on a base type.
                    continue;
                }

                PropertyMapperBase propertyMapper = null;

                var sourcePropertyAlias = sourceDocumentType.PropertyTypes
                    .Select(prop => prop.Alias)
                    .Where(alias => string.Equals(alias, destinationProperty.Name, StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefault();

                // Check if this property aligns with an default node properties
                var defaultPropertyMapping = DefaultPropertyMapper.GetDefaultMappingForName(destinationProperty.Name);
                if (defaultPropertyMapping != null)
                {
                    propertyMapper = new DefaultPropertyMapper(defaultPropertyMapping, this, destinationProperty);
                }
                else if (sourcePropertyAlias != null
                    && (destinationProperty.PropertyType.IsSystem()
                          || destinationProperty.PropertyType.IsEnum)) // check corresponding source property alias was found
                {
                    propertyMapper = new BasicPropertyMapper(
                        x => x, // direct mapping
                        destinationProperty.PropertyType, // using the desired type
                        this,
                        destinationProperty,
                        sourcePropertyAlias
                        );
                }
                else if (destinationProperty.PropertyType.IsModel())
                {
                    propertyMapper = new SinglePropertyMapper(
                        null,
                        null,
                        this,
                        destinationProperty,
                        sourcePropertyAlias // can be null to map ancestor
                        );
                }
                else if (destinationProperty.PropertyType.IsModelCollection())
                {
                    propertyMapper = new CollectionPropertyMapper(
                        null,
                        null,
                        this,
                        destinationProperty,
                        sourcePropertyAlias // can be null to map descendants
                        );
                }

                if (propertyMapper != null)
                {
                    PropertyMappers.Add(propertyMapper);
                }
            }
        }

        /// <summary>
        /// Maps a Node to a strongly typed model, including only the specified 
        /// relationship paths.
        /// </summary>
        /// <param name="sourceNode">The node to map from</param>
        /// <param name="paths">
        /// An array of relationship paths on the model to include, or null
        /// to include all relationships at the first level and none below.
        /// </param>
        [Obsolete("Use MapNode with NodeMappingContext instead")]
        public object MapNode(Node sourceNode, string[] paths)
        {
            var context = new NodeMappingContext(sourceNode, paths, null);

            return MapNode(context);
        }

        public object MapNode(NodeMappingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            object destination = Activator.CreateInstance(DestinationType);

            PropertyInfo[] includedPaths = null;

            // Check included paths are actually required
            if (context.Paths != null)
            {
                includedPaths = GetImmediateProperties(DestinationType, context.Paths.ToArray());

                foreach (var path in includedPaths)
                {
                    var propertyMapper = PropertyMappers.SingleOrDefault(x => x.DestinationInfo.Name == path.Name);

                    if (propertyMapper == null)
                    {
                        throw new InvalidPathException(
                            string.Format(
                                "The property '{0}' on '{1}' is not mapped - check your mappings.",
                                path.Name,
                                path.PropertyType.FullName
                                ));
                    }
                    else if (!propertyMapper.RequiresInclude)
                    {
                        throw new InvalidPathException(
                            string.Format(
                                @"The property '{0}' on '{1}' does not 
require an explicit include (do not include it as a path, it will be populated automatically).",
                                path.Name,
                                path.PropertyType.FullName
                                ));
                    }
                }
            }

            foreach (var propertyMapper in PropertyMappers)
            {
                if (context.Paths == null // include all paths
                    || !propertyMapper.RequiresInclude  // map all automatic paths
                    || includedPaths.Any(x => x.Name == propertyMapper.DestinationInfo.Name)) // map explicit paths
                {
                    var destinationValue = propertyMapper.MapProperty(context);
                    propertyMapper.DestinationInfo.SetValue(destination, destinationValue, null);
                }
            }

            return destination;
        }

        /// <summary>
        /// Maps a Node to a strongly typed model, excluding relationships except those specified.
        /// </summary>
        /// <param name="sourceNode">The node to map from</param>
        /// <param name="includedRelationships">An array of properties on the model which
        /// relationships should be mapped to, or null to map all properties.</param>
        [Obsolete("Use MapNode with paths instead")]
        public object MapNode(Node sourceNode, PropertyInfo[] includedRelationships)
        {
            return MapNode(sourceNode, includedRelationships.Select(x => x.Name).ToArray());
        }

        /// <summary>
        /// Gets the properties of <paramref name="type"/> which are defined
        /// by <paramref name="paths"/>.
        /// </summary>
        private static PropertyInfo[] GetImmediateProperties(Type type, string[] paths)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }

            var allProperties = type.GetProperties();
            var chosenProperties = new List<PropertyInfo>();

            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentException("No path can be empty or null.", "paths");
                }

                var segment = path.Split('.').First();

                if (!chosenProperties.Any(p => p.Name == segment))
                {
                    var property = allProperties.SingleOrDefault(p => p.Name == segment);

                    if (property != null)
                    {
                        chosenProperties.Add(property);
                    }
                    else
                    {
                        throw new InvalidPathException(type, path, segment);
                    }
                }
            }

            return chosenProperties.ToArray();
        }
    }

    /// <summary>
    /// Thrown when a path does not match up with the model graph.
    /// </summary>
    public class InvalidPathException : Exception
    {
        /// <summary>
        /// Basic constructor for path exceptions
        /// </summary>
        public InvalidPathException(string message)
            : base(message)
        {
        }

        /// <param name="type">The type where the segment was expected to correspond
        /// to a relationship.</param>
        /// <param name="path">The remaining path.</param>
        /// <param name="segment">The segment of the path which could to be matched up
        /// to the type.</param>
        public InvalidPathException(Type type, string path, string segment)
            : base(string.Format(
@"The segment '{0}' of the remaining path '{1}' does not refer to a valid 
relationship on '{2}'",
                    segment,
                    path,
                    type))
        {
        }
    }
}
