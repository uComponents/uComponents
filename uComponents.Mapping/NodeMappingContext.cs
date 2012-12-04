using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using umbraco.NodeFactory;

namespace uComponents.Mapping
{
    /// <summary>
    /// Describes the mapping for a single node.
    /// </summary>
    internal class NodeMappingContext
    {
        public NodeMappingContext(int id, string[] paths)
        {
            Id = id;
            Paths = (paths == null ? null : paths.ToList());
        }

        public NodeMappingContext(Node node, string[] paths)
            : this(node.Id, paths)
        {
            Node = node;
        }

        /// <summary>
        /// The node being mapped,if it has been loaded.
        /// </summary>
        private Node Node;

        /// <summary>
        /// The ID of the node being mapped.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The paths to be mapped.
        /// </summary>
        public List<string> Paths { get; set; }

        /// <summary>
        /// Gets the node being mapped.
        /// </summary>
        public Node GetNode()
        {
            if (Node != null)
            {
                return Node;
            }

            var node = new Node(Id);

            if (string.IsNullOrEmpty(node.Name))
            {
                return null;
            }

            return node;
        }
    }
}
