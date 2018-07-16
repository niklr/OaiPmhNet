using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OaiPmhNet.Models
{
    public class Set
    {
        /// <summary>
        /// A colon [:] separated list indicating the path from the root of the set hierarchy to the respective node.
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// A short human-readable string naming the set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An optional description of the set.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// An optional and repeatable container that may hold community-specific XML-encoded data about the set.
        /// </summary>
        public IEnumerable<XElement> AdditionalDescriptions { get; set; } = Enumerable.Empty<XElement>();
    }
}
