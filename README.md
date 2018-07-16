OaiPmhNet
=========

OaiPmhNet is a .NET library implementing the OAI-PMH specification. 
The Open Archives Initiative Protocol for Metadata Harvesting (OAI-PMH) is a low-barrier mechanism for repository interoperability. 
Data Providers are repositories that expose structured metadata via OAI-PMH. 
Service Providers then make OAI-PMH service requests to harvest that metadata. 
OAI-PMH is a set of six verbs or services that are invoked within HTTP.

[http://www.openarchives.org/pmh/](http://www.openarchives.org/pmh/)

### Dependencies:
* [NETStandard.Library 2.0.1](https://www.nuget.org/packages/NETStandard.Library/2.0.1)

## Implementation steps

The OaiPmhNet library can be customized to work with arbitrary data repositories by implementing 3 interfaces. 
A demonstration implementation can be found in the unit test project.

### 1. Implement IRecordRepository interface
```csharp
public interface IRecordRepository
{
	Record GetRecord(string identifier, string metadataPrefix);
	RecordContainer GetRecords(ArgumentContainer arguments, IResumptionToken resumptionToken = null);
	RecordContainer GetIdentifiers(ArgumentContainer arguments, IResumptionToken resumptionToken = null);
}
```

### 2. Implement IMetadataFormatRepository interface
```csharp
public interface IMetadataFormatRepository
{
	MetadataFormat GetByPrefix(string prefix);
	IQueryable<MetadataFormat> GetQuery();
}
```

### 3. Implement ISetRepository interface
```csharp
public interface ISetRepository
{
	SetContainer GetSets(ArgumentContainer arguments, IResumptionToken resumptionToken = null);
}
```

### 4. Override default configuration (example)
```csharp
var config = OaiConfiguration.Instance;

config.RepositoryName = constants.APPLICATION_NAME;
config.BaseUrl = () =>
{
	Uri baseUri = new Uri(UrlHelper.BaseSiteUri, "oai2");
	return baseUri.AbsoluteUri;
};
config.DeletedRecord = "transient";
config.AdminEmails = new string[] { constants.SUPPORT_EMAIL };
config.ResumptionTokenCustomParameterNames.Add("offset");
```

### 5. Initialize the DataProvider class
```csharp
DataProvider provider = new DataProvider(configuration, metadataFormatRepository, recordRepository);
```

### 6. Pass ArgumentContainer to DataProvider
```csharp
ArgumentContainer arguments = new ArgumentContainer(verb, metadataPrefix, resumptionToken, identifier, from, until, set);
XDocument document = provider.ToXDocument(DateTime.Now, arguments);
```

### 7. (Optional) Convert response to XHTML using XSLT
```csharp
document?.Root?.AddBeforeSelf(new XProcessingInstruction("xml-stylesheet", "type='text/xsl' href='/Content/xsl/oai2.xsl'"));
return this.Content(provider.ToString(document), "application/xml");
```