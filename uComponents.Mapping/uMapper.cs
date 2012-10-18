using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.NodeFactory;

namespace uComponents.Mapping
{
    /// <summary>
    /// Static convenience class for the NodeMappingEngine
    /// </summary>
    public static class uMapper
    {
        private static NodeMappingEngine _engine;

        private static void EnsureMappingEngine()
        {
            if (_engine == null)
            {
                _engine = new NodeMappingEngine();
            }
        }

        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type, 
        /// using unqualified destination class name as the document type alias.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <exception cref="DocumentTypeNotFoundException">If the source document type could not be found</exception>
        public static void CreateMap<TDestination>()
            where TDestination : class, new()
        {
            EnsureMappingEngine();

            _engine.CreateMap<TDestination>();
        }

        /// <summary>
        /// Creates a map to a strong type from an Umbraco document type.
        /// </summary>
        /// <typeparam name="TDestination">The type to map to.</typeparam>
        /// <param name="documentTypeAlias">The alias of the document type to map from.</param>
        /// <exception cref="DocumentTypeNotFoundException">If the source document type could not be found</exception>
        public static void CreateMap<TDestination>(string documentTypeAlias)
            where TDestination : class, new()
        {
            EnsureMappingEngine();

            _engine.CreateMap<TDestination>(documentTypeAlias);
        }

        /// <summary>
        /// Maps an Umbraco node to a strongly typed object.
        /// </summary>
        /// <typeparam name="TDestination">The type of object to map to.</typeparam>
        /// <param name="sourceNode">The node to map from.</param>
        /// <param name="includeRelationships">Whether to load relationships to other models.</param>
        /// <returns>A new instance of TDestination, or null if sourceNode is null.</returns>
        /// <exception cref="MapNotFoundException">If a map has not been created with CreateMap</exception>
        public static TDestination Map<TDestination>(Node sourceNode, bool includeRelationships = false)
            where TDestination : class, new()
        {
            return _engine.Map<TDestination>(sourceNode, includeRelationships);
        }
    }
}
