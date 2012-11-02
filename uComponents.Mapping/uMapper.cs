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
    /// Maps Umbraco <c>Node</c>s to strongly typed models.
    /// </summary>
    /// <remarks>
    /// This is a static convenience class which wraps single instance of a <c>NodeMappingEngine</c>.
    /// See http://ucomponents.org/umapper/ for usage examples and pretty pictures.
    /// </remarks>
    public static class uMapper
    {
        private static NodeMappingEngine _engine = new NodeMappingEngine();

        /// <summary>
        /// Gets the <c>INodeMappingEngine</c> being used by uMapper.
        /// </summary>
        public static INodeMappingEngine Engine
        {
            get { return _engine; }
        }

        /// <summary>
        /// Creates a map to <typeparamref name="TDestination" /> from an Umbraco document type, 
        /// which must have the same alias as the unqualified class name of 
        /// <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <returns>Fluent configuration for the newly created mapping.</returns>
        /// <exception cref="DocumentTypeNotFoundException">If a suitable document type could not be found</exception>
        public static INodeMappingExpression<TDestination> CreateMap<TDestination>()
            where TDestination : class, new()
        {
            return _engine.CreateMap<TDestination>();
        }

        /// <summary>
        /// Creates a map to <typeparamref name="TDestination" /> from an Umbraco document type,
        /// specifying the document type alias.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <param name="documentTypeAlias">The document type alias to map from.</param>
        /// <returns>Fluent configuration for the newly created mapping.</returns>
        /// <exception cref="DocumentTypeNotFoundException">If the document type could not be found.</exception>
        public static INodeMappingExpression<TDestination> CreateMap<TDestination>(string documentTypeAlias)
            where TDestination : class, new()
        {
            return _engine.CreateMap<TDestination>(documentTypeAlias);
        }

        /// <summary>
        /// Maps an Umbraco <c>Node</c> to a new instance of <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <param name="sourceNode">The <c>Node</c> to map from.</param>
        /// <param name="includeRelationships">Whether to load relationships.</param>
        /// <returns>
        /// A new instance of <typeparamref name="TDestination"/>, or <c>null</c> if 
        /// <paramref name="sourceNode"/> is <c>null</c>.
        /// </returns>
        /// <exception cref="MapNotFoundException">
        /// If a map for <typeparamref name="TDestination"/> has not 
        /// been created with <see cref="CreateMap()" />.
        /// </exception>
        /// <exception cref="WrongNodeForMapException">
        /// If no map could be found for <paramref name="sourceNode"/>'s
        /// node type alias to <typeparamref name="TDestination"/> or any class which derives from 
        /// <typeparamref name="TDestination"/>
        /// </exception>
        public static TDestination Map<TDestination>(Node sourceNode, bool includeRelationships = true)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(sourceNode, includeRelationships);
        }

        /// <summary>
        /// Gets an Umbraco <c>Node</c> as a new instance of <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TDestination">The type that the <c>Node</c> maps to.</typeparam>
        /// <param name="id">The ID of the <c>Node</c></param>
        /// <param name="includeRelationships">Whether to load all the <c>Node</c>'s relationships</param>
        /// <returns><c>null</c> if the <c>Node</c> does not exist.</returns>
        /// <exception cref="MapNotFoundException">
        /// If a map for <typeparamref name="TDestination"/> has not 
        /// been created with <see cref="CreateMap()" />.
        /// </exception>
        /// <exception cref="WrongNodeForMapException">
        /// If no map could be found for the node to <typeparamref name="TDestination"/> 
        /// or any class which derives from <typeparamref name="TDestination"/>
        /// </exception>
        public static TDestination GetSingle<TDestination>(int id, bool includeRelationships = true)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(new Node(id), includeRelationships);
        }

        /// <summary>
        /// Gets an Umbraco <c>Node</c> as a new instance of <typeparamref name="TDestination"/>,
        /// only including the specified relationships.
        /// </summary>
        /// <typeparam name="TDestination">The type that the <c>Node</c> maps to.</typeparam>
        /// <param name="id">The ID of the <c>Node</c></param>
        /// <param name="includedRelationships">
        /// The relationships to populate <typeparamref name="TDestination"/> with.
        /// </param>
        /// <returns><c>null</c> if the <c>Node</c> does not exist.</returns>
        /// <exception cref="MapNotFoundException">
        /// If a map for <typeparamref name="TDestination"/> has not 
        /// been created with <see cref="CreateMap()" />.
        /// </exception>
        /// <exception cref="WrongNodeForMapException">
        /// If no map could be found for the node to <typeparamref name="TDestination"/> 
        /// or any class which derives from <typeparamref name="TDestination"/>
        /// </exception>
        /// <example>
        /// <code>
        /// var person = uMapper.GetSingle(1234, x => x.Friends, x => x.Parent);
        /// person.Name; // not a relationship and not null
        /// person.Friends; // not null
        /// person.Parent; // not null
        /// person.Colleagues; // null
        /// </code>
        /// </example>
        public static TDestination GetSingle<TDestination>(int id, params Expression<Func<TDestination, object>>[] includedRelationships)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(new Node(id), includedRelationships);
        }

        /// <summary>
        /// Gets the current Umbraco <c>Node</c> as a new instance of <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TDestination">The type that the current <c>Node</c> maps to.</typeparam>
        /// <param name="includeRelationships">Whether to load all the <c>Node</c>'s relationships</param>
        /// <returns><c>null</c> if there is no current <c>Node</c>.</returns>
        /// <exception cref="MapNotFoundException">
        /// If a map for <typeparamref name="TDestination"/> has not 
        /// been created with <see cref="CreateMap()" />.
        /// </exception>
        /// <exception cref="WrongNodeForMapException">
        /// If no map could be found for the current node to <typeparamref name="TDestination"/> 
        /// or any class which derives from <typeparamref name="TDestination"/>
        /// </exception>
        /// <seealso cref="GetSingle(int, bool)"/>
        public static TDestination GetCurrent<TDestination>(bool includeRelationships = true)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(Node.GetCurrent(), includeRelationships);
        }

        /// <summary>
        /// Gets the current Umbraco <c>Node</c> as a new instance of <typeparamref name="TDestination"/>,
        /// only including the specified relationships.
        /// </summary>
        /// <typeparam name="TDestination">The type that the current <c>Node</c> maps to.</typeparam>
        /// <param name="includedRelationships">
        /// The relationships to populate <typeparamref name="TDestination"/> with.
        /// </param>
        /// <returns><c>null</c> if there is no current <c>Node</c>.</returns>
        /// <exception cref="WrongNodeForMapException">
        /// If no map could be found for the current node to <typeparamref name="TDestination"/> 
        /// or any class which derives from <typeparamref name="TDestination"/>
        /// </exception>
        public static TDestination GetCurrent<TDestination>(params Expression<Func<TDestination, object>>[] includedRelationships)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(Node.GetCurrent(), includedRelationships);
        }

        /// <summary>
        /// Gets all Umbraco <c>Node</c>s which map to <typeparamref name="TDestination"/> (including nodes which
        /// map to a class which derives from <typeparamref name="TDestination"/>).
        /// </summary>
        /// <typeparam name="TDestination">The type for the <c>Node</c>s to map to.</typeparam>
        /// <param name="includeRelationships">Whether to load all the <c>Node</c>s' relationships</param>
        /// <exception cref="MapNotFoundException">
        /// If a map for <typeparamref name="TDestination"/> has not 
        /// been created with <see cref="CreateMap()" />.
        /// </exception>
        public static IEnumerable<TDestination> GetAll<TDestination>(bool includeRelationships = true)
            where TDestination : class, new()
        {
            var destinationType = typeof(TDestination);

            if (!_engine.NodeMappers.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            var sourceNodeTypeAliases = _engine.GetCompatibleNodeTypeAliases(destinationType);

            return sourceNodeTypeAliases.SelectMany(alias =>
            {
                var nodes = uQuery.GetNodesByType(alias);

                return nodes.Select(n => _engine.Map<TDestination>(n, includeRelationships));
            });
        }

        /// <summary>
        /// Gets all Umbraco <c>Node</c>s which map to <typeparamref name="TDestination"/> (including nodes which
        /// map to a class which derives from <typeparamref name="TDestination"/>),
        /// only including the specified relationships.
        /// </summary>
        /// <typeparam name="TDestination">The type for the <c>Node</c>s to map to.</typeparam>
        /// <param name="includedRelationships">
        /// The relationships to populate the <typeparamref name="TDestination"/>s with.
        /// </param>
        /// <exception cref="MapNotFoundException">
        /// If a map for <typeparamref name="TDestination"/> has not 
        /// been created with <see cref="CreateMap()" />.
        /// </exception>
        public static IEnumerable<TDestination> GetAll<TDestination>(params Expression<Func<TDestination, object>>[] includedRelationships)
            where TDestination : class, new()
        {
            var destinationType = typeof(TDestination);

            if (!_engine.NodeMappers.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            var sourceNodeTypeAliases = _engine.GetCompatibleNodeTypeAliases(destinationType);

            return sourceNodeTypeAliases.SelectMany(alias =>
            {
                var nodes = uQuery.GetNodesByType(alias);

                return nodes.Select(n => _engine.Map<TDestination>(n, includedRelationships));
            });
        }
    }
}
