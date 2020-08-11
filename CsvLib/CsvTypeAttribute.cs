using System;

namespace CsvHelperTest
{
    public class CsvTypeAttribute : Attribute
    {
        public string Delimiter { get; set; } = ",";
        public string LineDelimiter { get; set; } = Environment.NewLine;
        public string FieldQuote { get; set; } = @"""";

        public CsvTypeAttribute(string delimiter = null,
                                string lineDelimiter = null,
                                string fieldQuote = null)
        {
            Delimiter = delimiter ?? Delimiter;
            LineDelimiter = lineDelimiter ?? LineDelimiter;
            FieldQuote = fieldQuote ?? FieldQuote;
        }
    }


}
