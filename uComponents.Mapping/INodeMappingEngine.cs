using System;
using umbraco.NodeFactory;
using System.Linq.Expressions;
using System.Reflection;
namespace uComponents.Mapping
{
    /// <summary>
    /// Handles the creation of map and the mapping of Umbraco nodes to strongly typed
    /// models.
    /// </summary>
    public interface INodeMappingEngine
    {
        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type, 
        /// using unqualified destination class name as the document type alias.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <returns>Further mapping configuration</returns>
        INodeMappingExpression<TDestination> CreateMap<TDestination>() 
            where TDestination : class, new();

        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <param name="documentTypeAlias">The alias of the document type to map from.</param>
        /// <returns>Further mapping configuration</returns>
        INodeMappingExpression<TDestination> CreateMap<TDestination>(string documentTypeAlias)
            where TDestination : class, new();

        /// <summary>
        /// Maps an Umbraco node to a strongly typed object.
        /// </summary>
        /// <param name="sourceNode">The node to map from.</param>
        /// <param name="destinationType">The type to map to.</param>
        /// <param name="includeRelationships">Whether to load relationships to other models.</param>
        /// <returns>A new instance of TDestination, or null if sourceNode is null or invalid.</returns>
        object Map(Node sourceNode, Type destinationType, bool includeRelationships);
        
        /// <summary>
        /// Maps an Umbraco node as a strongly typed object, only including specified relationships.
        /// </summary>
        /// <param name="sourceNode">The node to map from.</param>
        /// <param name="destinationType">The type to map to.</param>
        /// <param name="includedRelationships">The relationship properties to include.</param>
        /// <returns>Null if the node does not exist.</returns>
        object Map(Node sourceNode, Type destinationType, PropertyInfo[] includedRelationships);

        /// <summary>
        /// Maps an Umbraco node as TDestination.
        /// </summary>
        /// <typeparam name="TDestination">The type of object that the node maps to.</typeparam>
        /// <param name="sourceNode">The node to map from.</param>
        /// <param name="includeRelationships">Whether to include relationships for the node.</param>
        /// <returns>Null if sourceNode is null or invalid.</returns>
        TDestination Map<TDestination>(Node sourceNode, bool includeRelationships) 
            where TDestination : class, new();
        
        /// <summary>
        /// Maps an Umbraco node as TDestination, only including specified relationships.
        /// </summary>
        /// <typeparam name="TDestination">The type of object that the node maps to.</typeparam>
        /// <param name="sourceNode">The node to map from.</param>
        /// <param name="includedRelationships">The relationship properties to include.</param>
        /// <returns>Null if the node does not exist.</returns>
        TDestination Map<TDestination>(Node sourceNode, params Expression<Func<TDestination, object>>[] includedRelationships)
            where TDestination : class, new();
    }
}
