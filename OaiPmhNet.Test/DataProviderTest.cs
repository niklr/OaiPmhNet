using System;
using NUnit.Framework;
using OaiPmhNet.Models;

namespace OaiPmhNet.Test
{
    public class DataProviderTest
    {
        private string _baseUrl;
        private DataProvider _dataProvider;
        private readonly string _xmlRoot = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>";
        private readonly string _oaiPmhRoot = @"<OAI-PMH xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd"" xmlns=""http://www.openarchives.org/OAI/2.0/"">";
        private readonly string _oaiDcRoot = @"<oai_dc:dc xmlns:oai_dc=""http://www.openarchives.org/OAI/2.0/oai_dc/"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.openarchives.org/OAI/2.0/oai_dc/ http://www.openarchives.org/OAI/2.0/oai_dc.xsd"">";

        [OneTimeSetUp]
        public void Init()
        {
            OaiConfigurationTest.Init();
            IOaiConfiguration configuration = OaiConfiguration.Instance;
            var metadataFormatRepository = SampleMetadataFormatRepositoryTest.Create();
            var recordRepository = SampleRecordRepositoryTest.Create();
            _baseUrl = configuration.BaseUrl();
            _dataProvider = new DataProvider(configuration, metadataFormatRepository, recordRepository);
        }

        [Test]
        public void DataProvider_Error_BadVerb()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request>{_baseUrl}</request>
  <error code=""badVerb"">Value of the verb argument is not a legal OAI-PMH verb, the verb argument is missing, or the verb argument is repeated.</error>
</OAI-PMH>";

