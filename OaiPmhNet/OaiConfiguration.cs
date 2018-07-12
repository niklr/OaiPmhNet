using System;
using System.Collections.Generic;

namespace OaiPmhNet
{
    public class OaiConfiguration : IOaiConfiguration
    {
        private static volatile IOaiConfiguration instance;
        private static readonly object syncRoot = new object();

        private OaiConfiguration()
        {

        }

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static IOaiConfiguration Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new OaiConfiguration();
                    }
                }

                return instance;
            }
        }

        #region Identify configuration

        public string RepositoryName { get; set; } = "MyRepository";

        public Func<string> BaseUrl { get; set; } = () => { return "http://localhost/oai2"; };

        public string ProtocolVersion { get; set; } = "2.0";

        public string EarliestDatestamp { get; set; } = "1987-02-16T00:00:00Z";

        public string DeletedRecord { get; set; } = "no";

        public string Granularity { get; set; } = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";

        public string[] AdminEmails { get; set; } = new string[] { "test@domain.ch" };

        public string[] Compressions { get; set; } = new string[] { };

        public string[] Descriptions { get; set; } = new string[] { };

        #endregion

        #region Data Provider configuration

        public int PageSize { get; set; } = 10;

        public bool SupportSets { get; set; } = false;

        public TimeSpan? ExpirationTimeSpan { get; set; }

        #endregion

        #region Custom configuration

        public ISet<string> ResumptionTokenCustomParameterNames { get; set; } = new HashSet<string>();

        #endregion
    }
}
