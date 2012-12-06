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

        #region Default properties

        public INodeMappingExpression<TDestination> DefaultProperty<TSourceProperty, TDestinationProperty>(
            Expression<Func<TDestination, TDestinationProperty>> destinationProperty,
            Expression<Func<Node, TSourceProperty>> nodeProperty,
            Func<TSourceProperty, TDestinationProperty> mapping
            )
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }
            else if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }
            else if (nodeProperty == null)
            {
                throw new ArgumentNullException("nodeProperty");
            }

            var mapper = new DefaultPropertyMapper(
                _nodeMapper,
                destinationProperty.GetPropertyInfo(),
                nodeProperty.GetPropertyInfo(),
                x => (object)mapping((TSourceProperty)x)
                );

            _nodeMapper.InsertPropertyMapper(mapper);

            return this;
        }

        #endregion

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

            var mapper = new BasicPropertyMapper(
                null,
                null,
                _nodeMapper,
                destinationProperty.GetPropertyInfo(),
                propertyAlias
                );

            _nodeMapper.InsertPropertyMapper(mapper);

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

            var mapper = new BasicPropertyMapper(
                x => mapping((TSourceProperty)x),
                typeof(TSourceProperty),
                _nodeMapper,
                destinationProperty.GetPropertyInfo(),
                propertyAlias
                );

            _nodeMapper.InsertPropertyMapper(mapper);

            return this;
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

            var mapper = new SinglePropertyMapper(
                null,
                null,
                _nodeMapper,
                destinationProperty.GetPropertyInfo(),
                propertyAlias
                );

            _nodeMapper.InsertPropertyMapper(mapper);

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

            var mapper = new SinglePropertyMapper(
                x => mapping((TSourceProperty)x),
                typeof(TSourceProperty),
                _nodeMapper,
                destinationProperty.GetPropertyInfo(),
                propertyAlias
                );

            _nodeMapper.InsertPropertyMapper(mapper);

            return this;
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

            var mapper = new CollectionPropertyMapper(
                null,
                null,
                _nodeMapper,
                destinationProperty.GetPropertyInfo(),
                propertyAlias
                );

            _nodeMapper.InsertPropertyMapper(mapper);

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

            var mapper = new CollectionPropertyMapper(
                x => mapping((TSourceProperty)x),
                typeof(TSourceProperty),
                _nodeMapper,
                destinationProperty.GetPropertyInfo(),
                propertyAlias
                );

            _nodeMapper.InsertPropertyMapper(mapper);

            return this;
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

            var mapper = new CustomPropertyMapper(
                mapping,
                requiresInclude,
                allowCaching,
                _nodeMapper,
                destinationProperty.GetPropertyInfo()
                );

            _nodeMapper.InsertPropertyMapper(mapper);

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

            _nodeMapper.RemovePropertyMapper(destinationProperty.GetPropertyInfo());

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
                destinationProperty.GetPropertyInfo()
                );

            _nodeMapper.InsertPropertyMapper(mapper);

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

        /// <summary>
        /// See <c>INodeMappingExpression.ForProperty()</c>
        /// </summary>
        [Obsolete]
        public INodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            string nodeTypeAlias
            )
        {
            if (string.IsNullOrEmpty(nodeTypeAlias))
            {
                throw new ArgumentException("Node type alias cannot be null", "nodeTypeAlias");
            }

            var destinationPropertyInfo = destinationProperty.GetPropertyInfo();
            var propertyType = destinationPropertyInfo.PropertyType;
            PropertyMapperBase mapper = null;

            if (propertyType.GetMappedPropertyType() == MappedPropertyType.SystemOrEnum)
            {
                mapper = new BasicPropertyMapper(
                    null,
                    null,
                    _nodeMapper,
                    destinationPropertyInfo,
                    nodeTypeAlias
                    );
            }
            else if (propertyType.GetMappedPropertyType() == MappedPropertyType.Model)
            {
                mapper = new SinglePropertyMapper(
                    null,
                    null,
                    _nodeMapper,
                    destinationPropertyInfo,
                    nodeTypeAlias
                    );
            }
            else if (propertyType.GetMappedPropertyType() == MappedPropertyType.Collection)
            {
                mapper = new CollectionPropertyMapper(
                    null,
                    null,
                    _nodeMapper,
                    destinationPropertyInfo,
                    nodeTypeAlias
                    );
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    "TProperty", 
                    string.Format("The property type {0} on model {1} is not supported.", propertyType.FullName, typeof(TDestination).FullName)
                    );
            }

            _nodeMapper.InsertPropertyMapper(mapper);

            return this;
        }

        #endregion

    }

    /// <summary>
    /// Thrown when a model property could not be mapped to a document type property.
    /// </summary>
    public class PropertyAliasNotFoundException : Exception
    {
        /// <summary>
        /// Instantiates the exception.
        /// </summary>
        public PropertyAliasNotFoundException(Type destinationType, PropertyInfo property, string documentTypeAlias)
            : base(string.Format("Could not map {0}'s {1} property to a property on nodes of alias {2}.", destinationType.FullName, property.Name, documentTypeAlias))
        {
        }
    }

    /// <summary>
    /// Thrown when a model property could not be mapped to a default Node property.
    /// </summary>
    public class DefaultPropertyTypeException : Exception
    {
        /// <summary>
        /// Instantiates the exception.
        /// </summary>
        public DefaultPropertyTypeException(Type destinationType, PropertyInfo destinationProperty, PropertyInfo nodeProperty)
            : base(string.Format("Could not map {0}'s {1} property to a default Node property {2}.", destinationType.FullName, destinationProperty.Name, nodeProperty.Name))
        {
        }
    }
}
