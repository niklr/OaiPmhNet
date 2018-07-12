using System;
using System.Collections.Generic;
using System.Linq;

namespace OaiPmhNet.Models
{
    public class RecordHeader
    {
        /// <summary>
        /// The unique identifier of an item in a repository.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// The date of creation, modification or deletion of the record for the purpose of selective harvesting.
        /// </summary>
        public DateTime? Datestamp { get; set; }

        /// <summary>
        /// The set memberships of the item for the purpose of selective harvesting.
        /// </summary>
        public IEnumerable<string> SetSpecs { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Optional with a value of deleted indicating the withdrawal of availability of the specified metadata 
        /// format for the item, dependent on the repository support for deletions.
        /// </summary>
        public string Status { get; set; }
    }
}
