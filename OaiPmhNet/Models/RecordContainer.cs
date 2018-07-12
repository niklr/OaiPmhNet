using System.Collections.Generic;
using System.Linq;

namespace OaiPmhNet.Models
{
    public class RecordContainer
    {
        public IEnumerable<Record> Records { get; set; } = Enumerable.Empty<Record>();

        public IResumptionToken ResumptionToken { get; set; }
    }
}
