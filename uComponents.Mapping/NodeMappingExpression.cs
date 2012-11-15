using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using umbraco.NodeFactory;

namespace uComponents.Mapping
{
    /// <summary>
    /// Fluent configuration for a NodeMap.
    /// </summary>
    /// <typeparam name="TDestination">The destination model type</typeparam>
    internal class NodeMappingExpression<TDestination> : INodeMappingExpression<TDestination>
    {
        private NodeMappingEngine _engine { get; set; }
        private NodeMapper _nodeMapper { get; set; }

        public NodeMappingExpression(NodeMappingEngine engine, NodeMapper mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }
            else if (mapping == null)
            {
                throw new ArgumentNullException("engine");
            }

            _nodeMapper = mapping;
            _engine = engine;
        }

        /// <summary>
        /// Sets a custom property alias to be set for a the model property.
        /// </summary>
        /// <param name="destinationProperty">The member of the destination model
        /// to map to.</param>
        /// <param name="nodeTypeAlias">The property alias to map from.</param>
        public INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            string nodeTypeAlias
            )
        {
            if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }
            else if (string.IsNullOrEmpty(nodeTypeAlias))
            {
                throw new ArgumentException("Node type alias cannot be empty or null", "nodeTypeAlias");
            }

            var destinationPropertyInfo = (destinationProperty.Body as MemberExpression).Member as PropertyInfo;

            var existingPropertyMapper = this._nodeMapper.PropertyMappers
                .SingleOrDefault(x => x.DestinationInfo.Name == destinationPropertyInfo.Name);

            if (existingPropertyMapper != null)
            {
                _nodeMapper.PropertyMappers.Remove(existingPropertyMapper);
            }

            var newPropertyMapper = new NodePropertyMapper(_nodeMapper, destinationPropertyInfo, nodeTypeAlias);
            _nodeMapper.PropertyMappers.Add(newPropertyMapper);

            return this;
        }

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
        /// <param name="isRelationship">Whether the property should be deemed a relationship
        /// or not.</param>
        public INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, string[], object> propertyMapping,
            bool isRelationship
            )
        {
            if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }
            else if (propertyMapping == null)
            {
                throw new ArgumentNullException("propertyMapping");
            }

            var destinationPropertyInfo = (destinationProperty.Body as MemberExpression).Member as PropertyInfo;

            var existingMapper = this._nodeMapper.PropertyMappers
                .SingleOrDefault(x => x.DestinationInfo.Name == destinationPropertyInfo.Name);

            if (existingMapper != null)
            {
                _nodeMapper.PropertyMappers.Remove(existingMapper);
            }

            // Recreate property map
            var newMapper = new NodePropertyMapper(_nodeMapper, destinationPropertyInfo, propertyMapping, isRelationship);

            _nodeMapper.PropertyMappers.Add(newMapper);
            return this;
        }

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
        [Obsolete("Use the overload of ForProperty which takes an array of paths instead")]
        public INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, object> propertyMapping,
            bool isRelationship
            )
        {
            Func<Node, string[], object> mapping = (node, paths) => propertyMapping(node);

            return ForProperty(
                destinationProperty, 
                mapping, 
                isRelationship
                );
        }

        /// <summary>
        /// Removes the mapping for a property, if any exists.
        /// </summary>
        /// <param name="destinationProperty">The property on the model to NOT map to</param>
        /// <exception cref="ArgumentNullException">If destinationProperty is null</exception>
        public INodeMappingExpression<TDestination> RemoveMappingForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty
            )
        {
            if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }

            var destinationPropertyInfo = (destinationProperty.Body as MemberExpression).Member as PropertyInfo;

            var existingMapper = this._nodeMapper.PropertyMappers
                .SingleOrDefault(x => x.DestinationInfo.Name == destinationPropertyInfo.Name);

            if (existingMapper != null)
            {
                _nodeMapper.PropertyMappers.Remove(existingMapper);
            }

            return this;
        }
    }
}
