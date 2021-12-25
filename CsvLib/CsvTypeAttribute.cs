using System;

namespace CsvLibrary
{
    /// <summary>
    /// Represents type-level CSV serialization spec. Applied to a class or struct, allows
    /// simple serialization and deserialization.
    /// </summary>
    public class CsvTypeAttribute : Attribute
    {
        /// <summary>
        /// Delimiter used to separate individual fields in a record.
        /// </summary>
        public string FieldDelimiter { get; private set; } = ",";

        /// <summary>
        /// Delimiter used to separate records in a file.
        /// </summary>
        public string LineDelimiter { get; private set; } = Environment.NewLine;

        /// <summary>
        /// Reflective string used to delimit beginning and end of a field value, in the case
        /// that is collider with specified delimmiter values.
        /// </summary>
        public string FieldQuote { get; private set; } = @"""";

        /// <summary>
        /// Boolean value to indicate whether this file type requires a header. Null value
        /// indicates implicit header detection.
        /// </summary>
        public bool HasHeader { get; private set; } = true;

        /// <summary>
        /// File-level default dateTime format string for serialization.
        /// </summary>
        public string DateTimeFormatString { get; private set; } = null;

        public CsvTypeAttribute(string fieldDelimiter = null,
                                string lineDelimiter = null,
                                string fieldQuote = null,
                                bool hasHeader = true,
                                string dateTimeFormatString = null)
        {
            FieldDelimiter = fieldDelimiter ?? FieldDelimiter;
            LineDelimiter = lineDelimiter ?? LineDelimiter;
            FieldQuote = fieldQuote ?? FieldQuote;

            HasHeader = hasHeader;
            DateTimeFormatString = dateTimeFormatString;
        }
    }
}
