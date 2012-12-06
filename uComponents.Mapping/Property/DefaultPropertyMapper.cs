using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using umbraco.NodeFactory;

namespace uComponents.Mapping.Property
{
    internal class DefaultPropertyMapper : PropertyMapperBase
    {
        private DefaultPropertyMapping _mapping;

        public DefaultPropertyMapper(
            DefaultPropertyMapping mapping,
            NodeMapper nodeMapper,
            PropertyInfo destinationProperty
            )
            :base(nodeMapper, destinationProperty, null)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }

            RequiresInclude = false;
            AllowCaching = true;
            _mapping = mapping;
        }

        public override object MapProperty(NodeMappingContext context)
        {
            object value = null;

            // Check cache
            if (AllowCaching
                && Engine.CacheProvider != null 
                && Engine.CacheProvider.ContainsPropertyValue(context.Id, DestinationInfo.Name))
            {
                value = Engine.CacheProvider.GetPropertyValue(context.Id, DestinationInfo.Name);
            }
            else
            {
                var node = context.GetNode();

                if (node == null || string.IsNullOrEmpty(node.Name))
                {
                    throw new InvalidOperationException("Node cannot be null or empty");
                }

                value = _mapping(node);

                if (AllowCaching
                    && Engine.CacheProvider != null)
                {
                    Engine.CacheProvider.InsertPropertyValue(context.Id, DestinationInfo.Name, value);
                }
            }

            return value;
        }

        /// <summary>
        /// Given a model property name, returns a <see cref="DefaultPropertyMapping"/> if
        /// there is a match again the properties of <c>Node</c>.
        /// </summary>
        /// <param name="propertyName">The model property name.</param>
        /// <returns>The mapping or <c>null</c> if no match was found.</returns>
        public static DefaultPropertyMapping GetDefaultMappingForName(string propertyName)
        {
            switch (propertyName.ToLowerInvariant())
            {
                case "createdate":
                    return n => n.CreateDate;

                case "creatorid":
                    return n => n.CreatorID;

                case "creatorname":
                    return n => n.CreatorName;

                case "id":
                    return n => n.Id;

                case "level":
                    return n => n.Level;

                case "name":
                    return n => n.Name;

                case "niceurl":
                    return n => n.NiceUrl;

                case "nodetypealias":
                    return n => n.NodeTypeAlias;

                case "path":
                    return n => n.Path;

                case "sortorder":
                    return n => n.SortOrder;

                case "template":
                    return n => n.template;

                case "updatedate":
                    return n => n.UpdateDate;

                case "url":
                    return n => n.Url;

                case "urlname":
                    return n => n.UrlName;

                case "version":
                    return n => n.Version;

                case "writerid":
                    return n => n.WriterID;

                case "writername":
                    return n => n.WriterName;
            }

            return null;
        }
    }
}
