using System.Collections.Generic;
using System.Linq;

namespace OaiPmhNet.Models
{
    public class SetContainer
    {
        public IEnumerable<Set> Sets { get; set; } = Enumerable.Empty<Set>();

        public IResumptionToken ResumptionToken { get; set; }
    }
}
