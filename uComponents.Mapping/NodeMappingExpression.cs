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
    public class NodeMappingExpression<TDestination>
    {
        protected NodeMappingEngine Engine { get; set; }
        protected NodeMapper<TDestination> Mapping { get; set; }

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

            Mapping = mapping;
            Engine = engine;
        }

        /// <summary>
        /// Allows a different node type alias to be set for a specific member.
        /// </summary>
        /// <param name="destinationProperty">The member of the destination model
        /// to map to.</param>
        /// <param name="nodeTypeAlias">The new node type alias to map from.</param>
        /// <exception cref="ArgumentNullException">If destinationProperty is null</exception>
        /// <exception cref="ArgumentException">If nodeTypeAlias is null or empty</exception>
        public NodeMappingExpression<TDestination> ForProperty<TProperty>(
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

            var existingPropertyMapper = this.Mapping.PropertyMappers
                .SingleOrDefault(x => x.DestinationInfo.Name == name);

            if (existingPropertyMapper != null)
            {
                Mapping.PropertyMappers.Remove(existingPropertyMapper);
            }

            var newPropertyMapper = new NodePropertyMapper(Engine, typeof(TDestination).GetProperty(name), nodeTypeAlias);
            Mapping.PropertyMappers.Add(newPropertyMapper);

            return this;
        }

        /// <summary>
        /// Allows a different mapping to be used for a specific member.
        /// </summary>
        /// <param name="destinationProperty">The member of the destination model
        /// to map to.</param>
        /// <param name="propertyMappingExpression">The new mapping.</param>
        /// <param name="isRelationship">Whether the property should be considered
        /// a relationship or not.</param>
        public NodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            Expression<Func<Node, TProperty>> propertyMappingExpression,
            bool isRelationship
            )
        {
            string name = GetPropertyName(destinationProperty);

            var existingMapper = this.Mapping.PropertyMappers
                .SingleOrDefault(x => x.DestinationInfo.Name == name);

            if (existingMapper != null)
            {
                Mapping.PropertyMappers.Remove(existingMapper);
            }

            var internalPropertyMapping = propertyMappingExpression.Compile();
            Func<Node, string, object> newPropertyMapping = ((node, alias) => internalPropertyMapping(node));

            // Recreate property map
            var newMapper = new NodePropertyMapper(Engine, typeof(TDestination).GetProperty(name), newPropertyMapping, isRelationship);

            Mapping.PropertyMappers.Add(newMapper);
            return this;
        }

        /// <summary>
        /// Removes the mapping for a property, if any exists.
        /// </summary>
        /// <param name="destinationProperty">The property on the model to NOT map to</param>
        /// <exception cref="ArgumentNullException">If destinationProperty is null</exception>
        public NodeMappingExpression<TDestination> RemoveMappingForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty
            )
        {
            if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }

            string name = GetPropertyName(destinationProperty);

            var existingMapper = this.Mapping.PropertyMappers
                .SingleOrDefault(x => x.DestinationInfo.Name == name);

            if (existingMapper != null)
            {
                Mapping.PropertyMappers.Remove(existingMapper);
            }

            return this;
        }

        protected string GetPropertyName<TDestination, TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty
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
