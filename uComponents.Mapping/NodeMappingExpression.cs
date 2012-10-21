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
        private NodeMapper<TDestination> _nodeMapper { get; set; }

        public NodeMappingExpression(NodeMappingEngine engine, NodeMapper<TDestination> mapping)
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

            string name = GetPropertyName(destinationProperty);

            var existingPropertyMapper = this._nodeMapper.PropertyMappers
                .SingleOrDefault(x => x.DestinationInfo.Name == name);

            if (existingPropertyMapper != null)
            {
                _nodeMapper.PropertyMappers.Remove(existingPropertyMapper);
            }

            var newPropertyMapper = new NodePropertyMapper(_nodeMapper, typeof(TDestination).GetProperty(name), nodeTypeAlias);
            _nodeMapper.PropertyMappers.Add(newPropertyMapper);

            return this;
        }

        /// <summary>
        /// Sets a custom mapping to be used for a the model property.
        /// </summary>
        /// <param name="destinationProperty">The member of the destination model
        /// to map to.</param>
        /// <param name="propertyMapping">The new mapping function.</param>
        /// <param name="isRelationship">Whether the property should be deemed a relationship
        /// or not.</param>
        public INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, object> propertyMapping,
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

            string name = GetPropertyName(destinationProperty);

            var existingMapper = this._nodeMapper.PropertyMappers
                .SingleOrDefault(x => x.DestinationInfo.Name == name);

            if (existingMapper != null)
            {
                _nodeMapper.PropertyMappers.Remove(existingMapper);
            }

            // Recreate property map
            var newMapper = new NodePropertyMapper(_nodeMapper, typeof(TDestination).GetProperty(name), propertyMapping, isRelationship);

            _nodeMapper.PropertyMappers.Add(newMapper);
            return this;
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

            string name = GetPropertyName(destinationProperty);

            var existingMapper = this._nodeMapper.PropertyMappers
                .SingleOrDefault(x => x.DestinationInfo.Name == name);

            if (existingMapper != null)
            {
                _nodeMapper.PropertyMappers.Remove(existingMapper);
            }

            return this;
        }

        private string GetPropertyName<TDest, TProperty>(
            Expression<Func<TDest, TProperty>> destinationProperty
            )
        {
            if (destinationProperty == null)
            {
                throw new InvalidOperationException("The destination member must be specified");
            }

            var expression = (MemberExpression)destinationProperty.Body;
            return expression.Member.Name;
        }
    }
}
