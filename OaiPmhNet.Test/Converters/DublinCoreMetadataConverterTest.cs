using System;
using System.Xml.Linq;
using NUnit.Framework;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;

namespace OaiPmhNet.Test.Converters
{
    [TestFixture]
    public class DublinCoreMetadataConverterTest
    {
        private DublinCoreMetadataConverter _converter;
        internal const string OAI_DC_ROOT = @"<oai_dc:dc xmlns:oai_dc=""http://www.openarchives.org/OAI/2.0/oai_dc/"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.openarchives.org/OAI/2.0/oai_dc/ http://www.openarchives.org/OAI/2.0/oai_dc.xsd"">";

        [OneTimeSetUp]
        public void Init()
        {
            OaiConfigurationTest.Init();
            _converter = new DublinCoreMetadataConverter(OaiConfiguration.Instance, new DateConverter());
        }

        [Test]
        public void DublinCoreMetadataConverter_Decode()
        {
            DublinCoreMetadata expected = new DublinCoreMetadata();
            expected.Title.Add("Title1");
            expected.Title.Add("Title2");
            expected.Creator.Add("Creator 1");
            expected.Creator.Add("Creator 2");
            expected.Date.Add(new DateTime(1987, 2, 16, 0, 0, 0, DateTimeKind.Utc));

            XElement metadata = XElement.Parse($@"{OAI_DC_ROOT}
  <dc:title>Title1</dc:title>
  <dc:title>Title2</dc:title>
  <dc:creator>Creator 1</dc:creator>
  <dc:creator>Creator 2</dc:creator>
  <dc:date>1987-02-16T00:00:00Z</dc:date>
</oai_dc:dc>");

            DublinCoreMetadata actual = _converter.Decode(metadata);

            Assert.AreEqual(expected.Title.Count, actual.Title.Count);
            Assert.AreEqual(expected.Title[0], actual.Title[0]);
            Assert.AreEqual(expected.Title[1], actual.Title[1]);
            Assert.AreEqual(expected.Creator[0], actual.Creator[0]);
            Assert.AreEqual(expected.Creator[1], actual.Creator[1]);
            Assert.AreEqual(expected.Date[0], actual.Date[0]);
        }

        [Test]
        public void DublinCoreMetadataConverter_Encode()
        {
            string expected = $@"{OAI_DC_ROOT}
  <dc:title>Title1</dc:title>
  <dc:title>Title2</dc:title>
  <dc:creator>Creator 1</dc:creator>
  <dc:creator>Creator 2</dc:creator>
  <dc:date>1987-02-16T00:00:00Z</dc:date>
</oai_dc:dc>";

            DublinCoreMetadata metadata = new DublinCoreMetadata();
            metadata.Title.Add("Title1");
            metadata.Title.Add("Title2");
            metadata.Creator.Add("Creator 1");
            metadata.Creator.Add("Creator 2");
            metadata.Date.Add(new DateTime(1987, 2, 16, 0, 0, 0, DateTimeKind.Utc));

            string actual = _converter.Encode(metadata).ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}
