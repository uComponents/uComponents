using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using umbraco.NodeFactory;
using uComponents.Mapping.Property;

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

        #region Mapping properties

        public INodeMappingExpression<TDestination> BasicProperty(
            Expression<Func<TDestination, object>> destinationProperty, 
            string propertyAlias
            )
        {
            if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }
            else if (string.IsNullOrEmpty(propertyAlias))
            {
                throw new ArgumentException("Property alias cannot be null", "propertyAlias");
            }

            var destinationPropertyInfo = (destinationProperty.Body as MemberExpression).Member as PropertyInfo;

            var mapper = new BasicPropertyMapper(
                null,
                null,
                _nodeMapper,
                destinationPropertyInfo,
                propertyAlias
                );

            RemoveMappingForProperty(destinationProperty);

            _nodeMapper.PropertyMappers.Add(mapper);

            return this;
        }

        public INodeMappingExpression<TDestination> BasicProperty<TSourceProperty>(
            Expression<Func<TDestination, object>> destinationProperty, 
            BasicPropertyMapping<TSourceProperty> mapping, 
            string propertyAlias = null
            )
        {
            if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }
            else if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }

            var destinationPropertyInfo = (destinationProperty.Body as MemberExpression).Member as PropertyInfo;

            var mapper = new BasicPropertyMapper(
                x => mapping((TSourceProperty)x),
                typeof(TSourceProperty),
                _nodeMapper,
                destinationPropertyInfo,
                propertyAlias
                );

            RemoveMappingForProperty(destinationProperty);

            _nodeMapper.PropertyMappers.Add(mapper);

            return this;
        }

        public INodeMappingExpression<TDestination> SingleProperty(
            Expression<Func<TDestination, object>> destinationProperty, 
            string propertyAlias
            )
        {
            if (string.IsNullOrEmpty(propertyAlias))
            {
                throw new ArgumentException("Property alias cannot be null", "propertyAlias");
            }

            throw new NotImplementedException();
        }

        public INodeMappingExpression<TDestination> SingleProperty<TSourceProperty>(
            Expression<Func<TDestination, object>> destinationProperty, 
            SinglePropertyMapping<TSourceProperty> mapping, 
            string propertyAlias = null
            )
        {
            throw new NotImplementedException();
        }

        public INodeMappingExpression<TDestination> CollectionProperty(
            Expression<Func<TDestination, object>> destinationProperty, 
            string propertyAlias
            )
        {
            if (string.IsNullOrEmpty(propertyAlias))
            {
                throw new ArgumentException("Property alias cannot be null", "propertyAlias");
            }
            throw new NotImplementedException();
        }

        public INodeMappingExpression<TDestination> CollectionProperty<TSourceProperty>(
            Expression<Func<TDestination, object>> destinationProperty, 
            CollectionPropertyMapping<TSourceProperty> mapping, 
            string propertyAlias = null
            )
        {
            throw new NotImplementedException();
        }

        public INodeMappingExpression<TDestination> CustomProperty(
            Expression<Func<TDestination, object>> destinationProperty, 
            CustomPropertyMapping propertyMapping, 
            bool requiresInclude, 
            bool allowCaching
            )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the mapping for a property, if any exists.
        /// </summary>
        /// <param name="destinationProperty">The property on the model to NOT map to</param>
        /// <exception cref="ArgumentNullException">If destinationProperty is null</exception>
        public INodeMappingExpression<TDestination> RemoveMappingForProperty(
            Expression<Func<TDestination, object>> destinationProperty
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

        #endregion

        #region Legacy

        /// <see cref="INodeMappingExpression{T}.CollectionProperty{T}"/>
        [Obsolete]
        public INodeMappingExpression<TDestination> CollectionProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, IEnumerable<Node>> propertyMapping
            )
        {
            throw new NotImplementedException();
        }

        /// <see cref="INodeMappingExpression{T}.SingleProperty{T}"/>
        [Obsolete]
        public INodeMappingExpression<TDestination> SingleProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, Node> propertyMapping
            )
        {
            throw new NotImplementedException();
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
        /// <param name="requiresInclude">
        /// Whether the property requires an explicit include to be mapped.
        /// </param>
        [Obsolete]
        public INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, string[], object> propertyMapping,
            bool requiresInclude
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
            var newMapper = new PropertyMapperBase(_nodeMapper, destinationPropertyInfo, propertyMapping, requiresInclude);

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

        #endregion
    }
}
