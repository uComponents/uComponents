using System;
using System.Linq.Expressions;
using umbraco.NodeFactory;
using System.Collections.Generic;
namespace uComponents.Mapping
{
    /// <summary>
    /// Fluent configuration for an INodeMappingEngine mapping
    /// </summary>
    /// <typeparam name="TDestination">The destination model type</typeparam>
    public interface INodeMappingExpression<TDestination>
    {
        /// <summary>
        /// Sets a mapping for a collection of nodes to be used for the model
        /// property.  This property will require an include.
        /// </summary>
        /// <param name="destinationProperty">The property to map to.</param>
        /// <param name="propertyMapping">
        /// The mapping function which returns a collection of nodes (which
        /// will be mapped to models).
        /// </param>
        INodeMappingExpression<TDestination> CollectionProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, IEnumerable<Node>> propertyMapping
            );

        /// <summary>
        /// Sets a mapping for a single node to be used for the model
        /// property.  This property will require an include.
        /// </summary>
        /// <param name="destinationProperty">The property to map to.</param>
        /// <param name="propertyMapping">
        /// The mapping function which returns a single node (which
        /// will be mapped to it's corresponding model).
        /// </param>
        INodeMappingExpression<TDestination> SingleProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, Node> propertyMapping
            );


        /// <summary>
        /// Sets a custom mapping to be used for a the model property.
        /// </summary>
        /// <param name="destinationProperty">The member of the destination model
        /// to map to.</param>
        /// <param name="propertyMapping">
        /// The new mapping function.  Arguments are the node being mapped, 
        /// and an array of paths relative to the property being mapped
        /// (when mapping a relationship).
        /// </param>
        /// <param name="requiresInclude">
        /// Whether the property requires an explicit include.
        /// </param>
        INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, string[], object> propertyMapping, 
            bool requiresInclude
            );

        /// <summary>
        /// Sets a custom mapping to be used for a the model property.
        /// </summary>
        /// <param name="destinationProperty">The member of the destination model
        /// to map to.</param>
        /// <param name="propertyMapping">
        /// The new mapping function.
        /// </param>
        /// <param name="isRelationship">Whether the property should be deemed a relationship
        /// or not.</param>
        [Obsolete("Use the overload of ForProperty which takes a Func<Node, string[], object> instead")]
        INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, object> propertyMapping,
            bool isRelationship
            );

        /// <summary>
        /// Sets a custom property alias to be set for a the model property.
        /// </summary>
        /// <param name="destinationProperty">The member of the destination model
        /// to map to.</param>
        /// <param name="nodeTypeAlias">The property alias to map from.</param>
        INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty, 
            string nodeTypeAlias
            );

        /// <summary>
        /// Removes the mapping for a property, if any exists.
        /// </summary>
        /// <param name="destinationProperty">The property on the model to NOT map to</param>
        INodeMappingExpression<TDestination> RemoveMappingForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty
            );
    }
}