            var arguments = new ArgumentContainer("nastyVerb");
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_GetRecord_BadArgument_Identifier_Required()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""GetRecord"">{_baseUrl}</request>
  <error code=""badArgument"">The request does not include the required 'identifier' argument.</error>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.GetRecord.ToString());
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_GetRecord_BadArgument_MetadataPrefix_Required()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""GetRecord"" identifier=""oai:smir.ch:1"">{_baseUrl}</request>
  <error code=""badArgument"">The request does not include the required 'metadataPrefix' argument.</error>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.GetRecord.ToString(), string.Empty, string.Empty, "oai:smir.ch:1");
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_GetRecord()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""GetRecord"" identifier=""12"" metadataPrefix=""oai_dc"">http://localhost/test</request>
  <GetRecord>
    <record>
      <header>
        <identifier>12</identifier>
        <datestamp>2018-02-16T02:00:00Z</datestamp>
      </header>
      <metadata>
        {_oaiDcRoot}
          <dc:title>Title 2</dc:title>
          <dc:creator>Owner 2</dc:creator>
          <dc:date>2018-02-16T02:00:00Z</dc:date>
          <dc:identifier>12</dc:identifier>
        </oai_dc:dc>
      </metadata>
    </record>
  </GetRecord>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.GetRecord.ToString(), "oai_dc", string.Empty, "12");
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_Identify_Error_BadArgument()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""Identify"" metadataPrefix=""oai_dc"">{_baseUrl}</request>
  <error code=""badArgument"">The request includes a 'metadataPrefix' argument that is not allowed for this verb.</error>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.Identify.ToString(), "oai_dc");
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_Identify()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""Identify"">{_baseUrl}</request>
  <Identify>
    <repositoryName>MyRepository</repositoryName>
    <baseURL>{_baseUrl}</baseURL>
    <protocolVersion>2.0</protocolVersion>
    <earliestDatestamp>1987-02-16T00:00:00Z</earliestDatestamp>
    <deletedRecord>no</deletedRecord>
    <granularity>yyyy-MM-ddTHH:mm:ssZ</granularity>
    <adminEmail>test@domain.ch</adminEmail>
  </Identify>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.Identify.ToString());
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_ListMetadataFormats_BadArgument()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""ListMetadataFormats"" metadataPrefix=""oai_dc"">{_baseUrl}</request>
  <error code=""badArgument"">The request includes a 'metadataPrefix' argument that is not allowed for this verb.</error>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.ListMetadataFormats.ToString(), "oai_dc");
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_ListMetadataFormats_IdDoesNotExist()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""ListMetadataFormats"" identifier=""123"">{_baseUrl}</request>
  <error code=""idDoesNotExist"">The value of the identifier argument is unknown or illegal in this repository.</error>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.ListMetadataFormats.ToString(), string.Empty, string.Empty, "123");
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_ListMetadataFormats()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""ListMetadataFormats"">{_baseUrl}</request>
  <ListMetadataFormats>
    <metadataFormat>
      <metadataPrefix>oai_dc</metadataPrefix>
      <metadataNamespace>http://www.openarchives.org/OAI/2.0/oai_dc/</metadataNamespace>
      <schema>http://www.openarchives.org/OAI/2.0/oai_dc.xsd</schema>
    </metadataFormat>
    <metadataFormat>
      <metadataPrefix>rdf</metadataPrefix>
      <metadataNamespace>http://www.w3.org/1999/02/22-rdf-syntax-ns#</metadataNamespace>
      <schema>http://www.openarchives.org/OAI/2.0/rdf.xsd</schema>
    </metadataFormat>
  </ListMetadataFormats>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.ListMetadataFormats.ToString());
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_ListRecords_BadArgument()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""ListRecords"" identifier=""123"">{_baseUrl}</request>
  <error code=""badArgument"">The request includes a 'identifier' argument that is not allowed for this verb.</error>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.ListRecords.ToString(), string.Empty, string.Empty, "123");
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_ListRecords_BadArgument_Required()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""ListRecords"">{_baseUrl}</request>
  <error code=""badArgument"">The request does not include the required 'metadataPrefix' argument.</error>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.ListRecords.ToString());
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_ListRecords_CannotDisseminateFormat()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""ListRecords"" metadataPrefix=""test"">{_baseUrl}</request>
  <error code=""cannotDisseminateFormat"">The value of the 'metadataPrefix' argument is not supported by repository.</error>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.ListRecords.ToString(), "test");
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_ListIdentifiers()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""ListIdentifiers"" metadataPrefix=""oai_dc"">http://localhost/test</request>
  <ListIdentifiers>
    <header>
      <identifier>11</identifier>
      <datestamp>2018-02-16T01:00:00Z</datestamp>
    </header>
    <header>
      <identifier>12</identifier>
      <datestamp>2018-02-16T02:00:00Z</datestamp>
    </header>
    <resumptionToken completeListSize=""5"" cursor=""0"">metadataPrefix%3doai_dc</resumptionToken>
  </ListIdentifiers>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.ListIdentifiers.ToString(), "oai_dc");
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DataProvider_ListRecords()
        {
            string expected = $@"{_xmlRoot}
{_oaiPmhRoot}
  <responseDate>2018-05-08T11:05:00Z</responseDate>
  <request verb=""ListRecords"" metadataPrefix=""oai_dc"">http://localhost/test</request>
  <ListRecords>
    <record>
      <header>
        <identifier>11</identifier>
        <datestamp>2018-02-16T01:00:00Z</datestamp>
      </header>
      <metadata>
        {_oaiDcRoot}
          <dc:title>Title 1</dc:title>
          <dc:creator>Owner 1</dc:creator>
          <dc:contributor>Contributor 1</dc:contributor>
          <dc:contributor>Contributor 1.1</dc:contributor>
          <dc:date>2018-02-16T01:00:00Z</dc:date>
          <dc:identifier>11</dc:identifier>
        </oai_dc:dc>
      </metadata>
    </record>
    <record>
      <header>
        <identifier>12</identifier>
        <datestamp>2018-02-16T02:00:00Z</datestamp>
      </header>
      <metadata>
        {_oaiDcRoot}
          <dc:title>Title 2</dc:title>
          <dc:creator>Owner 2</dc:creator>
          <dc:date>2018-02-16T02:00:00Z</dc:date>
          <dc:identifier>12</dc:identifier>
        </oai_dc:dc>
      </metadata>
    </record>
    <resumptionToken completeListSize=""5"" cursor=""0"">metadataPrefix%3doai_dc</resumptionToken>
  </ListRecords>
</OAI-PMH>";

            var arguments = new ArgumentContainer(OaiVerb.ListRecords.ToString(), "oai_dc");
            var actual = _dataProvider.ToString(new DateTime(2018, 5, 8, 11, 5, 0, DateTimeKind.Utc), arguments);

            Assert.AreEqual(expected, actual);
        }
    }
}
