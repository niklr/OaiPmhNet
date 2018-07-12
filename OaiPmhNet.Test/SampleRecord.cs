using System;
using System.Collections.Generic;

namespace OaiPmhNet.Test
{
    public class SampleRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
        public IList<string> Contributors { get; set; } = new List<string>();
    }
}
