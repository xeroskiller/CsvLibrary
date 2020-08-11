using System;

namespace CsvLibrary
{
    public class CsvTypeAttribute : Attribute
    {
        public string Delimiter { get; set; } = ",";
        public string LineDelimiter { get; set; } = Environment.NewLine;
        public string FieldQuote { get; set; } = @"""";
        public bool HasHeader { get; set; } = true;

        public CsvTypeAttribute(string delimiter = null,
                                string lineDelimiter = null,
                                string fieldQuote = null,
                                bool? hasHeader = null)
        {
            Delimiter = delimiter ?? Delimiter;
            LineDelimiter = lineDelimiter ?? LineDelimiter;
            FieldQuote = fieldQuote ?? FieldQuote;
            HasHeader = hasHeader ?? true;
        }
    }


}
