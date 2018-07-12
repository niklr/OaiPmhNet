using System;
using System.Collections.Generic;

namespace OaiPmhNet.Models
{
    /// <summary>
    /// Dublin Core Metadata Element Set, Version 1.1
    /// http://dublincore.org/documents/dces/
    /// </summary>
    public class DublinCoreMetadata
    {
        public IList<string> Title { get; set; } = new List<string>();
        public IList<string> Creator { get; set; } = new List<string>();
        public IList<string> Subject { get; set; } = new List<string>();
        public IList<string> Description { get; set; } = new List<string>();
        public IList<string> Publisher { get; set; } = new List<string>();
        public IList<string> Contributor { get; set; } = new List<string>();
        public IList<DateTime> Date { get; set; } = new List<DateTime>();
        public IList<string> Type { get; set; } = new List<string>();
        public IList<string> Format { get; set; } = new List<string>();
        public IList<string> Identifier { get; set; } = new List<string>();
        public IList<string> Source { get; set; } = new List<string>();
        public IList<string> Language { get; set; } = new List<string>();
        public IList<string> Relation { get; set; } = new List<string>();
        public IList<string> Coverage { get; set; } = new List<string>();
        public IList<string> Rights { get; set; } = new List<string>();
    }
}
