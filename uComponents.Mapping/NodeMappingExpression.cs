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

        #region Basic properties

        public INodeMappingExpression<TDestination> BasicProperty(
            Expression<Func<TDestination, object>> destinationProperty,
            string propertyAlias
            )
        {
            if (string.IsNullOrEmpty(propertyAlias))
            {
                throw new ArgumentException("Property alias cannot be null", "propertyAlias");
            }


            AddBasicPropertyMapping(
                destinationProperty,
                null,
                null,
                propertyAlias
                );

            return this;
        }

        public INodeMappingExpression<TDestination> BasicProperty<TSourceProperty>(
            Expression<Func<TDestination, object>> destinationProperty,
            BasicPropertyMapping<TSourceProperty> mapping,
            string propertyAlias = null
            )
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }

            AddBasicPropertyMapping(
                destinationProperty,
                x => mapping((TSourceProperty)x),
                typeof(TSourceProperty),
                propertyAlias
                );

            return this;
        }

        private void AddBasicPropertyMapping(
            Expression<Func<TDestination, object>> destinationProperty,
            Func<object, object> mapping,
            Type sourcePropertyType,
            string propertyAlias
            )
        {
            if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }

            var destinationPropertyInfo = (destinationProperty.Body as MemberExpression).Member as PropertyInfo;

            var mapper = new BasicPropertyMapper(
                mapping,
                sourcePropertyType,
                _nodeMapper,
                destinationPropertyInfo,
                propertyAlias
                );

            RemoveMappingForProperty(destinationProperty);

            _nodeMapper.PropertyMappers.Add(mapper);

            if (_engine.CacheProvider != null)
            {
                _engine.CacheProvider.Clear();
            }
        }

        #endregion

        #region Single Properties

        public INodeMappingExpression<TDestination> SingleProperty(
            Expression<Func<TDestination, object>> destinationProperty,
            string propertyAlias
            )
        {
            if (string.IsNullOrEmpty(propertyAlias))
            {
                throw new ArgumentException("Property alias cannot be null", "propertyAlias");
            }

            AddSinglePropertyMapping(
                destinationProperty,
                null,
                null,
                propertyAlias
                );

            return this;
        }

        public INodeMappingExpression<TDestination> SingleProperty<TSourceProperty>(
            Expression<Func<TDestination, object>> destinationProperty,
            SinglePropertyMapping<TSourceProperty> mapping,
            string propertyAlias = null
            )
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }

            AddSinglePropertyMapping(
                destinationProperty,
                x => mapping((TSourceProperty)x),
                typeof(TSourceProperty),
                propertyAlias
                );

            return this;
        }

        private void AddSinglePropertyMapping(
            Expression<Func<TDestination, object>> destinationProperty,
            Func<object, int?> mapping,
            Type sourcePropertyType,
            string propertyAlias
            )
        {
            if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }

            var destinationPropertyInfo = (destinationProperty.Body as MemberExpression).Member as PropertyInfo;

            var mapper = new SinglePropertyMapper(
                mapping,
                sourcePropertyType,
                _nodeMapper,
                destinationPropertyInfo,
                propertyAlias
                );

            RemoveMappingForProperty(destinationProperty);

            _nodeMapper.PropertyMappers.Add(mapper);

            if (_engine.CacheProvider != null)
            {
                _engine.CacheProvider.Clear();
            }
        }

        #endregion

        #region Collection properties

        public INodeMappingExpression<TDestination> CollectionProperty(
            Expression<Func<TDestination, object>> destinationProperty,
            string propertyAlias
            )
        {
            if (string.IsNullOrEmpty(propertyAlias))
            {
                throw new ArgumentException("Property alias cannot be null", "propertyAlias");
            }

            AddCollectionPropertyMapping(
                destinationProperty,
                null,
                null,
                propertyAlias
                );

            return this;
        }

        public INodeMappingExpression<TDestination> CollectionProperty<TSourceProperty>(
            Expression<Func<TDestination, object>> destinationProperty,
            CollectionPropertyMapping<TSourceProperty> mapping,
            string propertyAlias = null
            )
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }

            AddCollectionPropertyMapping(
                destinationProperty,
                x => mapping((TSourceProperty)x),
                typeof(TSourceProperty),
                propertyAlias
                );

            return this;
        }

        private void AddCollectionPropertyMapping(
            Expression<Func<TDestination, object>> destinationProperty,
            Func<object, IEnumerable<int>> mapping,
            Type sourcePropertyType,
            string propertyAlias
            )
        {
            if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }

            var destinationPropertyInfo = (destinationProperty.Body as MemberExpression).Member as PropertyInfo;

            var mapper = new CollectionPropertyMapper(
                mapping,
                sourcePropertyType,
                _nodeMapper,
                destinationPropertyInfo,
                propertyAlias
                );

            RemoveMappingForProperty(destinationProperty);

            _nodeMapper.PropertyMappers.Add(mapper);

            if (_engine.CacheProvider != null)
            {
                _engine.CacheProvider.Clear();
            }
        }

        #endregion

        #region Custom properties

        public INodeMappingExpression<TDestination> CustomProperty(
            Expression<Func<TDestination, object>> destinationProperty,
            CustomPropertyMapping mapping,
            bool requiresInclude,
            bool allowCaching
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

            var mapper = new CustomPropertyMapper(
                mapping,
                requiresInclude,
                allowCaching,
                _nodeMapper,
                destinationPropertyInfo
                );

            RemoveMappingForProperty(destinationProperty);

            _nodeMapper.PropertyMappers.Add(mapper);

            if (_engine.CacheProvider != null)
            {
                _engine.CacheProvider.Clear();
            }

            return this;
        }

        #endregion

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

            if (_engine.CacheProvider != null)
            {
                _engine.CacheProvider.Clear();
            }

            return this;
        }

        #region Legacy

        /// <summary>
        /// See <c>INodeMappingExpression.ForProperty()</c>
        /// </summary>
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

            CustomPropertyMapping mapping = (id, paths, cache) =>
            {
                var node = new Node(id);

                if (string.IsNullOrEmpty(node.Name))
                {
                    return null;
                }

                return propertyMapping(node, paths);
            }; 

            var mapper = new CustomPropertyMapper(
                mapping,
                requiresInclude,
                false,
                _nodeMapper,
                destinationPropertyInfo
                );

            var existingMapper = this._nodeMapper.PropertyMappers
                .SingleOrDefault(x => x.DestinationInfo.Name == destinationPropertyInfo.Name);

            if (existingMapper != null)
            {
                _nodeMapper.PropertyMappers.Remove(existingMapper);
            }

            _nodeMapper.PropertyMappers.Add(mapper);

            if (_engine.CacheProvider != null)
            {
                _engine.CacheProvider.Clear();
            }

            return this;
        }

        /// <summary>
        /// See <c>INodeMappingExpression.ForProperty()</c>
        /// </summary>
        [Obsolete]
        public INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Func<Node, object> propertyMapping,
            bool isRelationship
            )
        {
            Func<Node, string[], object> mapping = (node, paths) => propertyMapping(node);

            if (_engine.CacheProvider != null)
            {
                _engine.CacheProvider.Clear();
            }

            return ForProperty(
                destinationProperty,
                mapping,
                isRelationship
                );
        }

        #endregion
    }
}
