using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uComponents.Mapping.Property
{
    /// <summary>
    /// A delegate signature for mapping a non-relational property.
    /// </summary>
    /// <param name="propertyValue">
    /// The value of the property, taken from the <c>Node</c> being mapped.
    /// </param>
    /// <returns>The mapped property value.</returns>
    public delegate object BasicPropertyMapping(object propertyValue);

    /// <summary>
    /// A delegate signature for mapping a single-relationship property.
    /// </summary>
    /// <param name="propertyValue">
    /// The value of the property, taken from the <c>Node</c> being mapped.
    /// </param>
    /// <returns>The ID of the node which will be mapped to the model's property.</returns>
    public delegate int SinglePropertyMapping(object propertyValue);

    /// <summary>
    /// A delegate signature for mapping a collection-relationship property.
    /// </summary>
    /// <param name="propertyValue">
    /// The value of the property, taken from the <c>Node</c> being mapped.
    /// </param>
    /// <returns>A collection of node IDs which will be mapped to the model's property.</returns>
    public delegate IEnumerable<int> CollectionPropertyMapping(object propertyValue);

    /// <summary>
    /// A delegate signature for mapping a custom property.
    /// </summary>
    /// <param name="nodeId">
    /// The ID of the node whose property is being mapped.  You have to retrieve the
    /// node property value yourself, using the relevant Umbraco API calls.
    /// </param>
    /// <param name="paths">
    /// The paths of the mapping, relative to the property
    /// being mapped.  This should be used if this delegate maps
    /// out child relationships.
    /// </param>
    /// <param name="cache">
    /// A cache to optionally use while mapping the property.  Will be <c>null</c> if
    /// caching is disabled on the <see cref="INodeMappingEngine"/>.
    /// </param>
    /// <returns>The mapped property value.</returns>
    public delegate object CustomPropertyMapping(int nodeId, string[] paths, ICacheProvider cache);
}
