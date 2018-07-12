using System;

namespace OaiPmhNet.Converters
{
    public interface IDateConverter
    {
        bool TryDecode(string date, out DateTime dateTime);

        string Encode(string granularity, DateTime? date);
    }
}
