using System.Collections.Generic;
using OaiPmhNet.Models;

namespace OaiPmhNet
{
    /// <summary>
    /// The interface representing the repository for metadata formats.
    /// </summary>
    public interface IMetadataFormatRepository
    {
        /// <summary>
        /// Gets the metadata format corresponding to the provided prefix.
        /// </summary>
        /// <param name="prefix">The prefix to specify the metadata format.</param>
        /// <returns>The metadata format corresponding to the provided prefix.</returns>
        MetadataFormat GetMetadataFormat(string prefix);

        /// <summary>
        /// Gets a list of available metadata formats.
        /// </summary>
        /// <returns>A list of available metadata formats.</returns>
        IEnumerable<MetadataFormat> GetMetadataFormats();
    }
}
