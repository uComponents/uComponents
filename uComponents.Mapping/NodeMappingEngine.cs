using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;

namespace uComponents.Mapping
{
    /// <summary>
    /// Handles the creation of map and the mapping of Umbraco nodes to strongly typed
    /// models.
    /// </summary>
    public class NodeMappingEngine : INodeMappingEngine
    {
        internal readonly static MethodInfo GetNodePropertyMethod = typeof(NodeExtensions).GetMethod("GetProperty");
        internal Dictionary<Type, NodeMapper> NodeMappers { get; set; }

        public NodeMappingEngine()
        {
            this.NodeMappers = new Dictionary<Type, NodeMapper>();
        }

        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type, 
        /// using unqualified destination class name as the document type alias.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <returns>Further mapping configuration</returns>
        /// <exception cref="DocumentTypeNotFoundException">If the source document type could not be found</exception>
        public INodeMappingExpression<TDestination> CreateMap<TDestination>(string documentTypeAlias)
            where TDestination : class, new()
        {
            var destinationType = typeof(TDestination);

            // Remove current mapping if any
            if (NodeMappers.ContainsKey(destinationType))
            {
                NodeMappers.Remove(destinationType);
            }

            // Get document type
            var docType = DocumentType.GetByAlias(documentTypeAlias);

            if (docType == null)
            {
                throw new DocumentTypeNotFoundException(documentTypeAlias);
            }

            var nodeMapper = new NodeMapper<TDestination>(this, documentTypeAlias);

            foreach (var destinationProperty in destinationType.GetProperties())
            {
                NodePropertyMapper customPropertyMapper = null;
                Func<Node, object> defaultPropertyMapping = null;

                // Default node properties
                switch (destinationProperty.Name.ToLowerInvariant())
                {
                    case "createdate":
                        defaultPropertyMapping = node => node.CreateDate;
                        break;
                    case "creatorid":
                        defaultPropertyMapping = node => node.CreatorID;
                        break;
                    case "creatorname":
                        defaultPropertyMapping = node => node.CreatorName;
                        break;
                    case "id":
                        defaultPropertyMapping = node => node.Id;
                        break;
                    case "level":
                        defaultPropertyMapping = node => node.Level;
                        break;
                    case "name":
                        defaultPropertyMapping = node => node.Name;
                        break;
                    case "nodetypealias":
                        defaultPropertyMapping = node => node.NodeTypeAlias;
                        break;
                    case "path":
                        defaultPropertyMapping = node => node.Path;
                        break;
                    case "sortorder":
                        defaultPropertyMapping = node => node.SortOrder;
                        break;
                    case "template":
                        defaultPropertyMapping = node => node.template;
                        break;
                    case "updatedate":
                        defaultPropertyMapping = node => node.UpdateDate;
                        break;
                    case "url":
                        defaultPropertyMapping = node => node.Url;
                        break;
                    case "urlname":
                        defaultPropertyMapping = node => node.UrlName;
                        break;
                    case "version":
                        defaultPropertyMapping = node => node.Version;
                        break;
                    case "writerid":
                        defaultPropertyMapping = node => node.WriterID;
                        break;
                    case "writername":
                        defaultPropertyMapping = node => node.WriterName;
                        break;
                    default:
                        // Map custom properties
                        var sourcePropertyAlias = docType.PropertyTypes
                            .Select(prop => prop.Alias)
                            .Where(alias => string.Equals(alias, destinationProperty.Name, StringComparison.InvariantCultureIgnoreCase))
                            .SingleOrDefault();

                        if (sourcePropertyAlias != null
                            || destinationProperty.PropertyType.IsModelCollection()
                            || destinationProperty.PropertyType.IsModel())
                        {
                            customPropertyMapper = new NodePropertyMapper(nodeMapper, destinationProperty, sourcePropertyAlias);
                        }
                        break;
                }

                if (customPropertyMapper != null)
                {
                    nodeMapper.PropertyMappers.Add(customPropertyMapper);
                }
                else if (defaultPropertyMapping != null)
                {
                    var defaultNodePropertyMapper = new NodePropertyMapper(nodeMapper, destinationProperty, defaultPropertyMapping, false);
                    nodeMapper.PropertyMappers.Add(defaultNodePropertyMapper);
                }
            }

            NodeMappers[destinationType] = nodeMapper;

            return new NodeMappingExpression<TDestination>(this, nodeMapper);
        }

        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <param name="documentTypeAlias">The alias of the document type to map from.</param>
        /// <returns>Further mapping configuration</returns>
        /// <exception cref="DocumentTypeNotFoundException">If the source document type could not be found</exception>
        public INodeMappingExpression<TDestination> CreateMap<TDestination>()
            where TDestination : class, new()
        {
            var destinationType = typeof(TDestination);

            return this.CreateMap<TDestination>(destinationType.Name);
        }

        /// <summary>
        /// Maps an Umbraco node to a strongly typed object.
        /// </summary>
        /// <typeparam name="TDestination">The type of object to map to.</typeparam>
        /// <param name="sourceNode">The node to map from.</param>
        /// <param name="includeRelationships">Whether to load relationships to other models.</param>
        /// <returns>A new instance of TDestination, or null if sourceNode is null.</returns>
        /// <exception cref="MapNotFoundException">If a suitable map for TDestination has not 
        /// been created with CreateMap</exception>
        public object Map(Node sourceNode, Type destinationType, bool includeRelationships)
        {
            if (sourceNode == null
                || string.IsNullOrEmpty(sourceNode.Name))
            {
                return null;
            }

            if (!NodeMappers.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            var nodeMapper = NodeMappers[destinationType];

            return nodeMapper.MapNode(sourceNode, includeRelationships);
        }

        /// <summary>
        /// Gets an Umbraco node as a strongly typed object.
        /// </summary>
        /// <typeparam name="TDestination">The type of object that the node maps to.</typeparam>
        /// <param name="id">The id of the node</param>
        /// <returns>Null if the node does not exist.</returns>
        /// <exception cref="MapNotFoundException">If a suitable map for TDestination has not 
        /// been created with CreateMap</exception>
        public TDestination Map<TDestination>(Node sourceNode, bool includeRelationships)
            where TDestination : class, new()
        {
            return (TDestination)Map(sourceNode, typeof(TDestination), includeRelationships);
        }
    }

    #region Exceptions

    public class DocumentTypeNotFoundException : Exception
    {
        public DocumentTypeNotFoundException(string documentTypeAlias)
            : base(string.Format(@"The document type with alias '{0}' could not be found.  
Consider using the overload of CreateMap which specifies a document type alias", documentTypeAlias))
        {
        }
    }

    public class MapNotFoundException : Exception
    {
        public MapNotFoundException(Type destinationType)
            : base(string.Format(@"No map could be found for type '{0}'.  Remember
to run CreateMap for every model type you are using.", destinationType.FullName))
        {
        }
    }

    public class CollectionTypeNotSupported : Exception
    {
        public CollectionTypeNotSupported(Type type)
            : base(string.Format(@"Could not map to collection of type '{0}'.  
Use IEnumerable, or alternatively make sure your collection type has
a single parameter constructor which takes an IEnumerable (such as List)", type.FullName))
        {
        }
    }

    #endregion
}
