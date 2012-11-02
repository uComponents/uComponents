using System;
using System.Collections.Generic;
using umbraco.NodeFactory;
using System.Reflection;
namespace uComponents.Mapping
{
    /// <summary>
    /// Property-wise maps a single type of Umbraco Node to a type of model.
    /// </summary>
    public interface INodeMapper
    {
        /// <summary>
        /// Maps a Node to a strongly typed model, excluding relationships except those specified.
        /// </summary>
        /// <param name="sourceNode">The node to map from</param>
        /// <param name="includedRelationships">An array of properties on the model which
        /// relationships should be mapped to, or null to map all properties.</param>
        object MapNode(Node sourceNode, PropertyInfo[] includedRelationships);
    }
}
