using System;
using System.Collections.Generic;
using System.Linq;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;

namespace OaiPmhNet.Test
{
    public class SampleRecordRepository : IRecordRepository
    {
        private readonly IOaiConfiguration _configuration;
        private readonly IList<SampleRecord> _records;
        private readonly IDateConverter _dateConverter;
        private readonly IDublinCoreMetadataConverter _dublinCoreMetadataConverter;

        public SampleRecordRepository(IOaiConfiguration configuration, IList<SampleRecord> records, 
            IDateConverter dateConverter, IDublinCoreMetadataConverter dublinCoreMetadataConverter)
        {
            _configuration = configuration;
            _records = records;
            _dateConverter = dateConverter;
            _dublinCoreMetadataConverter = dublinCoreMetadataConverter;
        }

        public Record GetRecord(string identifier, string metadataPrefix)
        {
            if (int.TryParse(identifier, out int parsedIdentifier))
                return ToRecord(_records.FirstOrDefault(r => r.Id == parsedIdentifier));
            else
                return null;
        }

        public RecordContainer Get(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            RecordContainer container = new RecordContainer();

            IQueryable<SampleRecord> records = _records.AsQueryable().OrderBy(r => r.Id);

            int totalCount = records.Count();

            if (resumptionToken != null)
            {
                if (resumptionToken.From.HasValue)
                    records = records.Where(r => r.Date >= resumptionToken.From.Value);
                if (resumptionToken.Until.HasValue)
                    records = records.Where(r => r.Date <= resumptionToken.Until.Value);
                if (resumptionToken.Custom.ContainsKey("offset") && resumptionToken.Custom.TryGetValue("offset", out string offset))
                {
                    if (int.TryParse(offset, out int parsedOffset))
                        records = records.Where(r => r.Id > parsedOffset);
                }
            }

            ResumptionToken newResumptionToken = null;

            if (records.Count() > _configuration.PageSize)
            {
                newResumptionToken = new ResumptionToken();
                container.ResumptionToken = newResumptionToken;

                if (_dateConverter.TryDecode(arguments.From, out DateTime from))
                    newResumptionToken.From = from;
                if (_dateConverter.TryDecode(arguments.Until, out DateTime until))
                    newResumptionToken.Until = until;
                newResumptionToken.MetadataPrefix = arguments.MetadataPrefix;
                newResumptionToken.Set = arguments.Set;
                if (_configuration.ExpirationTimeSpan.HasValue)
                    newResumptionToken.ExpirationDate = DateTime.UtcNow.Add(_configuration.ExpirationTimeSpan.Value);
                newResumptionToken.CompleteListSize = totalCount;
                newResumptionToken.Cursor = 0;
            }

            records = records.Take(_configuration.PageSize);

            if (newResumptionToken != null)
            {
                if (resumptionToken != null)
                {
                    // Increase the cursor value
                    newResumptionToken.Cursor = resumptionToken.Cursor.HasValue ?
                        resumptionToken.Cursor.Value + _configuration.PageSize : _configuration.PageSize;
                }

                // Add custom offset value
                var lastRecord = records.LastOrDefault();
                if (lastRecord != null)
                    newResumptionToken.Custom.Add("offset", lastRecord.Id.ToString());
            }

            container.Records = records.Select(r => ToRecord(r));

            return container;
        }

        public RecordContainer GetIdentifiers(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            return Get(arguments, resumptionToken);
        }

        #region private methods

        private Record ToRecord(SampleRecord sampleRecord)
        {
            if (sampleRecord == null)
                return null;

            return new Record()
            {
                Header = new RecordHeader()
                {
                    Identifier = sampleRecord.Id.ToString(),
                    Datestamp = sampleRecord.Date
                },
                Metadata = new RecordMetadata()
                {
                    Content = _dublinCoreMetadataConverter.Encode(new DublinCoreMetadata()
                    {
                        Title = new List<string>() { sampleRecord.Title },
                        Creator = new List<string>() { sampleRecord.Owner },
                        Contributor = sampleRecord.Contributors,
                        Date = new List<DateTime>() { sampleRecord.Date },
                        Identifier = new List<string> { sampleRecord.Id.ToString() }
                    })
                }
            };
        }

        #endregion
    }
}
