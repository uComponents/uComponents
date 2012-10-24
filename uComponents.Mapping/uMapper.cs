using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.NodeFactory;
using umbraco;
using System.Linq.Expressions;

namespace uComponents.Mapping
{
    /// <summary>
    /// Maps Umbraco nodes to strongly typed models.
    /// </summary>
    public static class uMapper
    {
        private static NodeMappingEngine _engine = new NodeMappingEngine();

        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type, 
        /// using unqualified destination class name as the document type alias.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <returns>Further mapping configuration</returns>
        /// <exception cref="DocumentTypeNotFoundException">If the source document type could not be found</exception>
        public static INodeMappingExpression<TDestination> CreateMap<TDestination>()
            where TDestination : class, new()
        {
            return _engine.CreateMap<TDestination>();
        }

        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <param name="documentTypeAlias">The alias of the document type to map from.</param>
        /// <returns>Further mapping configuration</returns>
        /// <exception cref="DocumentTypeNotFoundException">If the source document type could not be found</exception>
        public static INodeMappingExpression<TDestination> CreateMap<TDestination>(string documentTypeAlias)
            where TDestination : class, new()
        {
            return _engine.CreateMap<TDestination>(documentTypeAlias);
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
        public static TDestination Map<TDestination>(Node sourceNode, bool includeRelationships = true)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(sourceNode, includeRelationships);
        }

        /// <summary>
        /// Gets an Umbraco node as TDestination.
        /// </summary>
        /// <typeparam name="TDestination">The type of object that the node maps to.</typeparam>
        /// <param name="id">The id of the node</param>
        /// <returns>Null if the node does not exist.</returns>
        /// <exception cref="MapNotFoundException">If a suitable map for TDestination has not 
        /// been created with CreateMap</exception>
        public static TDestination GetSingle<TDestination>(int id, bool includeRelationships = true)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(new Node(id), includeRelationships);
        }

        /// <summary>
        /// Gets the current Umbraco node as TDestination.
        /// </summary>
        /// <typeparam name="TDestination">
        /// The type of object that the current Node maps to.
        /// </typeparam>
        /// <param name="includeRelationships">Whether to load the node's relationships</param>
        /// <returns>Null if there is no current node.</returns>
        public static TDestination GetCurrent<TDestination>(bool includeRelationships = true)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(Node.GetCurrent(), includeRelationships);
        }

        /// <summary>
        /// Gets all Umbraco nodes which map to TDestination.
        /// </summary>
        /// <typeparam name="TDestination">The destination model type.</typeparam>
        /// <param name="includeRelationships">Whether to include all relationships.</param>
        /// <returns>The collection of mapped models.</returns>
        public static IEnumerable<TDestination> GetAll<TDestination>(bool includeRelationships = true)
            where TDestination : class, new()
        {
            var destinationType = typeof(TDestination);

            if (!_engine.NodeMappers.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            var sourceNodeTypeAlias = _engine.NodeMappers[destinationType].SourceNodeTypeAlias;

            return uQuery.GetNodesByType(sourceNodeTypeAlias)
                .Select(n => _engine.Map<TDestination>(n, includeRelationships));
        }

        /// <summary>
        /// Gets all Umbraco nodes which map to TDestination, only including specified relationships.
        /// </summary>
        /// <typeparam name="TDestination">The type of object that the nodes map to.</typeparam>
        /// <param name="includedRelationships">The relationship properties to include.</param>
        /// <returns>Null if the node does not exist.</returns>
        /// <exception cref="MapNotFoundException">If a suitable map for TDestination has not 
        /// been created with CreateMap</exception>
        public static IEnumerable<TDestination> GetAll<TDestination>(params Expression<Func<TDestination, object>>[] includedRelationships)
            where TDestination : class, new()
        {
            var destinationType = typeof(TDestination);

            if (!_engine.NodeMappers.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            var sourceNodeTypeAlias = _engine.NodeMappers[destinationType].SourceNodeTypeAlias;

            return uQuery.GetNodesByType(sourceNodeTypeAlias)
                .Select(n => _engine.Map<TDestination>(n, includedRelationships));
        }
    }
}
