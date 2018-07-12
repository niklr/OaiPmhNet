using System;

namespace OaiPmhNet
{
    [Flags]
    public enum OaiArgument
    {
        None = 0,
        MetadataPrefix = 1,
        ResumptionToken = 2,
        Identifier = 4,
        From = 8,
        Until = 16,
        Set = 32
    }
}
