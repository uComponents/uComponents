using System;
using System.Linq.Expressions;
using umbraco.NodeFactory;
namespace uComponents.Mapping
{
    /// <summary>
    /// Fluent configuration for an INodeMappingEngine mapping
    /// </summary>
    /// <typeparam name="TDestination">The destination model type</typeparam>
    public interface INodeMappingExpression<TDestination>
    {
        /// <summary>
        /// Allows a different node type alias to be set for a specific member.
        /// </summary>
        /// <param name="destinationProperty">The member of the destination model
        /// to map to.</param>
        /// <param name="nodeTypeAlias">The new node type alias to map from.</param>
        INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty, 
            Func<Node, object> propertyMappingExpression, 
            bool isRelationship
            );

        /// <summary>
        /// Allows a different mapping to be used for a specific member.
        /// </summary>
        /// <param name="destinationProperty">The member of the destination model
        /// to map to.</param>
        /// <param name="propertyMappingExpression">The new mapping.</param>
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
