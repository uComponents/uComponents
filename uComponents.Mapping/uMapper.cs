using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.NodeFactory;

namespace uComponents.Mapping
{
    /// <summary>
    /// Maps Umbraco nodes to strongly typed models.
    /// </summary>
    public static class uMapper
    {
        private static INodeMappingEngine _engine = new NodeMappingEngine();

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
        /// Removes a map which maps to TDestination, if any exists.
        /// </summary>
        /// <typeparam name="TDestination">The model being mapped to</typeparam>
        public static void RemoveMap<TDestination>()
        {
            _engine.RemoveMap<TDestination>();
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
        public static TDestination Map<TDestination>(Node sourceNode, bool includeRelationships = false)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(sourceNode, includeRelationships);
        }

        /// <summary>
        /// Gets an Umbraco node as a strongly typed object.
        /// </summary>
        /// <typeparam name="TDestination">The type of object that the node maps to.</typeparam>
        /// <param name="id">The id of the node</param>
        /// <returns>Null if the node does not exist.</returns>
        /// <exception cref="MapNotFoundException">If a suitable map for TDestination has not 
        /// been created with CreateMap</exception>
        public static TDestination Get<TDestination>(int id, bool includeRelationships = false)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(new Node(id), includeRelationships);
        }
    }
}
