using System;
using umbraco.NodeFactory;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Caching;
namespace uComponents.Mapping
{
    /// <summary>
    /// Handles the creation of map and the mapping of Umbraco <c>Node</c>s to strongly typed
    /// models.
    /// </summary>
    public interface INodeMappingEngine
    {
        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type
        /// using the unqualified class name of <typeparamref name="TDestination"/> 
        /// as the document type alias.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <returns>Further mapping configuration</returns>
        INodeMappingExpression<TDestination> CreateMap<TDestination>() 
            where TDestination : class, new();

        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <param name="documentTypeAlias">The document type alias to map from.</param>
        /// <returns>Further mapping configuration</returns>
        INodeMappingExpression<TDestination> CreateMap<TDestination>(string documentTypeAlias)
            where TDestination : class, new();

        /// <summary>
        /// Sets the cache provider for the engine to use. This will clear any existing
        /// cache provider.  Set as null to disable caching.
        /// </summary>
        void SetCacheProvider(ICacheProvider cacheProvider);

        /// <summary>
        /// Gets an Umbraco <c>Node</c> as a <typeparamref name="TDestination"/>, only including 
        /// specified relationship paths.
        /// </summary>
        /// <typeparam name="TDestination">
        /// The type of object that <paramref name="sourceNode"/> maps to.
        /// </typeparam>
        /// <param name="sourceNode">The <c>Node</c> to map from.</param>
        /// <param name="paths">The relationship paths to include.</param>
        /// <returns><c>null</c> if the node does not exist.</returns>
        TDestination Map<TDestination>(Node sourceNode, params string[] paths)
            where TDestination : class, new();

        /// <summary>
        /// Loads a path for a model.  If the path is already loaded, it will
        /// be overwritten.
        /// </summary>
        /// <param name="model">The model to load the path for.</param>
        /// <param name="path">
        /// The path to load, e.g. "Customers.Dinners".
        /// 
        /// TODO: Add overload with expression syntax.
        /// </param>
        void LoadPath<TDestination>(TDestination model, string path)
            where TDestination : class, new();

        /// <summary>
        /// Gets a query for nodes which map to <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <returns>A fluent configuration for the query.</returns>
        INodeQuery<TDestination> Query<TDestination>()
            where TDestination : class, new();

        #region Legacy

        [Obsolete("Use paths instead")]
        TDestination Map<TDestination>(Node sourceNode, bool includeRelationships)
            where TDestination : class, new();

        [Obsolete("Use paths instead")]
        TDestination Map<TDestination>(Node sourceNode, params Expression<Func<TDestination, object>>[] includedRelationships)
            where TDestination : class, new();

        #endregion
    }
}
