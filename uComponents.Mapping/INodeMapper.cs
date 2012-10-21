using System;
using System.Collections.Generic;
using umbraco.NodeFactory;
namespace uComponents.Mapping
{
    /// <summary>
    /// Property-wise maps a single type of Umbraco Node to a type of model.
    /// </summary>
    public interface INodeMapper
    {
        /// <summary>
        /// Maps a Node to a strongly typed model
        /// </summary>
        /// <param name="sourceNode">The node to map from</param>
        /// <param name="includeRelationships">
        /// Whether to include relationships with other nodes
        /// </param>
        /// <returns>The strongly typed model</returns>
        object MapNode(Node sourceNode, bool includeRelationships);
    }

    /// <summary>
    /// Property-wise maps a single type of Umbraco Node to an instance of TDestination.
    /// </summary>
    public interface INodeMapper<TDestination> : INodeMapper
    {
        /// <summary>
        /// Maps a Node to an instance of TDestination
        /// </summary>
        /// <param name="sourceNode">The node to map from</param>
        /// <param name="includeRelationships">
        /// Whether to include relationships with other nodes
        /// </param>
        new TDestination MapNode(Node sourceNode, bool includeRelationships);
    }
}
