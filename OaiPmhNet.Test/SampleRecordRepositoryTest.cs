using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;
using OaiPmhNet.Test.Converters;

namespace OaiPmhNet.Test
{
    [TestFixture]
    public class SampleRecordRepositoryTest
    {
        private SampleRecordRepository _repository;

        public static SampleRecordRepository Create()
        {
            IList<SampleRecord> records = new List<SampleRecord>()
            {
                new SampleRecord()
                {
                    Id = 11,
                    Date = new DateTime(2018, 2, 16, 1, 0, 0, DateTimeKind.Utc),
                    Title = "Title 1",
                    Owner = "Owner 1",
                    Contributors = new List<string>() { "Contributor 1", "Contributor 1.1" }
                },
                new SampleRecord()
                {
                    Id = 13,
                    Date = new DateTime(2018, 2, 16, 3, 0, 0, DateTimeKind.Utc),
                    Title = "Title 3"
                },
                new SampleRecord()
                {
                    Id = 12,
                    Date = new DateTime(2018, 2, 16, 2, 0, 0, DateTimeKind.Utc),
                    Title = "Title 2",
                    Owner = "Owner 2"
                },
                new SampleRecord()
                {
                    Id = 14,
                    Date = new DateTime(2018, 2, 16, 4, 0, 0, DateTimeKind.Utc),
                    Title = "Title 4",
                    Owner = "Owner 4"
                },
                new SampleRecord()
                {
                    Id = 15,
                    Date = new DateTime(2018, 2, 16, 5, 0, 0, DateTimeKind.Utc),
                    Title = "Title 5",
                    Owner = "Owner 5",
                    Contributors = new List<string>() { "Contributor 5" }
                }
            };

            IOaiConfiguration configuration = OaiConfiguration.Instance;
            DateConverter dateConverter = new DateConverter();
            DublinCoreMetadataConverter dublinCoreMetadataConverter = new DublinCoreMetadataConverter(configuration, dateConverter);

            return new SampleRecordRepository(OaiConfiguration.Instance, records, dateConverter, dublinCoreMetadataConverter);
        }

        [OneTimeSetUp]
        public void Init()
        {
            OaiConfigurationTest.Init();

            _repository = Create();
        }

        [Test]
        public void SampleRecordRepository_Get()
        {
            var expected1 = new RecordContainer()
            {
                Records = new List<Record>()
                {
                    new Record()
                    {
                        Header = new RecordHeader()
                        {
                            Identifier = "11"
                        },
                        Metadata = new RecordMetadata()
                        {
                            Content = XElement.Parse($@"{DublinCoreMetadataConverterTest.OAI_DC_ROOT}
  <dc:title>Title 1</dc:title>
  <dc:creator>Owner 1</dc:creator>
  <dc:contributor>Contributor 1</dc:contributor>
  <dc:contributor>Contributor 1.1</dc:contributor>
  <dc:date>2018-02-16T01:00:00Z</dc:date>
  <dc:identifier>11</dc:identifier>
</oai_dc:dc>")
                        }
                    },
                    new Record()
                    {
                        Header = new RecordHeader()
                        {
                            Identifier = "12"
                        }
                    }
                },
                ResumptionToken = new ResumptionToken()
                {
                    CompleteListSize = 5,
                    Cursor = 0,
                    Custom = new Dictionary<string, string>() { { "offset", "12" } }
                }
            };

            var actual1 = _repository.Get(new ArgumentContainer(OaiVerb.ListRecords.ToString()));

            Assert.IsNotNull(actual1.ResumptionToken);
            Assert.AreEqual(expected1.ResumptionToken.CompleteListSize, actual1.ResumptionToken.CompleteListSize);
            Assert.AreEqual(expected1.ResumptionToken.Cursor, actual1.ResumptionToken.Cursor);
            Assert.AreEqual(expected1.Records.ToList()[0].Header.Identifier, actual1.Records.ToList()[0].Header.Identifier);
            Assert.AreEqual(expected1.Records.ToList()[1].Header.Identifier, actual1.Records.ToList()[1].Header.Identifier);
            Assert.AreEqual(expected1.Records.ToList()[0].Metadata.Content.ToString(), actual1.Records.ToList()[0].Metadata.Content.ToString());
            Assert.AreEqual(expected1.ResumptionToken.Custom["offset"], actual1.ResumptionToken.Custom["offset"]);

            var expected2 = new RecordContainer()
            {
                Records = new List<Record>()
                {
                    new Record()
                    {
                        Header = new RecordHeader()
                        {
                            Identifier = "13"
                        }
                    },
                    new Record()
                    {
                        Header = new RecordHeader()
                        {
                            Identifier = "14"
                        }
                    }
                },
                ResumptionToken = new ResumptionToken()
                {
                    CompleteListSize = 5,
                    Cursor = 2,
                    Custom = new Dictionary<string, string>() { { "offset", "14" } }
                }
            };

            var actual2 = _repository.Get(new ArgumentContainer(OaiVerb.ListRecords.ToString()), actual1.ResumptionToken);

            Assert.IsNotNull(actual2.ResumptionToken);
            Assert.AreEqual(expected2.ResumptionToken.CompleteListSize, actual2.ResumptionToken.CompleteListSize);
            Assert.AreEqual(expected2.ResumptionToken.Cursor, actual2.ResumptionToken.Cursor);
            Assert.AreEqual(expected2.Records.ToList()[0].Header.Identifier, actual2.Records.ToList()[0].Header.Identifier);
            Assert.AreEqual(expected2.Records.ToList()[1].Header.Identifier, actual2.Records.ToList()[1].Header.Identifier);
            Assert.AreEqual(expected2.ResumptionToken.Custom["offset"], actual2.ResumptionToken.Custom["offset"]);
        }
    }
}
