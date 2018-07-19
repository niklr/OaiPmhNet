using System.Collections.Generic;
using OaiPmhNet.Models;

namespace OaiPmhNet
{
    public interface IMetadataFormatRepository
    {
        MetadataFormat GetMetadataFormat(string prefix);
        IEnumerable<MetadataFormat> GetMetadataFormats();
    }
}
