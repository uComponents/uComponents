using System;
using umbraco.NodeFactory;
namespace uComponents.Mapping
{
    /// <summary>
    /// Mapper which maps from Umbraco Node properties to strongly typed model properties
    /// </summary>
    public interface INodePropertyMapper
    {
        /// <summary>
        /// Maps the property from a node
        /// </summary>
        /// <param name="sourceNode">The node to map from</param>
        /// <returns>The strongly typed, mapped property</returns>
        object MapProperty(Node sourceNode);
    }
}
