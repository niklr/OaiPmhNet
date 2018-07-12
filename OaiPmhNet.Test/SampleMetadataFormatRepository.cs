using System.Collections.Generic;
using System.Linq;
using OaiPmhNet.Models;

namespace OaiPmhNet.Test
{
    public class SampleMetadataFormatRepository : IMetadataFormatRepository
    {
        private readonly Dictionary<string, MetadataFormat> _dictionary;

        public SampleMetadataFormatRepository(IList<MetadataFormat> metadataFormats)
        {
            _dictionary = metadataFormats.ToDictionary(f => f.Prefix, f => f);
        }

        public MetadataFormat GetByPrefix(string prefix)
        {
            if (_dictionary.TryGetValue(prefix, out MetadataFormat format))
                return format;
            else
                return null;
        }

        public IQueryable<MetadataFormat> GetQuery()
        {
            return _dictionary.Select(o => o.Value).AsQueryable();
        }
    }
}
