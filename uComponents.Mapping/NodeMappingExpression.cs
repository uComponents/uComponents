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
        protected NodeMap<TDestination> Mapping { get; set; }

        public NodeMappingExpression(NodeMappingEngine engine, NodeMap<TDestination> mapping)
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
        /// <param name="nodeTypeAlias">The new node type alias to map from.
        /// If null, the mapping is removed.</param>
        public NodeMappingExpression<TDestination> ForProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationProperty,
            string nodeTypeAlias
            )
        {
            if (nodeTypeAlias == string.Empty)
            {
                throw new ArgumentException("Cannot be empty", "nodeTypeAlias");
            }

            string name = GetPropertyName(destinationProperty);

            var existingPropertyMapping = this.Mapping.PropertyMappings
                .SingleOrDefault(x => x.DestinationInfo.Name == name);

            if (existingPropertyMapping != null)
            {
                Mapping.PropertyMappings.Remove(existingPropertyMapping);
            }

            if (nodeTypeAlias != null)
            {
                var newPropertyMapping = existingPropertyMapping == null
                    ? new NodePropertyMap { DestinationInfo =  }
                    : existingPropertyMapping;

                Engine.MapPropertyFromAlias(newPropertyMapping, nodeTypeAlias);
                Mapping.PropertyMappings.Add(newPropertyMapping);
            }

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
            var existingPropertyMapping = this.GetExistingNodePropertyMap(destinationProperty);

            var internalPropertyMapping = propertyMappingExpression.Compile();
            Func<Node, string, object> newPropertyMapping = (node, alias) =>
                {
                    return internalPropertyMapping(node);
                };

            // Alter property map
            existingPropertyMapping.SourceAlias = null;
            existingPropertyMapping.IsRelationship = isRelationship;
            existingPropertyMapping.Mapping = newPropertyMapping;

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
