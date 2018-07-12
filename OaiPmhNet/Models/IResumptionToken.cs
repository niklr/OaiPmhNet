using System;
using System.Collections.Generic;

namespace OaiPmhNet.Models
{
    public interface IResumptionToken
    {
        // A UTC datetime value, which specifies a lower bound for datestamp-based selective harvesting.
        DateTime? From { get; }

        // A UTC datetime value, which specifies a upper bound for datestamp-based selective harvesting.
        DateTime? Until { get; }

        // A required argument, which specifies that headers should be returned 
        // only if the metadata format matching the supplied metadataPrefix is available or, 
        // depending on the repository's support for deletions, has been deleted. 
        string MetadataPrefix { get; }

        // An optional argument which specifies set criteria for selective harvesting.
        string Set { get; }

        // A UTC datetime indicating when the resumptionToken ceases to be valid.
        DateTime? ExpirationDate { get; }

        // An integer indicating the cardinality of the complete list 
        // (i.e., the sum of the cardinalities of the incomplete lists).
        int? CompleteListSize { get; }

        // A count of the number of elements of the complete list thus far returned 
        // (i.e. cursor starts at 0).
        int? Cursor { get; }

        // Optional custom parameters not part of the specification.
        IDictionary<string, string> Custom { get; }
    }
}
