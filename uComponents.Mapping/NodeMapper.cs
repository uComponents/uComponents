using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using umbraco.NodeFactory;
using umbraco;
using umbraco.cms.businesslogic.web;

namespace uComponents.Mapping
{
    /// <summary>
    /// Mapper which maps from Umbraco Node properties to strongly typed model properties
    /// </summary>
    internal class NodeMapper
    {
        public NodeMappingEngine Engine { get; private set; }
        public Type DestinationType { get; private set; }
        public List<NodePropertyMapper> PropertyMappers { get; private set; }
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
            PropertyMappers = new List<NodePropertyMapper>();

            // See if base properties have been mapped already.
            var baseNodeMapper = Engine.GetParentNodeMapperForType(destinationType);
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

                NodePropertyMapper customPropertyMapper = null;
                Func<Node, string[], object> defaultPropertyMapping = null;

                // Default node properties
                switch (destinationProperty.Name.ToLowerInvariant())
                {
                    case "createdate":
                        defaultPropertyMapping = (node, path) => node.CreateDate;
                        break;
                    case "creatorid":
                        defaultPropertyMapping = (node, path) => node.CreatorID;
                        break;
                    case "creatorname":
                        defaultPropertyMapping = (node, path) => node.CreatorName;
                        break;
                    case "id":
                        defaultPropertyMapping = (node, path) => node.Id;
                        break;
                    case "level":
                        defaultPropertyMapping = (node, path) => node.Level;
                        break;
                    case "name":
                        defaultPropertyMapping = (node, path) => node.Name;
                        break;
                    case "niceurl":
                        defaultPropertyMapping = (node, path) => node.NiceUrl;
                        break;
                    case "nodetypealias":
                        defaultPropertyMapping = (node, path) => node.NodeTypeAlias;
                        break;
                    case "path":
                        defaultPropertyMapping = (node, path) => node.Path;
                        break;
                    case "sortorder":
                        defaultPropertyMapping = (node, path) => node.SortOrder;
                        break;
                    case "template":
                        defaultPropertyMapping = (node, path) => node.template;
                        break;
                    case "updatedate":
                        defaultPropertyMapping = (node, path) => node.UpdateDate;
                        break;
                    case "url":
                        defaultPropertyMapping = (node, path) => node.Url;
                        break;
                    case "urlname":
                        defaultPropertyMapping = (node, path) => node.UrlName;
                        break;
                    case "version":
                        defaultPropertyMapping = (node, path) => node.Version;
                        break;
                    case "writerid":
                        defaultPropertyMapping = (node, path) => node.WriterID;
                        break;
                    case "writername":
                        defaultPropertyMapping = (node, path) => node.WriterName;
                        break;
                    default:
                        // Map custom properties
                        var sourcePropertyAlias = sourceDocumentType.PropertyTypes
                            .Select(prop => prop.Alias)
                            .Where(alias => string.Equals(alias, destinationProperty.Name, StringComparison.InvariantCultureIgnoreCase))
                            .SingleOrDefault();

                        if (sourcePropertyAlias != null
                            || destinationProperty.PropertyType.IsModelCollection()
                            || destinationProperty.PropertyType.IsModel()
                            || destinationProperty.PropertyType.IsEnum)
                        {
                            customPropertyMapper = new NodePropertyMapper(this, destinationProperty, sourcePropertyAlias);
                        }
                        break;
                }

                if (customPropertyMapper != null)
                {
                    PropertyMappers.Add(customPropertyMapper);
                }
                else if (defaultPropertyMapping != null)
                {
                    var defaultNodePropertyMapper = new NodePropertyMapper(this, destinationProperty, defaultPropertyMapping, false);
                    PropertyMappers.Add(defaultNodePropertyMapper);
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
        public object MapNode(Node sourceNode, string[] paths)
        {
            object destination = null;

            // Check for cached model
            if (Engine.IsCachingEnabled)
            {
                var cachedModel = Engine.CacheProvider.Get(sourceNode.Id.ToString());

                if (cachedModel != null)
                {
                    // TODO copy to destination
                }
            }

            if (destination == null)
            {
                destination = Activator.CreateInstance(DestinationType);

                // Map non-relationships
                foreach (var propertyMapper in PropertyMappers.Where(x => !x.RequiresInclude))
                {
                    var destinationValue = propertyMapper.MapProperty(sourceNode, new string[0]);
                    propertyMapper.DestinationInfo.SetValue(destination, destinationValue, null);
                }

                // TODO copy to cache...

                MapPaths(destination, paths);

                return destination;
            }

//            PropertyInfo[] includedRelationships = null;

//            if (paths != null)
//            {
//                includedRelationships = GetImmediateProperties(DestinationType, paths);

//                // Check relationships actually refer to relationships
//                foreach (var relationshipInfo in includedRelationships)
//                {
//                    var propertyMapper = PropertyMappers.SingleOrDefault(x => x.DestinationInfo.Name == relationshipInfo.Name);

//                    if (propertyMapper == null)
//                    {
//                        throw new InvalidPathException(
//                            string.Format(
//                                "The property '{0}' on '{1}' is not mapped - check your mappings.", 
//                                relationshipInfo.Name,
//                                relationshipInfo.PropertyType.FullName
//                                ));
//                    }
//                    else if (!propertyMapper.IsRelationship)
//                    {
//                        throw new InvalidPathException(
//                            string.Format(
//                                @"The property '{0}' on '{1}' does not 
//refer to a relationship (do not include it as a path, it will be populated automatically).",
//                                relationshipInfo.Name,
//                                relationshipInfo.PropertyType.FullName
//                                ));
//                    }
//                }
//            }

//            if (Engine.IsCachingEnabled)
//            {
//                var cache = Engine.CacheProvider;

//                // TODO
//                throw new NotImplementedException();
//            }

//            foreach (var propertyMapper in PropertyMappers)
//            {
//                if (paths == null // include all relationships
//                    || !propertyMapper.IsRelationship  // map all non-relationship properties
//                    || includedRelationships.Any(x => x.Name == propertyMapper.DestinationInfo.Name)) // check this relationship is included
//                {
//                    var pathsForProperty = GetNextLevelPaths(propertyMapper.DestinationInfo.Name, paths);
//                    var destinationValue = propertyMapper.MapProperty(sourceNode, pathsForProperty);
//                    propertyMapper.DestinationInfo.SetValue(destination, destinationValue, null);
//                }
//            }

            return destination;
        }

        /// <summary>
        /// Populates a model with <paramref name="paths"/>.
        /// </summary>
        /// <param name="model">A model of type <see cref="DestinationType"/></param>
        /// <param name="paths">An array of relationship paths to populate.</param>
        private void MapPaths(object model, string[] paths)
        {
            // TODO Check for cached node ID of relationships
            throw new NotImplementedException();
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

        /// <summary>
        /// Gets the paths relative to a relationship.
        /// </summary>
        private static string[] GetNextLevelPaths(string relationshipName, string[] paths)
        {
            if (string.IsNullOrEmpty(relationshipName))
            {
                throw new ArgumentException("The property name must be specified", "propertyName");
            } 
            else if (paths == null)
            {
                // No paths
                return new string[0];
            }

            var pathsForProperty = new List<string>();

            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentException("No path can be null or empty", "paths");
                }

                var segments = path.Split('.');

                if (segments.First() == relationshipName
                    && segments.Length > 1)
                {
                    pathsForProperty.Add(string.Join(".", segments.Skip(1)));
                }
            }

            return pathsForProperty.ToArray();
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
