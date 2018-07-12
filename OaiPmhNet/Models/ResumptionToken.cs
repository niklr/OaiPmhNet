using System;
using System.Collections.Generic;

namespace OaiPmhNet.Models
{
    public class ResumptionToken : IResumptionToken
    {
        public DateTime? From { get; set; }
        public DateTime? Until { get; set; }
        public string MetadataPrefix { get; set; }
        public string Set { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CompleteListSize { get; set; }
        public int? Cursor { get; set; }
        public IDictionary<string, string> Custom { get; set; } = new Dictionary<string, string>();
    }
}
