using System;
using System.Linq.Expressions;
using umbraco.NodeFactory;
using System.Collections.Generic;
using uComponents.Mapping.Property;
namespace uComponents.Mapping
{
    /// <summary>
    /// Fluent configuration for an <see cref="INodeMappingEngine"/> mapping
    /// </summary>
    /// <typeparam name="TDestination">The destination model type</typeparam>
    public interface INodeMappingExpression<TDestination>
    {
        /// <summary>
        /// Sets the property alias to map from for an automatically mapped
        /// property.
        /// </summary>
        /// <param name="destinationProperty">The property to map to.</param>
        /// <param name="propertyAlias">
        /// The node property alias to use instead of the default.
        /// </param>
        INodeMappingExpression<TDestination> DefaultProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            string propertyAlias
            );

        /// <summary>
        /// Sets a non-relational mapping for a model property.  This property
        /// will not require an include.
        /// </summary>
        /// <param name="destinationProperty">The property to map to.</param>
        /// <param name="mapping">
        /// Maps the node property to the model property.
        /// </param>
        INodeMappingExpression<TDestination> BasicProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            BasicPropertyMapping mapping,
            string propertyAlias = null
            );

        /// <summary>
        /// Sets a mapping for a single node to be used for the model
        /// property.  This property will require an include.
        /// </summary>
        /// <param name="destinationProperty">The property to map to.</param>
        /// <param name="mapping">
        /// A mapping which retrieves the ID of the node to map to the property.
        /// </param>
        /// <param name="propertyAlias">
        /// An optional node property alias override.
        /// </param>
        INodeMappingExpression<TDestination> SingleProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            SinglePropertyMapping mapping,
            string propertyAlias = null
            );

        /// <summary>
        /// Sets a mapping for a collection to be used for the model
        /// property.  This property will require an include.
        /// </summary>
        /// <param name="destinationProperty">The property to map to.</param>
        /// <param name="mapping">
        /// A mapping which retrieves the IDs of the items in the collection.
        /// </param>
        /// <param name="propertyAlias">
        /// An optional node property alias override.
        /// </param>
        INodeMappingExpression<TDestination> CollectionProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            CollectionPropertyMapping mapping,
            string propertyAlias = null
            );

        /// <summary>
        /// Sets a custom mapping to be used for a model property.
        /// </summary>
        /// <param name="destinationProperty">The property to map to.</param>
        /// <param name="propertyMapping">
        /// A mapping which retrieves the value of the property.
        /// </param>
        /// <param name="requiresInclude">
        /// Whether the property requires an explicit include.
        /// </param>
        /// <param name="allowCaching">
        /// If set to true, the result of <paramref name="propertyMapping"/> will be cached.
        /// While caching is disabled on the <see cref="INodeMappingEngine"/>, no caching will occur.
        /// </param>
        INodeMappingExpression<TDestination> CustomProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            CustomPropertyMapping propertyMapping,
            bool requiresInclude,
            bool allowCaching
            );

        /// <summary>
        /// Removes the mapping for a property, if any exists.
        /// </summary>
        /// <param name="destinationProperty">The property on the model to NOT map to</param>
        INodeMappingExpression<TDestination> RemoveMappingForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty
            );

        #region Legacy

        [Obsolete("Use CustomProperty() instead")]
        INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, string[], object> propertyMapping,
            bool requiresInclude
            );

        [Obsolete("Use CustomProperty() instead")]
        INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, object> propertyMapping,
            bool isRelationship
            );

        #endregion
    }
}
