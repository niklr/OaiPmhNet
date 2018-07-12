using System;
using System.Collections.Generic;

namespace OaiPmhNet
{
    public interface IOaiConfiguration
    {
        #region Identify configuration

        /// <summary>
        /// A human readable name for the repository.
        /// </summary>
        string RepositoryName { get; set; }

        /// <summary>
        /// The base URL of the repository.
        /// </summary>
        Func<string> BaseUrl { get; set; }

        /// <summary>
        /// The version of the OAI-PMH supported by the repository.
        /// </summary>
        string ProtocolVersion { get; set; }

        /// <summary>
        /// A UTCdatetime that is the guaranteed lower limit of all datestamps recording changes, modifications, or deletions in the repository.
        /// </summary>
        string EarliestDatestamp { get; set; }

        /// <summary>
        /// The manner in which the repository supports the notion of deleted records.
        /// </summary>
        string DeletedRecord { get; set; }

        /// <summary>
        /// The finest harvesting granularity supported by the repository. 
        /// </summary>
        string Granularity { get; set; }

        /// <summary>
        /// The e-mail addresses of the administrators of the repository.
        /// </summary>
        string[] AdminEmails { get; set; }

        /// <summary>
        /// Compression encodings supported by the repository.
        /// </summary>
        string[] Compressions { get; set; }

        /// <summary>
        /// An extensible mechanism for communities to describe their repositories.
        /// </summary>
        string[] Descriptions { get; set; }

        #endregion

        #region Data Provider configuration

        /// <summary>
        /// A numeric value indicating the page size used for pagination.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// A boolean value indicating whether sets are supported.
        /// </summary>
        bool SupportSets { get; set; }

        /// <summary>
        /// An optional time interval indicating when the resumption token ceases to be valid.
        /// </summary>
        TimeSpan? ExpirationTimeSpan { get; set; }

        #endregion

        #region Custom configuration

        /// <summary>
        /// Optional custom parameter names in the resumption token.
        /// </summary>
        ISet<string> ResumptionTokenCustomParameterNames { get; set; }

        #endregion
    }
}
