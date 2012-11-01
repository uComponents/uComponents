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
using System.Linq.Expressions;

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

        /// <summary>
        /// Instantiates a new NodeMappingEngine
        /// </summary>
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

            var nodeMapper = new NodeMapper(this, destinationType, docType);

            NodeMappers[destinationType] = nodeMapper;

            return new NodeMappingExpression<TDestination>(this, nodeMapper);
        }

        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
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
        /// <param name="sourceNode">The node to map from.</param>
        /// <param name="destinationType">The type to map to.</param>
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
            else if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            else if (!NodeMappers.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            CheckMapping(sourceNode.NodeTypeAlias, destinationType);

            var nodeMapper = NodeMappers[destinationType];

            if (includeRelationships)
            {
                return nodeMapper.MapNode(sourceNode);
            }
            else
            {
                // Don't include any relationships
                return nodeMapper.MapNode(sourceNode, new PropertyInfo[0]);
            }
        }

        /// <summary>
        /// Gets an Umbraco node as a strongly typed object, only including specified relationships.
        /// </summary>
        /// <param name="sourceNode">The node to map from.</param>
        /// <param name="destinationType">The type to map to.</param>
        /// <param name="includedRelationships">The relationship properties to include.</param>
        /// <returns>Null if the node does not exist.</returns>
        /// <exception cref="MapNotFoundException">If a suitable map for destinationType has not 
        /// been created with CreateMap</exception>
        /// <exception cref="ArgumentNullException">If includedRelationships is null</exception>
        public object Map(Node sourceNode, Type destinationType, PropertyInfo[] includedRelationships)
        {
            if (sourceNode == null
                || string.IsNullOrEmpty(sourceNode.Name))
            {
                return null;
            }
            else if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            else if (includedRelationships == null)
            {
                throw new ArgumentNullException("includedRelationships");
            }
            else if (!NodeMappers.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            CheckMapping(sourceNode.NodeTypeAlias, destinationType);

            // Check 'includedRelationships' actually refer to relationships
            var propertyMappers = NodeMappers[destinationType].PropertyMappers;
            foreach (var relationshipInfo in includedRelationships)
            {
                var propertyMapper = propertyMappers.SingleOrDefault(x => x.DestinationInfo == relationshipInfo);

                if (propertyMapper == null)
                {
                    throw new ArgumentException(
                        string.Format(@"The property '{0}' specified by 'includedRelationships' does 
not have a valid map", relationshipInfo.Name), "includedRelationships"
                     );
                }
                else if (!propertyMapper.IsRelationship)
                {
                    throw new ArgumentException(@"One of the properties on 'destinationType' does not 
refer to a relationship.", "includedRelationships");
                }
            }

            var nodeMapper = NodeMappers[destinationType];

            return nodeMapper.MapNode(sourceNode, includedRelationships);
        }

        /// <summary>
        /// Gets an Umbraco node as a TDestination.
        /// </summary>
        /// <typeparam name="TDestination">The type of object that the node maps to.</typeparam>
        /// <param name="sourceNode">The node to map from.</param>
        /// <param name="includeRelationships">Whether to include the node's relationships</param>
        /// <returns>Null if the node does not exist.</returns>
        /// <exception cref="MapNotFoundException">If a suitable map for TDestination has not 
        /// been created with CreateMap</exception>
        public TDestination Map<TDestination>(Node sourceNode, bool includeRelationships)
            where TDestination : class, new()
        {
            return (TDestination)Map(sourceNode, typeof(TDestination), includeRelationships);
        }

        /// <summary>
        /// Gets an Umbraco node as a TDestination, only including specified relationships.
        /// </summary>
        /// <typeparam name="TDestination">The type of object that the node maps to.</typeparam>
        /// <param name="sourceNode">The node to map from.</param>
        /// <param name="includedRelationships">The relationship properties to include.</param>
        /// <returns>Null if the node does not exist.</returns>
        /// <exception cref="MapNotFoundException">If a suitable map for TDestination has not 
        /// been created with CreateMap</exception>
        public TDestination Map<TDestination>(Node sourceNode, params Expression<Func<TDestination, object>>[] includedRelationships)
            where TDestination : class, new()
        {
            // Get properties from included relationships expression
            var properties = includedRelationships.Select(e => (e.Body as MemberExpression).Member as PropertyInfo).ToArray();

            return (TDestination)Map(sourceNode, typeof(TDestination), properties);
        }

        /// <summary>
        /// Examines the engine's <see cref="NodeMappers"/> and returns node mapper
        /// which maps to the closest base class of <paramref name="type"/>.
        /// </summary>
        /// <returns>
        /// <c>null</c>  if there are no mappers which map to a base class of 
        /// <paramref name="type"/>.
        /// </returns>
        internal NodeMapper GetParentNodeMapperForType(Type type)
        {
            var ancestorMappers = new List<NodeMapper>();

            foreach (var nodeMapper in NodeMappers)
            {
                if (nodeMapper.Value.DestinationType.IsAssignableFrom(type)
                    && type != nodeMapper.Value.DestinationType)
                {
                    ancestorMappers.Add(nodeMapper.Value);
                }
            }

            // Sort by inheritance
            ancestorMappers.Sort((x, y) => 
                {
                    return x.DestinationType.IsAssignableFrom(y.DestinationType)
                        ? 1
                        : -1;
                });

            return ancestorMappers.FirstOrDefault();
        }

        /// <summary>
        /// Checks that there is a valid mapping from <paramref name="sourceNodeTypeAlias"/>
        /// to <paramref name="destinationType"/>.
        /// </summary>
        /// <exception cref="WrongNodeForMapException">
        /// If no mapping exists from <paramref name="sourceNodeTypeAlias"/> to 
        /// <paramref name="destinationType"/> or any class derived from 
        /// <paramref name="destinationType"/>.
        /// </exception>
        private void CheckMapping(string sourceNodeTypeAlias, Type destinationType)
        {
            var compatibleMappingFound = false;
            foreach (var nodeMapper in NodeMappers)
            {
                if (destinationType.IsAssignableFrom(nodeMapper.Value.DestinationType)
                    && string.Equals(nodeMapper.Value.SourceNodeTypeAlias, sourceNodeTypeAlias, StringComparison.InvariantCultureIgnoreCase))
                {
                    compatibleMappingFound = true;
                    break;
                }
            }

            if (!compatibleMappingFound)
            {
                throw new WrongNodeForMapException(sourceNodeTypeAlias, destinationType);
            }
        }
    }

    #region Exceptions

    /// <summary>
    /// The node type alias was not found in the current Umbraco instance.
    /// </summary>
    public class DocumentTypeNotFoundException : Exception
    {
        /// <param name="nodeTypeAlias">The node type alias requested and not found</param>
        public DocumentTypeNotFoundException(string nodeTypeAlias)
            : base(string.Format(@"The document type with alias '{0}' could not be found.  
Consider using the overload of CreateMap which specifies a document type alias", nodeTypeAlias))
        {
        }
    }

    /// <summary>
    /// No map exists for this engine for the destination type
    /// </summary>
    public class MapNotFoundException : Exception
    {
        /// <param name="destinationType">The requested and unfound destination type</param>
        public MapNotFoundException(Type destinationType)
            : base(string.Format(@"No map could be found for type '{0}'.  Remember
to run CreateMap for every model type you are using.", destinationType.FullName))
        {
        }
    }

    /// <summary>
    /// Exception for when a node is mapped to an incompatible type.
    /// </summary>
    public class WrongNodeForMapException : Exception
    {
        /// <param name="nodeTypeAlias">The node type alias being mapped from.</param>
        /// <param name="destinationType">The destination type being mapped to.</param>
        public WrongNodeForMapException(string nodeTypeAlias, Type destinationType)
            : base(string.Format(@"Node with node type alias '{0}' does not map to
model type '{1}'.  Make sure you are mapping the correct node.",
            nodeTypeAlias,
            destinationType.FullName
            ))
        {
        }
    }

    /// <summary>
    /// A collection which cannot be instiatated/populated by the mapping engine
    /// is used.
    /// </summary>
    public class CollectionTypeNotSupported : Exception
    {
        /// <param name="type">The unsupported collection type.</param>
        public CollectionTypeNotSupported(Type type)
            : base(string.Format(@"Could not map to collection of type '{0}'.  
Use IEnumerable, or alternatively make sure your collection type has
a single parameter constructor which takes an IEnumerable (such as List)", type.FullName))
        {
        }
    }

    #endregion
}
